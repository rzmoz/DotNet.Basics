using System;
using DotNet.Basics.Serilog.Looging;

namespace DotNet.Basics.Serilog.Looging
{
    public interface ILoogTarget
    {
        Action<LoogLevel, string, Exception?> LogTarget { get; }
        Action<LoogLevel, string, string, TimeSpan> TimingTarget { get; }
    }
}
