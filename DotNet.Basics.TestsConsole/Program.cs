using System;
using DotNet.Basics.NLog;
using NLog;
using NLog.Targets;

namespace DotNet.Basics.TestsConsole
{
    class Program
    {
        static int Main(string[] args)
        {
            using (var nlc = new NLogConfigurator())
            {
                nlc.AddTarget(new ColoredConsoleTarget { Layout = "${message}" }.WithOutputColors());
            }
            var logger = LogManager.GetCurrentClassLogger();
            logger.Debug("Debug");
            logger.Trace("Trace");
            logger.Info("Info");
            logger.Warn("Warn");
            logger.Error("Error");
            logger.Fatal("Fatal");
            Console.ReadKey();
            return int.Parse(args[0]);
        }
    }
}
