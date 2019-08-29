using System;
using System.Threading.Tasks;
using DotNet.Basics.Cli;

namespace DotNet.Basics.ExeTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var cliHost = new CliHostBuilder(args)
                .Build();


            cliHost.Log.Metric("MyMetric", 2.54);

            return 0;
        }
    }
}
