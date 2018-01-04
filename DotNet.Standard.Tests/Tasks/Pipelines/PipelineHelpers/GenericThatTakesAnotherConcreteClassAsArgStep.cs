using System.Threading;
using System.Threading.Tasks;
using DotNet.Standard.Tasks;
using DotNet.Standard.Tasks.Pipelines;

namespace DotNet.Standard.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class GenericThatTakesAnotherConcreteClassAsArgStep<T> : PipelineStep<T> where T : class, new()
    {
        private ClassThatTakesAnAbstractClassAsCtorParam _argStepDependsOn;

        public GenericThatTakesAnotherConcreteClassAsArgStep(ClassThatTakesAnAbstractClassAsCtorParam argStepDependsOn)
        {
            _argStepDependsOn = argStepDependsOn;
        }

        protected override Task RunImpAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            return Task.FromResult("");
        }
    }
}
