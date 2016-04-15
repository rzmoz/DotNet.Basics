using System;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Pipelines
{
    public abstract class TaskStep<T> where T : EventArgs, new()
    {
        protected TaskStep()
        {
            DisplayName = GetType().Name;
        }

        public abstract Task RunAsync(T args, IDiagnostics logger);

        public virtual void Init()
        {
        }

        public string DisplayName { get; protected set; }

        internal IIocContainer Container { get; set; }
    }
}
