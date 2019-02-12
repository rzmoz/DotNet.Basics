using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using Serilog;

namespace DotNet.Basics.ExeTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.ColoredConsole()
                .CreateLogger();
            //Test output from process manually
            foreach (var index in Enumerable.Range(0, 10))
            {
                CmdPrompt.Run("dotnet build Dummy.sln", output => log.Debug("{Index}{Output}", index, output));
                await Task.Delay(500).ConfigureAwait(false);
            }

            return 0;

        }
    }
}
