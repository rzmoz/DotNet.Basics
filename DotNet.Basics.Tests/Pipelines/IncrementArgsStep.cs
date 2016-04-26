using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tests.Pipelines
{
    public class IncrementArgsStep : PipelineStep<EventArgs<int>>
    {
        private readonly ClassThatIncrementArgsDependOn _classThatIncrementArgsDependOn;

        public IncrementArgsStep(ClassThatIncrementArgsDependOn classThatIncrementArgsDependOn)
        {
            _classThatIncrementArgsDependOn = classThatIncrementArgsDependOn;
            DisplayName = "MyIncrementArgsStep";
        }

        public override async Task RunAsync(EventArgs<int> args, ILogger logger)
        {
            await Task.Delay(1.MilliSeconds()).ConfigureAwait(false);//silence compiler warning
            args.Value = _classThatIncrementArgsDependOn.IncrementByOne(args.Value);
            logger.LogInformation($"Value is now: {args.Value}");
        }
    }
}
