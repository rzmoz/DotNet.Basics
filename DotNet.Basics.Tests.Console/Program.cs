using System;
using System.Linq;
using DotNet.Basics.ConsoleApp;


namespace DotNet.Basics.Tests.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmd = new CmdLine()
                .Register("RequiredTrueAllowEmptyTrue", Required.Yes, AllowEmpty.Yes, param => { });

            cmd.Parse(args);
        }
    }
}
