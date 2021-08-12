using System;
using System.Threading;

namespace EmulatorHost.Network
{
    public class WorkerThread
    {
        private readonly ManualResetEventSlim manualResetEvent = new ManualResetEventSlim(false);

        private readonly Thread thread;

        public WorkerThread(Action<Func<bool>> execution, int millisecondsWaitTimeOut)
        {
            void Runner()
            {
                execution(() => !manualResetEvent.WaitHandle.WaitOne(millisecondsWaitTimeOut));
                //todo event on stop
            }

            thread = new Thread(Runner);
        }

        public void Start()
        {

            thread.Start();
        }

        public void Stop()
        {
            manualResetEvent.Set();
        }

        public bool IsRunning() => thread.IsAlive;
    }
}