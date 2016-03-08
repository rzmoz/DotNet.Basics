using System;
using DotNet.Basics.ConsoleApp;

namespace DotNet.Basics.Tests.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdLine = new CmdLine();
            cmdLine.RegisterParameter(_exitCodeParameter);
            cmdLine.RegisterParameter(_writeErrorParameter);
            cmdLine.Parse(args);

            int exitCode = 0;

            if (cmdLine[_exitCodeParameter].Exists)
                exitCode = int.Parse(cmdLine[_exitCodeParameter].Value);

            if (cmdLine[_writeErrorParameter].Exists)
                System.Console.Error.Write("Some error happened");

            System.Console.WriteLine("Executing CSharp.Basics.Tests.Console...");

            Environment.ExitCode = exitCode;
            Environment.Exit(exitCode);
        }

        private static readonly CmdLineParameter _exitCodeParameter = new CmdLineParameter("ExitCode", "ExitCode") { Required = false };
        private static readonly CmdLineParameter _writeErrorParameter = new CmdLineParameter("WriteError", "WriteError") { Required = false, AllowEmptyValue = true };
    }
}
