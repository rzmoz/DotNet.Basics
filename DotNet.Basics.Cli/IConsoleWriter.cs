using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli
{
    public interface IConsoleWriter
    {
        void Write(LogLevel level, string message, Exception e = null);
    }
}
