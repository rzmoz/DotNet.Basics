using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class OnceOnlyTask : ManagedTask
    {
        public OnceOnlyTask(Action task, string id = null) : base(new OnceOnlySyncTask(task).Run, id)
        {
        }

        public OnceOnlyTask(Func<CancellationToken, Task> task, string id = null) : base(new OnceOnlyAsyncTask(task).RunAsync, id)
        {
        }

        public OnceOnlyTask(Action syncTask, Func<CancellationToken, Task> asyncTask, string id) : base(new OnceOnlySyncTask(syncTask).Run, new OnceOnlyAsyncTask(asyncTask).RunAsync, id)
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
