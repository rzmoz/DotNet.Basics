using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public static class Repeat
    {
        public static RepeaterTask TaskAsync(Func<Task> asyncAction)
        {
            return new RepeaterTask(asyncAction);
        }

        public static RepeaterTask Task(Action action)
        {
            return new RepeaterTask(action);
        }

        public static RepeaterTask TaskOnce(Action action)
        {
            var onceOnlyAction = new OnceOnlyAction(action);
            return new RepeaterTask((Action)onceOnlyAction.Invoke);
        }
    }
}
