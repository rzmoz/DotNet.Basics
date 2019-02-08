using DotNet.Basics.Tests.Pipelines.PipelineHelpers;

namespace DotNet.Basics.Tests.Pipelines
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
