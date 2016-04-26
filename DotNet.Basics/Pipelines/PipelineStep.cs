using System;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Pipelines
{
    public abstract class PipelineStep<T> where T : EventArgs, new()
    {
        protected PipelineStep()
        {
            DisplayName = GetType().Name;
        }

        public abstract Task RunAsync(T args, ILogger logger);

        public virtual void Init()
        {
        }

        public string DisplayName { get; protected set; }

        internal IocContainer Container { get; set; }
    }
}
