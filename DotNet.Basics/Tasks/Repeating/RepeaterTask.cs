using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public class RepeaterTask : ManagedTask<EventArgs>
    {
        public RepeaterTask(Func<Task<int>> task) : base(task)
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
