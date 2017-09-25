using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public class OnceOnlyTask
    {
        private Action _syncTask;
        private Func<Task> _asyncTask;

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

        public OnceOnlyTask(Func<Task> asyncTask)
        {
            _asyncTask = async () =>
            {
                try
                {
                    await asyncTask().ConfigureAwait(false);
                }
                finally
                {
                    _asyncTask = () => Task.FromResult("");
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
