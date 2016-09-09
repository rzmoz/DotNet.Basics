using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public static class Repeat
    {
        private static readonly ManagedTaskFactory _taskFactory = new ManagedTaskFactory();

        public static RepeaterTask Task(Func<Task> task)
        {
            return _taskFactory.Create<RepeaterTask>(async rid => await task());
        }

        public static RepeaterTask TaskOnce(Func<Task> task)
        {
            var onceOnlyTask = _taskFactory.Create<OnceOnlyTask>(async rid => await task());
            return _taskFactory.Create<RepeaterTask>(onceOnlyTask);
        }

        public static RepeaterTask Task(Action task)
        {
            return _taskFactory.Create<RepeaterTask>(rid => task());
        }

        public static RepeaterTask TaskOnce(Action task)
        {
            var onceOnlyTask = _taskFactory.Create<OnceOnlyTask>(rid => task());
            return _taskFactory.Create<RepeaterTask>(onceOnlyTask);
        }
    }
}
