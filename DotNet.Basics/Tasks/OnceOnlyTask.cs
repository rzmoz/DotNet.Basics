using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class OnceOnlyTask : ManagedTask
    {
        private Action<string> _syncTask;
        private Func<string, Task> _asyncTask;

        public OnceOnlyTask(ManagedTask task) : base(task)
        {
            _syncTask = rid =>
            {
                _syncTask = rd => { };
                base.Run(rid);
            };
            _asyncTask = async runId =>
             {
                 _asyncTask = rid => Task.CompletedTask;
                 await base.RunAsync(runId).ConfigureAwait(false);
             };
        }

        public OnceOnlyTask(string id, Action<string> syncTask, Func<string, TaskEndedReason> preconditionsMet = null) : base(id, syncTask, preconditionsMet)
        {
            _syncTask = rid =>
            {
                _syncTask = rd => { };
                base.Run(rid);
            };
            _asyncTask = async runId =>
            {
                _asyncTask = rid => Task.CompletedTask;
                await base.RunAsync(runId).ConfigureAwait(false);
            };
        }

        public OnceOnlyTask(string id, Func<string, Task> asyncTask, Func<string, TaskEndedReason> preconditionsMet = null) : base(id, asyncTask, preconditionsMet)
        {
            _syncTask = rid =>
            {
                _syncTask = rd => { };
                base.Run(rid);
            };
            _asyncTask = async runId =>
            {
                _asyncTask = rid => Task.CompletedTask;
                await base.RunAsync(runId).ConfigureAwait(false);
            };
        }

        internal override void Run(string runId)
        {
            _syncTask(runId);
        }

        internal override async Task RunAsync(string runId)
        {
            await _asyncTask(runId).ConfigureAwait(false);
        }
    }
}
