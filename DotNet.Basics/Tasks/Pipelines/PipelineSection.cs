using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Pipelines
{
    public abstract class PipelineSection<T> : ManagedTask<T> where T : new()
    {
        protected PipelineSection()
            : this(null)
        {
        }
        protected PipelineSection(string name) : base((args, ct) => { })
        {
            Name = ResolveName(name);
        }

        protected override Task InnerRunAsync(T args, CancellationToken ct)
        {
            return RunImpAsync(args, ct);
        }

        protected abstract Task RunImpAsync(T args, CancellationToken ct);

        private string ResolveName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                var type = GetType();
                if (type == typeof(PipelineStep<T>))
                    return PipelineTaskTypes.Step;
                if (type == typeof(PipelineBlock<T>))
                    return PipelineTaskTypes.Block;
                if (type == typeof(Pipeline<T>))
                    return PipelineTaskTypes.Pipeline;

                return null;
            }
            return name;
        }
    }
}
