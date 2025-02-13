using System;

namespace DotNet.Basics.Diagnostics
{
    public interface ILogTarget
    {
        Action<LogLevel, string, Exception> LogTarget { get; }
        Action<LogLevel, string, string, TimeSpan> TimingTarget { get; }
    }
}
