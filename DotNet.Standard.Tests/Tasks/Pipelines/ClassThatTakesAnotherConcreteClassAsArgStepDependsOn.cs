using DotNet.Standard.Tests.Tasks.Pipelines.PipelineHelpers;

namespace DotNet.Standard.Tests.Tasks.Pipelines
{
    public class ClassThatTakesAnotherConcreteClassAsArgStepDependsOn
    {
        private DescendantArgs _args;

        public ClassThatTakesAnotherConcreteClassAsArgStepDependsOn(DescendantArgs args)
        {
            _args = args;
        }
    }
}
