using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Pipelines
{
    public abstract class PipelineStep<T> where T : EventArgs, new()
    {
        protected PipelineStep()
        {
            DisplayName = GetType().Name;
        }

        public abstract Task RunAsync(T args, CancellationToken ct);

        public virtual void Init()
        {
        }

        public string DisplayName { get; protected set; }

        internal SimpleContainer Container { get; set; }
    }
}
