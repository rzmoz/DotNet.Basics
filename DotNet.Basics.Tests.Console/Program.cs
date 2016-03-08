using System;
using System.Linq;


namespace DotNet.Basics.Tests.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Executing DotNet.Basics.Tests.Console...");

            Environment.ExitCode = Int32.Parse(args.Single());
            Environment.Exit(Environment.ExitCode);
        }
    }
}
