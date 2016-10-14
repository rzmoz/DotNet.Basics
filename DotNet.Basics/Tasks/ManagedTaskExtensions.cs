using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public static class ManagedTaskExtensions
    {
        public static Action ToOnceOnly(this Action task)
        {
            return new OnceOnlyTask(task).RunSync;
        }
        public static Func<Task> ToOnceOnly(this Func<Task> task)
        {
            return new OnceOnlyTask(task).RunAsync;
        }
    }
}
