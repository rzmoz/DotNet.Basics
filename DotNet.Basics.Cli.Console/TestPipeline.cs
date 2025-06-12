using DotNet.Basics.Pipelines;
using Microsoft.Extensions.Logging;
using LoggerExtensions = DotNet.Basics.Diagnostics.LoggerExtensions;

namespace DotNet.Basics.Cli.Console
{
    public class TestPipeline : Pipeline<TestPipelineArgs>
    {
        private readonly ILogger _log;

        public TestPipeline(ILogger log, IServiceProvider services) : base(services)
        {
            _log = log;
            AddStep<TestPipelineStep>();
            AddStep(nameof(ExitCodeStep), ExitCodeStep);
        }

        public Task<int> ExitCodeStep(TestPipelineArgs args)
        {
            LoggerExtensions.Info(_log, "Hello World from {}!", nameof(TestPipeline));
            return Task.FromResult(0);
        }
    }
}
