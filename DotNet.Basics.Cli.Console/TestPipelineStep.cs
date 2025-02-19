using DotNet.Basics.Pipelines;
using DotNet.Basics.Serilog.Looging;

namespace DotNet.Basics.Cli.Console
{
    public class TestPipelineStep(ILoog log) : PipelineStep<TestPipelineArgs>
    {
        protected override Task<int> RunImpAsync(TestPipelineArgs args)
        {
            log.Debug(args.OtherCode.ToString());
            log.Debug($"{nameof(args.MyBool)}:{args.MyBool}");

            log.Verbose($"{nameof(log.Verbose)} {nameof(log.Verbose).Highlight()} lalalalalalala");
            log.Debug($"{nameof(log.Debug)} {nameof(log.Debug).Highlight()} lalalalalalala");
            log.Info($"{nameof(log.Info)} {nameof(log.Info).Highlight()} lalalalalalala");
            log.Success($"{nameof(log.Success)}   {nameof(log.Success).Highlight()} lalalalalalala");
            log.Warning($"{nameof(log.Warning)} {nameof(log.Warning).Highlight()} lalalalalalala");
            log.Error($"{nameof(log.Error)} {nameof(log.Error).Highlight()} lalalalalalala");
            log.Fatal($"{nameof(log.Fatal)} {nameof(log.Fatal).Highlight()} lalalalalalala");
            return Task.FromResult(args.ExitCode);
        }
    }
}
