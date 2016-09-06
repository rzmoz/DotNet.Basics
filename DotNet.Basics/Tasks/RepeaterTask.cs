using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTask : ManagedTask
    {
        public RepeaterTask(Action task) : base(task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Action task) : base(id, task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(Func<CancellationToken, Task> task) : base(task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Func<CancellationToken, Task> task) : base(id, task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(Action syncTask, Func<CancellationToken, Task> asyncTask) : base(syncTask, asyncTask)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Action syncTask, Func<CancellationToken, Task> asyncTask) : base(id, syncTask, asyncTask)
        {
            Options = new RepeatOptions();
        }

        internal RepeatOptions Options { get; }
    }
}
