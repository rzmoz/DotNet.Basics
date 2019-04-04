using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class GenericThatTakesAnotherConcreteClassAsArgStep<T> : PipelineStep<T>
    {
        private ClassThatTakesAnAbstractClassAsCtorParam _argStepDependsOn;

        public GenericThatTakesAnotherConcreteClassAsArgStep(ClassThatTakesAnAbstractClassAsCtorParam argStepDependsOn)
        {
            _argStepDependsOn = argStepDependsOn;
        }

        protected override Task RunImpAsync(T args, LogDispatcher log, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}
