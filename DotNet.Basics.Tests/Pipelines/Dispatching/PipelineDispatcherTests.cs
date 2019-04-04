using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines.Dispatching;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Pipelines.Dispatching
{
    public class PipelineDispatcherTests
    {
        [Fact]
        public async Task RunAsync_ArgsHasPipeInValue_ArgsIsParsed()
        {
            var argPart1 = "Hello";
            var argPart2 = "World!";
            string messageReceived = null;

            var setByArgsValue = $"{argPart1}|{argPart2}";
            var args = $"{{'SetByArgs':'{setByArgsValue}'}}";

            var dispatcher = new PipelineDispatcherBuilder().InNamespace(typeof(PipelineDispatcherTests).Namespace).Build(typeof(DispatchTestPipeline));
            dispatcher.MessageLogged += (lvl, msg, e) => { messageReceived = msg; };
            //act
            var argsOut = (DispatchTestArgs)await dispatcher.RunAsync("Dispatching.DispatchTest", args).ConfigureAwait(false);

            //Assert
            messageReceived.Should().Be($"{nameof(DispatchTestPipeline)} / MyStep / Hello World!");
            argsOut.SetByArgs.Should().Be(setByArgsValue);
            argsOut.SplitArgs.First().Should().Be(argPart1);
            argsOut.SplitArgs.Last().Should().Be(argPart2);
        }

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
