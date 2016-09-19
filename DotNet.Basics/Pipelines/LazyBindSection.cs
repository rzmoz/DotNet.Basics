using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Pipelines
{
    public class LazyBindSection<T, TSection> : PipelineSection<T>
        where T : EventArgs, new()
        where TSection : PipelineSection<T>
    {
        private readonly Func<TSection> _getSection;

        public LazyBindSection(string name, Func<TSection> getSection) : base(name)
        {
            if (getSection == null) throw new ArgumentNullException(nameof(getSection));
            _getSection = getSection;
        }

        public override SectionType SectionType
        {
            get
            {
                var step = _getSection();
                return step.SectionType;
            }
        }

        protected override void Init()
        {
            var step = _getSection();
            Name = step.Name;
        }

        protected override async Task InnerRunAsync(T args, CancellationToken ct)
        {
            var step = _getSection();
            Name = step.Name;
            await step.RunAsync(args, ct).ConfigureAwait(false);
        }
    }
}
