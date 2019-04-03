using System;
using Serilog;

namespace DotNet.Basics.Tests.NetCore.EchoOut
{
    public class EchoOutProgram
    {
        static int Main(string[] args)
        {
            foreach (var arg in args)
                Console.WriteLine(arg);

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
