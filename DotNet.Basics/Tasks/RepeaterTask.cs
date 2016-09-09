using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTask : ManagedTask
    {
        public RepeaterTask(ManagedTask task) : base(task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Action<string> syncTask, Func<string, TaskEndedReason> preconditionsMet = null) : base(id, syncTask, preconditionsMet)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Func<string, Task> asyncTask, Func<string, TaskEndedReason> preconditionsMet = null) : base(id, asyncTask, preconditionsMet)
        {
            Options = new RepeatOptions();
        }

        internal RepeatOptions Options { get; }
    }
}
