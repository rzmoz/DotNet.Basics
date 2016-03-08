using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tests.Pipelines.ContainerModeHelpers
{
    public class DebugModeStep : ModeStep
    {
        public override async Task RunAsync(ContainerModeArgs args, IDiagnostics logger)
        {
            await Task.Delay(1.MilliSeconds()).ConfigureAwait(false);//silence compiler warning
            args.Mode = IocMode.Debug;
            logger.Log($"Mode set to {args.Mode.ToName()}!");
        }
    }
}
