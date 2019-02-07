using System;
using Serilog;

namespace DotNet.Basics.Tests.NetCore.EchoOut
{
    public class EchoOutProgram
    {
        static int Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            foreach (var arg in args)
                log.Information(arg);

            try
            {
                return int.Parse(args[0]);
            }
            catch (Exception e)
            {
                log.Error(e, e.ToString());
                return -100000;
            }
        }
    }
}
