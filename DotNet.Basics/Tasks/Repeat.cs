using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public static class Repeat
    {
        public static RepeaterAsyncTask TaskAsync(Func<Task> asyncAction)
        {
            return new RepeaterAsyncTask(asyncAction.Invoke);
        }

        public static RepeaterSyncTask Task(Action action)
        {
            return new RepeaterSyncTask(action);
        }

        public static RepeaterSyncTask TaskOnce(Action action)
        {
            var onceOnlyAction = new OnceOnlyAction(action);
            return new RepeaterSyncTask(onceOnlyAction.Invoke);
        }
    }
}
