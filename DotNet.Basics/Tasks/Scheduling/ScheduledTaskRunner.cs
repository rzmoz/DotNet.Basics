using System;
using System.Threading;

namespace DotNet.Basics.Tasks.Scheduling
{
    public class ScheduledTaskRunner : IDisposable
    {
        private Timer _timer;

        public ScheduledTaskRunner(ISyncTask task, TimeSpan interval, StartOptions startOptions = StartOptions.InitStarted)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            Task = task;
            Interval = interval;
            if (startOptions == StartOptions.InitStarted)
                Start();
        }

        public ISyncTask Task { get; }
        public TimeSpan Interval { get; }

        public void Start()
        {
            Dispose();
            _timer = new Timer(Run, new AutoResetEvent(false), TimeSpan.Zero, Interval);
        }

        public void Stop()
        {
            Dispose();
        }

        //Called from the Timer so it already runs in a background thread
        protected virtual void Run(object state)
        {
            Task.Run();
        }

        public void Dispose()
        {
            _timer?.Dispose(new AutoResetEvent(false));
        }
    }
}
