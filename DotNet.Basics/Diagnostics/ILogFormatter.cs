using System.IO;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public interface ILogFormatter
    {
        void Format(LogLevel level, string? message, TextWriter output);
    }
}
