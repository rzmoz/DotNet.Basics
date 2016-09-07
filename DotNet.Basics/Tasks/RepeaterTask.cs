using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTask : ManagedTask
    {
        public RepeaterTask(ManagedTask task) : base(task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(Action<string> task) : base(task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Action<string> task) : base(id, task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(Func<string, Task> task) : base(task)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Func<string, Task> task) : base(id, task)
        {
            Options = new RepeatOptions();
        }
        
        internal RepeatOptions Options { get; }
    }
}
