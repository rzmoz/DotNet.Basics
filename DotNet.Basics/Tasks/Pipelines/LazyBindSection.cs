using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class LazyBindSection<T, TSection> : PipelineSection<T>
        where T : class, new()
        where TSection : PipelineSection<T>
    {
        private readonly Func<TSection> _getSection;

        public LazyBindSection(string name, Func<TSection> getSection) : base(name)
        {
            if (getSection == null) throw new ArgumentNullException(nameof(getSection));
            _getSection = getSection;
        }

        public override string TaskType
        {
            get
            {
                var step = _getSection();
                return step.TaskType;
            }
        }

        protected override async Task RunImpAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            var step = _getSection();
            Name = step.Name;
            var result = await step.RunAsync(args, ct).ConfigureAwait(false);
            issues.Add(result.Issues);
        }
    }
}
