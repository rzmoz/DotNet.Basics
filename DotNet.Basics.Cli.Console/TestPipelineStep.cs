using DotNet.Basics.Diagnostics;
using DotNet.Basics.Pipelines;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli.Console
{
    public class TestPipelineStep(ILogger log) : PipelineStep<TestPipelineArgs>
    {
        protected override Task<int> RunImpAsync(TestPipelineArgs args)
        {
            log.Debug(args.OtherCode.ToString());
            log.Debug($"{nameof(args.MyBool)}:{args.MyBool}");

            log.Trace($"Trace {"Trace".Highlight()} lalalalalalala");
            log.Debug($"Debug {"Debug".Highlight()} lalalalalalala");
            log.Info($"Info {"Info".Highlight()} lalalalalalala");
            log.Success($"Success {"Success".Highlight()} lalalalalalala");
            log.Warn($"Warn {"Warn".Highlight()} lalalalalalala");
            log.Error($"Error {"Error".Highlight()} lalalalalalala");
            log.Critical($"Critical {"Critical".Highlight()} lalalalalalala");
            return Task.FromResult(args.ExitCode);
        }
    }
}
