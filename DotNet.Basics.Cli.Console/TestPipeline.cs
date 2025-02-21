using DotNet.Basics.Pipelines;
using DotNet.Basics.Serilog.Looging;

namespace DotNet.Basics.Cli.Console
{
    public class TestPipeline : Pipeline<TestPipelineArgs>
    {
        private readonly ILoog _log;

        public TestPipeline(ILoog log, IServiceProvider services) : base(services)
        {
            _log = log;
            AddStep<TestPipelineStep>();
            AddStep(nameof(ExitCodeStep), ExitCodeStep);
        }

        public Task<int> ExitCodeStep(TestPipelineArgs args)
        {
            var promptLogger = _log.WithPromptLogger();
            promptLogger.WriteError.Invoke($"Hello World from {"PromptLogger".Highlight()}!");
            return Task.FromResult(0);
        }
    }
}
