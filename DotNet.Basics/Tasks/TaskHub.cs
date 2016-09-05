using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public static class TaskHub
    {
        public static RepeaterTask Repeat(Func<CancellationToken, Task> task)
        {
            return new RepeaterTask(task);
        }

        public static RepeaterTask RepeatOnce(Func<CancellationToken, Task> task)
        {
            var onceOnlyTask = new OnceOnlyTask(task);
            return new RepeaterTask(onceOnlyTask.RunAsync);
        }

        public static RepeaterTask Repeat(Action task)
        {
            return new RepeaterTask(task);
        }

        public static RepeaterTask RepeatOnce(Action task)
        {
            var onceOnlyTask = new OnceOnlyTask(task);
            return new RepeaterTask(onceOnlyTask.Run);
        }
    }
}
