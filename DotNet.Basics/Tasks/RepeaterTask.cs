using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTask : ManagedTask<EventArgs>, ITask
    {
        public RepeaterTask(Func<Task> task) : base(task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(Action task) : base(task)
        {
            Options = new RepeatOptions();
        }

        internal RepeatOptions Options { get; }
    }
}
