using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Pipelines
{
    public class LazyBindStep<T, TStep> : PipelineSection<T>
        where T : EventArgs, new()
        where TStep : PipelineSection<T>
    {
        private readonly Func<TStep> _getStep;

        public LazyBindStep(string name, Func<TStep> getStep) : base(name)
        {
            if (getStep == null) throw new ArgumentNullException(nameof(getStep));
            _getStep = getStep;
        }

        public override SectionType SectionType=>SectionType.Step;

        public override void Init()
        {
            var step = _getStep();
            Name = step.Name;
        }

        protected override async Task InnerRunAsync(T args, CancellationToken ct)
        {
            var step = _getStep();
            Name = step.Name;
            await step.RunAsync(args, ct).ConfigureAwait(false);
        }
    }
}
