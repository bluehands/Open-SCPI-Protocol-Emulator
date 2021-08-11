//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Concurrent;
//using System.Net;
//using System.Net.Sockets;
//using System.Reactive.Linq;
//using System.Text;
//using System.Threading;
//using EmulatorHost.NetworkInterface;
//using FluentAssertions;
//using FunicularSwitch;
//using Microsoft.Extensions.Logging.Abstractions;

//namespace EmulatorHostTests.NetworkInterface
//{
//    [TestClass]
//    public class TcpServerTests
//    {
//        [TestMethod]
//        public void Start_StateUnderTest_ExpectedBehavior()
//        {
//            // Arrangenew 
//            var socketServer = new EmulatorHost.NetworkInterface.TcpServer();
//            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5025);
//            var queue = new ConcurrentQueue<byte[]>();
//            socketServer.Start(endPoint, queue);

//            // Act
//            socketServer.Start(endPoint, queue);

//            // Assert
//        }

//        [TestMethod]
//        public void Stop_StateUnderTest_ExpectedBehavior()
//        {
//            // Arrange
//            var socketServer = new EmulatorHost.NetworkInterface.TcpServer();
//            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5025);
//            var queue = new ConcurrentQueue<byte[]>();
//            socketServer.Start(endPoint, queue);

//           // var result = socketServer.Stop();

//            // Assert
//            //startResult.Should().BeOfType<Ok<Unit>>();
//            //result.Should().BeOfType<Ok<Unit>>("worker thread should be closed");
//        }

//        [TestMethod]
//        public void SendToHostServer_StateUnderTest_ExpectedBehavior()
//        {
//            // Arrange
//            var socketServer = new EmulatorHost.NetworkInterface.TcpServer();
//            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5025);
//            var queue = new ConcurrentQueue<byte[]>();
//            socketServer.Start(endPoint, queue);

//            Thread.Sleep(1000);




//            var message1 = "hi";
//            var message2 = "ttt";

//            var res = socketServer.ReceiveStream.FirstAsync();

            

//            var client = new TcpClient();
//            client.Connect(endPoint);

//            var networkStream = client.GetStream();
//            networkStream.Write(Encoding.ASCII.GetBytes(message1+"\n"));
//            client.Close();
//            res.Wait().Should().Be(message1);


//            socketServer.ReceiveStream.Subscribe(res => Console.WriteLine(res));
//            client = new TcpClient();
//            client.Connect(endPoint);
//            networkStream.Write(Encoding.ASCII.GetBytes(message2 + "\n"));

//            Thread.Sleep(2000);

//           socketServer.Stop();
//        }        
        
//        [TestMethod]
//        public void ReceiveFromHostServer_StateUnderTest_ExpectedBehavior()
//        {
//            // Arrange

//            var socketServer = new EmulatorHost.NetworkInterface.TcpServer();
//            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5025);
//            var queue = new ConcurrentQueue<byte[]>();
//            socketServer.Start(endPoint, queue);


//            Thread.Sleep(100);


//            var client = new TcpClient();
//            client.Connect(endPoint);
//            var message = "hi";
//            queue.Enqueue(Encoding.ASCII.GetBytes(message));
//            var buffer = new byte[300];
//            var networkStream = client.GetStream();
//            var bytes = networkStream.Read(buffer);

//            Encoding.ASCII.GetString(buffer, 0, bytes).Should().Be(message);

//            client.Connected.Should().BeTrue();
//            networkStream.CanWrite.Should().BeTrue();
//            networkStream.Write(Encoding.ASCII.GetBytes(message));

//            socketServer.Stop();
//        }

//        [TestMethod]
//        public void SendToHostServer_SctedBehavior()
//        {
//            // Arrange

//            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.1.126"), 5025);
//            var client = new TcpClient();
//            client.SendBufferSize = 100;
//            client.ReceiveBufferSize = 100;
//            client.Connect(endPoint);

//            var buffer = Encoding.ASCII.GetBytes("*IDN?\n");
//            var networkStream = client.GetStream();
//            networkStream.Write(buffer, 0, buffer.Length);
//            var buf = new byte[100];
//            var bytes = networkStream.Read(buf, 0, buf.Length);
//            Console.WriteLine(Encoding.ASCII.GetString(buf, 0, bytes));

//        }
//    }
//}