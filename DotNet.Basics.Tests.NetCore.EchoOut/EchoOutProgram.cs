using System;
using System.Linq;
using DotNet.Basics.Cli;
using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;


namespace DotNet.Basics.Tests.NetCore.EchoOut
{
    public class EchoOutProgram
    {
        static int Main(string[] args)
        {
            args.PauseIfDebug();

            var cliHost = new CliHostBuilder(args)
                .Build();
            
            var range = Enumerable.Range(0, 100);
            range.ForEachParallel(i =>
            {
                var level = (LogLevel)int.Parse((i % 6).ToString());
                cliHost.Log.Write(level, $"{Guid.NewGuid()} {"highlight me".Highlight()} end of string");
            });
            
            try
            {
                return int.Parse(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -100000;
            }
        }
    }
}
