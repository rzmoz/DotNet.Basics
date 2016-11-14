using DotNet.Basics.Rest;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class ClassThatTakesAnAbstractClassAsCtorParam
    {
        private readonly IRestClient _client;

        public ClassThatTakesAnAbstractClassAsCtorParam(IRestClient client)
        {
            _client = client;
        }
    }
}
