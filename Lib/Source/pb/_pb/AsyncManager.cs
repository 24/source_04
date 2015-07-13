using System;
using System.Threading;

namespace pb
{
    public abstract class AsyncManager : IDisposable
    {
        protected static TimeSpan __defaultTimeoutStopBackgroundThread = TimeSpan.FromSeconds(30);
        protected static TimeSpan __defaultSleepTime = TimeSpan.FromSeconds(5);
        protected TimeSpan _timeoutStopBackgroundThread = TimeSpan.FromSeconds(30);
        protected TimeSpan _sleepTime = TimeSpan.FromSeconds(5);
        protected Thread _backgroundThread = null;
        protected bool _stopBackgroundThread = false;
        protected Action<Exception> _onError = exception => TraceException(exception);

        public TimeSpan TimeoutStopBackgroundThread { get { return _timeoutStopBackgroundThread; } set { _timeoutStopBackgroundThread = value; } }
        public TimeSpan SleepTime { get { return _sleepTime; } set { _sleepTime = value; } }
        public Action<Exception> OnError { get { return _onError; } set { _onError = value; } }

        public void Dispose()
        {
            Stop();
        }

        public virtual void Start()
        {
            StartBackgroundThread();
        }

        public virtual void Stop()
        {
            StopBackgroundThread();
        }

        public bool IsStarted()
        {
            return _backgroundThread != null;
        }

        protected void StartBackgroundThread()
        {
            if (_backgroundThread != null)
                return;
            _stopBackgroundThread = false;
            _backgroundThread = new Thread(new ThreadStart(BackgroundThread));
            _backgroundThread.Start();
        }

        protected void StopBackgroundThread()
        {
            if (_backgroundThread == null)
                return;
            _stopBackgroundThread = true;
            if (!_backgroundThread.Join(_timeoutStopBackgroundThread))
                throw new PBException("error unable to stop background thread");
            _backgroundThread = null;
            _stopBackgroundThread = false;
        }

        protected void BackgroundThread()
        {
            while (true)
            {
                if (_stopBackgroundThread)
                    break;
                try
                {
                    ThreadExecute();
                }
                catch (Exception exception)
                {
                    if (_onError != null)
                        _onError(exception);
                }
                Thread.Sleep(_sleepTime);
            }
        }

        protected static void TraceException(Exception exception)
        {
            Trace.WriteLine("error : {0}", exception.Message);
            Trace.WriteLine(exception.StackTrace);
        }

        protected abstract void ThreadExecute();
    }
}
