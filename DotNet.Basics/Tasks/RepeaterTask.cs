using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTask
    {
        public RepeaterTask(Action action, RepeatOptions options = null)
        {
            Action = () => Task.Run(action);
            Options = options ?? new RepeatOptions();
        }
        public RepeaterTask(Func<Task> action, RepeatOptions options = null)
        {
            Action = action;
            Options = options ?? new RepeatOptions();
        }

        internal Func<Task> Action { get; }

        public RepeaterTask WithOptions(Action<RepeatOptions> setOptions)
        {
            setOptions?.Invoke(Options);
            return new RepeaterTask(Action, Options);
        }

        internal RepeatOptions Options { get; }
    }
}