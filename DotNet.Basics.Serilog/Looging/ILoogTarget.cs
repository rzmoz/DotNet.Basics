using System;

namespace DotNet.Basics.Serilog.Diagnostics
{
    public interface ILoogTarget
    {
        Action<LoogLevel, string, Exception?> LogTarget { get; }
        Action<LoogLevel, string, string, TimeSpan> TimingTarget { get; }
    }
}
