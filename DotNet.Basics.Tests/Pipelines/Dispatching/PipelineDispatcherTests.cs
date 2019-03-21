using System.Threading.Tasks;
using DotNet.Basics.Pipelines.Dispatching;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Pipelines.Dispatching
{
    public class PipelineDispatcherTests
    {
        [Fact]
        public async Task RunAsync_Dispatch_PipelineIsDispatched()
        {
            var setByArgsValue = "HelloArgs!";
            var args = $"{{'SetByArgs':'{setByArgsValue}'}}";

            var dispatcher = new PipelineDispatcherBuilder().InNamespace(typeof(PipelineDispatcherTests).Namespace).Build(typeof(DispatchTestPipeline));

            //act
            var argsOut = (DispatchTestArgs)await dispatcher.RunAsync("Dispatching.DispatchTest", args).ConfigureAwait(false);
            
            //Assert
            
            argsOut.SetByArgs.Should().Be(setByArgsValue);
            argsOut.SetByPipeline.Should().Be("Yes");
            
        }
    }
}
