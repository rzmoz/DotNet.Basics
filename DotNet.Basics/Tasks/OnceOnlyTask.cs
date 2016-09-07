using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class OnceOnlyTask : ManagedTask
    {
        public OnceOnlyTask(ManagedTask task) : base(task)
        {
        }

        public OnceOnlyTask(Action<string> task) : this(string.Empty, task)
        {
        }

        public OnceOnlyTask(string id, Action<string> task) : base(id, (Action<string>)OnceOnlySyncTask.Create(task).Run)
        {
        }

        public OnceOnlyTask(Func<string, Task> task) : this(string.Empty, OnceOnlyAsyncTask.Create(task).RunAsync)
        {
        }

        public OnceOnlyTask(string id, Func<string, Task> task) : base(id, OnceOnlyAsyncTask.Create(task).RunAsync)
        {
        }
        
        private class OnceOnlySyncTask
        {
            private OnceOnlySyncTask(Action<string> task)
            {
                _task = runId =>
                {
                    _task = rid => { };
                    task.Invoke(runId);
                };
            }

            private Action<string> _task;

            public void Run(string runId)
            {
                _task.Invoke(runId);
            }

            public static OnceOnlySyncTask Create(Action<string> task)
            {
                return new OnceOnlySyncTask(task);
            }
        }

        private class OnceOnlyAsyncTask
        {
            private Func<string, Task> _task;

            private OnceOnlyAsyncTask(Func<string, Task> task)
            {
                _task = async runId =>
                {
                    _task = rid => Task.CompletedTask;
                    await task.Invoke(runId).ConfigureAwait(false);
                };
            }

            public async Task RunAsync(string runId)
            {
                await _task.Invoke(runId).ConfigureAwait(false);
            }

            public static OnceOnlyAsyncTask Create(Func<string, Task> task)
            {
                return new OnceOnlyAsyncTask(task);
            }
        }
    }
}
