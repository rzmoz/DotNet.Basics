using System;
using System.Text;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public class SystemConsoleWriter : ConsoleWriter
    {
        public override void Write(LogLevel level, string message, Exception e = null)
        {
            var outputBuilder = new StringBuilder();
            outputBuilder.Append($"[{ToOutputString(level)}]");
            outputBuilder.Append(" ");
            outputBuilder.Append($"{message}\r\n{e}");
            var output = outputBuilder.ToString().StripHighlight();

            Console.Write(output);
        }
    }
}
