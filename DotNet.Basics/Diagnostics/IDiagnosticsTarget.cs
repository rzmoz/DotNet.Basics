using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public interface IDiagnosticsTarget
    {
        Action<LogLevel, string, Exception> LogTarget { get; }
        Action<string, string, TimeSpan> TimingTarget { get; }
    }
}
