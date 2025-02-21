using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public class OnceOnlyTask
    {
        private Action _syncTask = () => { };
        private Func<Task<int>> _asyncTask = () => Task.FromResult(-1);

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
            _asyncTask = async () =>
            {
                try
                {
                    return await asyncTask().ConfigureAwait(false);
                }
                finally
                {
                    _asyncTask = () => Task.FromResult(0);
                }
            };
        }

        public void RunSync()
        {
            _syncTask();
        }
        public Task<int> RunAsync()
        {
            return _asyncTask();
        }
    }
}