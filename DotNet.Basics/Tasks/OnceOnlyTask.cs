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

        public OnceOnlyTask(string id, Action task) : base(id, (Action)OnceOnlySyncTask.Create(task).Run)
        {
        }

        public OnceOnlyTask(Func<Task> task) : this(string.Empty, OnceOnlyAsyncTask.Create(task).RunAsync)
        {
        }

        public OnceOnlyTask(string id, Func<Task> task) : base(id, OnceOnlyAsyncTask.Create(task).RunAsync)
        {
        }

        public OnceOnlyTask(Action syncTask, Func<Task> asyncTask) : this(string.Empty, syncTask, asyncTask)
        {
        }

        public OnceOnlyTask(string id, Action syncTask, Func<Task> asyncTask) : base(id, OnceOnlySyncTask.Create(syncTask).Run, OnceOnlyAsyncTask.Create(asyncTask).RunAsync)
        {
        }

        private class OnceOnlySyncTask
        {
            private OnceOnlySyncTask(Action task)
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

            public static OnceOnlySyncTask Create(Action task)
            {
                return new OnceOnlySyncTask(task);
            }
        }

        private class OnceOnlyAsyncTask
        {
            private Func<Task> _task;

            private OnceOnlyAsyncTask(Func<Task> task)
            {
                _task = async () =>
                {
                    _task = () => Task.CompletedTask;
                    await task.Invoke().ConfigureAwait(false);
                };
            }

            public async Task RunAsync()
            {
                await _task.Invoke().ConfigureAwait(false);
            }

            public static OnceOnlyAsyncTask Create(Func<Task> task)
            {
                return new OnceOnlyAsyncTask(task);
            }
        }
    }
}
