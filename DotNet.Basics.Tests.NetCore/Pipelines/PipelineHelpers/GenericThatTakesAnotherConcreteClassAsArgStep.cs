using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
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
