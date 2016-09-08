using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Pipelines
{
    public class EagerBindStep<T> : PipelineSection<T> where T : EventArgs, new()
    {
        private readonly Func<T, CancellationToken, Task> _step;

        public EagerBindStep(string name, Func<T, CancellationToken, Task> step) : base(name)
        {
            if (step == null) throw new ArgumentNullException(nameof(step));
            _step = step;
        }

        public override SectionType SectionType => SectionType.Step;

        protected override async Task InnerRunAsync(T args, CancellationToken ct)
        {
            await _step(args, ct).ConfigureAwait(false);
        }
    }
}
