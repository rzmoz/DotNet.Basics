using System;
using DotNet.Basics.NLog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tests.NetCore.EchoOut
{
    public class EchoOutProgram
    {
        static int Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddNLogging(config => config.AddColoredConsoleTarget());
            var sp = services.BuildServiceProvider();
            var logger = sp.GetService<ILogger>();
            
            logger.LogInformation($"Starting {typeof(EchoOutProgram).Namespace}...");

            foreach (var arg in args)
                logger.LogDebug(arg);

            try
            {
                return int.Parse(args[0]);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(e.ToString());
                return -100000;
            }
        }
    }
}
