using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public static class Repeat
    {
        public static RepeaterTask Task(Func<Task> task)
        {
            return new RepeaterTask(async rid => await task());
        }

        public static RepeaterTask TaskOnce(Func<Task> task)
        {
            var onceOnlyTask = new OnceOnlyTask(async rid => await task());
            return new RepeaterTask(onceOnlyTask.RunAsync);
        }

        public static RepeaterTask Task(Action task)
        {
            return new RepeaterTask(rid => task());
        }

        public static RepeaterTask TaskOnce(Action task)
        {
            var onceOnlyTask = new OnceOnlyTask(rid => task());
            return new RepeaterTask((Action<string>)onceOnlyTask.Run);
        }
    }
}
