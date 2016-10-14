using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class OnceOnlyAction
    {
        private Action _syncTask;
        private Func<Task> _asyncTask;

        public OnceOnlyAction(Action task)
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

        public OnceOnlyAction(Func<Task> asyncTask)
        {
            _asyncTask = async () =>
            {
                try
                {
                    await asyncTask().ConfigureAwait(false);
                }
                finally
                {
                    _asyncTask = () => Task.CompletedTask;
                }
            };
        }

        public void RunSync()
        {
            _syncTask();
        }
        public async Task RunAsync()
        {
            await _asyncTask().ConfigureAwait(false);
        }
    }
}
