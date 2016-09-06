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

        public RepeaterTask(Func<Task> task) : base(task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Func<Task> task) : base(id, task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(Action syncTask, Func<Task> asyncTask) : base(syncTask, asyncTask)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Action syncTask, Func<Task> asyncTask) : base(id, syncTask, asyncTask)
        {
            Options = new RepeatOptions();
        }

        internal RepeatOptions Options { get; }
    }
}
