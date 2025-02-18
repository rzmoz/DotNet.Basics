using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class GenericThatTakesAnotherConcreteClassAsArgStep<T>(
        ClassThatTakesAnAbstractClassAsCtorParam argStepDependsOn) : PipelineStep<T>
    {
        private ClassThatTakesAnAbstractClassAsCtorParam _argStepDependsOn = argStepDependsOn;

        protected override Task<int> RunImpAsync(T args, CancellationToken ct)
        {
            return Task.FromResult(0);
        }
    }
}
