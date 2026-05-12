using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public class OnceOnlyTask
    {
        private Action _syncTask = () => { };
        private Func<CancellationToken, Task<int>> _asyncTask = _ => Task.FromResult(-1);

        public OnceOnlyTask(Action task)
        {
            _syncTask = () =>
            {
                try
                {
                    task();
                }
                finally
                {
                    _syncTask = () => { };
                }
            };
        }

        public OnceOnlyTask(Func<Task<int>> asyncTask)
        {
            _asyncTask = async ct =>
            {
                try
                {
                    ct.ThrowIfCancellationRequested();
                    return await asyncTask().ConfigureAwait(false);
                }
                finally
                {
                    _asyncTask = _ => Task.FromResult(0);
                }
            };
        }

        public void RunSync()
        {
            _syncTask();
        }
        public Task<int> RunAsync(CancellationToken cancellationToken = default)
        {
            return _asyncTask(cancellationToken);
        }
    }
}
