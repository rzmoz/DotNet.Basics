using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.Dispatching
{
    public class DispatchTestPipeline : Pipeline<DispatchTestArgs>
    {
        public DispatchTestPipeline()
        {
            AddStep("MyStep", (args, ct) =>
            {
                args.SetByPipeline = "Yes";
                args.SplitArgs = args.SetByArgs.Split('|');
            });
        }
    }
}
