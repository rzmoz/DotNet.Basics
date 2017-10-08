using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public static class Repeat
    {
        public static RepeaterTask Task(Func<Task> task)
        {
            return new RepeaterTask(task);
        }

        public static RepeaterTask TaskOnce(Func<Task> task)
        {
            return new RepeaterTask(task.ToOnceOnly());
        }

        public static RepeaterTask Task(Action task)
        {
            return new RepeaterTask(task);
        }

        public static RepeaterTask TaskOnce(Action task)
        {
            return new RepeaterTask(task.ToOnceOnly());
        }
    }
}
