using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics
{
    public class LongRunningOperationsOptions
    {
        public TimeSpan PingInterval { get; set; } = 1.Minutes();
    }
}
