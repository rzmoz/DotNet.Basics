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

        public RepeaterTask(string id, Action<string> syncTask) : base(id, syncTask)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Func<string, Task> asyncTask) : base(id, asyncTask)
        {
            Options = new RepeatOptions();
        }

        internal RepeatOptions Options { get; }
    }
}
