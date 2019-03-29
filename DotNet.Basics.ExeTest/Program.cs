using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

namespace DotNet.Basics.ExeTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            //Test output from process manually
            foreach (var index in Enumerable.Range(0, 10))
            {
                CmdPrompt.Run("dotnet build Dummy.sln", output => Console.WriteLine($"{index}{output}"));
                await Task.Delay(500).ConfigureAwait(false);
            }

            return 0;
        }
    }
}
