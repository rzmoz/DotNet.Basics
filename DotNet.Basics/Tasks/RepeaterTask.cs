using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTask : ManagedTask
    {
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

        public RepeaterTask(Action<string> syncTask, Func<string, Task> asyncTask) : base(syncTask, asyncTask)
        {
            Options = new RepeatOptions();
        }

        public RepeaterTask(string id, Action<string> syncTask, Func<string, Task> asyncTask) : base(id, syncTask, asyncTask)
        {
            Options = new RepeatOptions();
        }

        internal RepeatOptions Options { get; }
    }
}
