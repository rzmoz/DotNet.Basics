using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTask : ManagedTask
    {
        private static readonly ConcurrentDictionary<string, ConcurrentStack<string>> _runningBackgroundTasks =
            new ConcurrentDictionary<string, ConcurrentStack<string>>();

        public BackgroundTask(ManagedTask task) : base(task)
        {
        }

        public BackgroundTask(Action<string> task) : base(task)
        {
        }

        public BackgroundTask(string id, Action<string> task) : base(id, task)
        {
        }

        public BackgroundTask(Func<string, Task> task) : base(task)
        {
        }

        public BackgroundTask(string id, Func<string, Task> task) : base(id, task)
        {
        }

        public bool IsRunning()
        {
            ConcurrentStack<string> runStack;
            if (_runningBackgroundTasks.TryGetValue(Id, out runStack) == false)
                return false;
            return runStack.Count > 0;
        }

        internal override void Run(string runId)
        {
            //ensure task exists on run stack
            _runningBackgroundTasks.TryAdd(Id, new ConcurrentStack<string>());

            Task.Run(() =>
            {
                var runStack = _runningBackgroundTasks[Id];
                try
                {
                    runStack.Push(runId);
                    base.Run(runId ?? string.Empty);
                }
                finally
                {
                    string poppedRuId;
                    runStack.TryPop(out poppedRuId);
                }
            }, CancellationToken.None);
        }

        internal override Task RunAsync(string runId)
        {
            //ensure task exists on run stack
            _runningBackgroundTasks.TryAdd(Id, new ConcurrentStack<string>());

            Task.Run(async () =>
            {
                var runStack = _runningBackgroundTasks[Id];
                try
                {
                    runStack.Push(runId);
                    await base.RunAsync(runId ?? string.Empty).ConfigureAwait(false);
                }
                finally
                {
                    string poppedRuId;
                    runStack.TryPop(out poppedRuId);
                }
            }, CancellationToken.None);
            return Task.CompletedTask;
        }
    }
}
