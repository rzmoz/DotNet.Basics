using Microsoft.Extensions.Logging;
using System;

namespace DotNet.Basics.Cli.Logging
{
    public interface IConsoleLogger
    {
        public void Log(LogLevel level, string message, Exception? e);
    }
}
