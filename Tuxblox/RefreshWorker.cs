using System;
using System.Threading;

namespace Tuxblox
{
    public class RefreshWorker
    {
        private Thread _RefreshThread;
        private int _WaitTime;

        public RefreshWorker(int intervalInMilliseconds, Action callback)
        {
            _RefreshThread = new Thread(new ThreadStart(() => Refresh(callback)));
            _WaitTime = intervalInMilliseconds;

            _RefreshThread.Start();
        }

        public void Stop()
        {
            _RefreshThread.Abort();
        }

        private void Refresh(Action callback)
        {
            while (true)
            {
                callback.Invoke();

                Thread.Sleep(_WaitTime);
            }
        }
    }
}
