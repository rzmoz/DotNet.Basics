using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class OnceOnlyTask : ManagedTask
    {
        public OnceOnlyTask(Action task) : this(string.Empty, task)
        {
        }

        public OnceOnlyTask(string id, Action task) : base(id, new OnceOnlySyncTask(task).Run)
        {
        }

        public OnceOnlyTask(Func<CancellationToken, Task> task) : this(string.Empty, new OnceOnlyAsyncTask(task).RunAsync)
        {
        }

        public OnceOnlyTask(string id, Func<CancellationToken, Task> task) : base(id, task)
        {
        }

        public OnceOnlyTask(Action syncTask, Func<CancellationToken, Task> asyncTask) : this(string.Empty, syncTask, asyncTask)
        {
        }

        public OnceOnlyTask(string id, Action syncTask, Func<CancellationToken, Task> asyncTask) : base(id, new OnceOnlySyncTask(syncTask).Run, new OnceOnlyAsyncTask(asyncTask).RunAsync)
        {
        }

        private class OnceOnlySyncTask
        {
            public OnceOnlySyncTask(Action task)
            {
                _task = () =>
                {
                    _task = () => { };
                    task.Invoke();
                };
            }

            private Action _task;

            public void Run()
            {
                _task.Invoke();
            }
        }

        private class OnceOnlyAsyncTask
        {
            private Func<CancellationToken, Task> _task;

            public OnceOnlyAsyncTask(Func<CancellationToken, Task> task)
            {
                _task = async (ct) =>
                {
                    _task = ctx => Task.CompletedTask;
                    await task.Invoke(ct).ConfigureAwait(false);
                };
            }

            public async Task RunAsync(CancellationToken ct = default(CancellationToken))
            {
                await _task.Invoke(ct).ConfigureAwait(false);
            }
        }
    }
}
