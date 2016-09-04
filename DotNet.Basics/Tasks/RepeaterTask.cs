using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTask : ManagedTask
    {
        public RepeaterTask(Func<CancellationToken, Task> task, string id = null) : base(task, id)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(Action task, string id = null) : base(task, id)
        {
            Options = new RepeatOptions();
        }

        internal RepeatOptions Options { get; }
    }
}
