using DotNet.Basics.Pipelines;
using DotNet.Basics.Serilog.Looging;

namespace DotNet.Basics.Cli.Console
{
    public class TestPipeline : Pipeline<TestPipelineArgs>
    {
        private readonly ILoog _log;

        public TestPipeline(ILoog log)
        {
            _log = log;
            AddStep(nameof(Looging), Looging);
            AddStep(nameof(ExitCodeStep), ExitCodeStep);
        }

        public Task<int> Looging(TestPipelineArgs args)
        {
            _log.Debug(args.OtherCode.ToString());
            /*
            _log.Verbose($"{nameof(_log.Verbose)} {nameof(_log.Verbose).Highlight()} lalalalalalala");
            _log.Debug($"{nameof(_log.Debug)} {nameof(_log.Debug).Highlight()} lalalalalalala");
            _log.Info($"{nameof(_log.Info)} {nameof(_log.Info).Highlight()} lalalalalalala");
            _log.Success($"{nameof(_log.Success)} {nameof(_log.Success).Highlight()} lalalalalalala");
            _log.Warning($"{nameof(_log.Warning)} {nameof(_log.Warning).Highlight()} lalalalalalala");
            _log.Error($"{nameof(_log.Error)} {nameof(_log.Error).Highlight()} lalalalalalala");
            _log.Fatal($"{nameof(_log.Fatal)} {nameof(_log.Fatal).Highlight()} lalalalalalala");*/
            return Task.FromResult(args.ExitCode);
        }
        public Task<int> ExitCodeStep(TestPipelineArgs args)
        {
            return Task.FromResult(0);
        }
    }
}
