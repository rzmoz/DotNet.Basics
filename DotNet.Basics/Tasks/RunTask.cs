using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public abstract class RunTask : RunTask<TaskOptions>
    {
        protected RunTask(TaskOptions options = null, string id = null) : base(options, id)
        {
        }
    }
    public abstract class RunTask<T> where T : TaskOptions, new()
    {
        protected RunTask(T options = null, string id = null)
        {
            Id = id ?? string.Empty;
            InstanceId = Guid.NewGuid().ToString("N");
            Options = options ?? new T();
        }

        public string Id { get; }
        public string InstanceId { get; }

        public T Options { get; set; }

        public abstract void Run();
        public abstract Task RunAsync(CancellationToken ct = default(CancellationToken));
    }
}
