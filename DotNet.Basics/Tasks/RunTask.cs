using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public abstract class RunTask : RunTask<TaskOptions>
    {
        protected RunTask(TaskOptions options = null) : base()
        {
        }
    }
    public abstract class RunTask<T> where T : TaskOptions, new()
    {
        protected RunTask(T options = null)
        {
            Options = options ?? new T();
        }
        
        public T Options { get; set; }

        public abstract void Run();
        public abstract Task RunAsync(CancellationToken ct = default(CancellationToken));
    }
}
