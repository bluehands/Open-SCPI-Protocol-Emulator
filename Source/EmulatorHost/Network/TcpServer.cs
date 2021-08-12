using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace EmulatorHost.Network
{
    public class TcpServer
    {
        private readonly ILogger<TcpServer> logger;
        private TcpListener tcpListener;

        private readonly WorkerThread connectionThread;
        private WorkerThread transmissionThread;

        private static readonly char separator = '\n';

        private static readonly int readBufferCapacity = 200;
        private readonly byte[] readBuffer = new byte[readBufferCapacity];
        
        private ConcurrentQueue<IByteArrayConvertible> queue;

        private readonly Func<string, byte[]> byteArrayConverter = val => Encoding.ASCII.GetBytes(val);

        private readonly DataBuilder dataBuilder;
        public IObservable<string> ReceiveStream => dataBuilder.DataStream;


        public TcpServer(ILogger<TcpServer> logger)
        {
            this.logger = logger;
            dataBuilder = new DataBuilder();


            connectionThread = new WorkerThread(ConnectionWorker, 1000);
            transmissionThread = new WorkerThread(TransmissionWorker, 15);
        }

        public void Start(IPEndPoint endPoint, ConcurrentQueue<IByteArrayConvertible> outputQueue)
        {
            this.queue = outputQueue;
            tcpListener = new TcpListener(endPoint);
            tcpListener.Start(1);
            connectionThread.Start();
            logger.LogInformation($"Socket server on {endPoint} running.");
        }

        public void Stop()
        {
            dataBuilder.Stop();
            transmissionThread.Stop();
            connectionThread.Stop();
            tcpListener?.Stop();
        }

        private void ConnectionWorker(Func<bool> isRunning)
        {
            while (isRunning())
            {
                if (!tcpListener.Pending()) continue;
                logger.LogInformation("Pending connection in queue...");
                DiscardConnectionAndRestart();
            }
        }

        private void DiscardConnectionAndRestart()
        {
            transmissionThread.Stop();
            //todo wait for stop?
            transmissionThread = new WorkerThread(TransmissionWorker, 15);
            transmissionThread.Start();
        }

        private void TransmissionWorker(Func<bool> assertIsRunning)
        {
            logger.LogInformation($"Accepting clients at <{tcpListener.LocalEndpoint}>...");
            using var client = tcpListener.AcceptTcpClient();
            client.ReceiveTimeout = 40;

            if (client.Connected)
            {
                logger.LogInformation(
                    $"Client from <{client.Client.RemoteEndPoint}> connected to <{tcpListener.LocalEndpoint}>");
                using var stream = client.GetStream();
                while (client.Connected && assertIsRunning())
                {
                    try
                    {
                        HandleTransmission(assertIsRunning, stream);
                    }
                    catch (Exception e)
                    {
                        logger.LogError($"{e.Message}");
                        // todo handle exception
                    }
                }

                logger.LogInformation(
                    $"Client lost connection or another client connected <{client.Client.RemoteEndPoint}>");
            }
            else
            {
                //todo accepted but not connected
                logger.LogError("Accepted client is not connected");
            }

            logger.LogInformation("Closed transmission thread");
        }

        private void HandleTransmission(Func<bool> assertIsRunning, NetworkStream stream)
        {
            if (stream.CanRead)
            {
                while (stream.DataAvailable && assertIsRunning())
                {
                    var bytes = stream.Read(readBuffer, 0, readBuffer.Length);
                    dataBuilder.Append(Encoding.ASCII.GetString(readBuffer, 0, bytes));
                }
            }
            else
            {
                //todo err data but not readable
                logger.LogCritical("Stream data no readable");
            }


            if (stream.CanWrite)
            {
                if (queue.IsEmpty) return;
                while (!queue.IsEmpty && assertIsRunning())
                {
                    if (queue.TryDequeue(out var result))
                    {
                        var data = result.ToByteArray(byteArrayConverter);
                        stream.Write(data, 0, data.Length);
                    }
                }

                stream.Write(Encoding.ASCII.GetBytes(separator.ToString()));
                stream.Flush();
            }
            else
            {
                //todo error cannot send to stream
                logger.LogCritical("Stream not writable");
            }
        }
    }
}