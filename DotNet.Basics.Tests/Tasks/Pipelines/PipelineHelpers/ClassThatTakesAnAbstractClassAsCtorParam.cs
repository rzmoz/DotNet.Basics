
namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class ClassThatTakesAnAbstractClassAsCtorParam
    {
        private readonly IAbstract _abstract;

        public ClassThatTakesAnAbstractClassAsCtorParam(IAbstract @abstract)
        {
            _abstract = @abstract;
        }
    }
}
