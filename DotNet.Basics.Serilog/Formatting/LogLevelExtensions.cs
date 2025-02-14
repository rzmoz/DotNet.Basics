﻿using DotNet.Basics.Serilog.Diagnostics;
using Serilog.Events;

namespace DotNet.Basics.Serilog.Formatting
{
    public static class LogLevelExtensions
    {
        public static LogEventLevel ToLogEventLevel(this LoogLevel lvl)
        {
            return lvl switch
            {
                LoogLevel.Verbose => LogEventLevel.Verbose,
                LoogLevel.Debug => LogEventLevel.Debug,
                LoogLevel.Info => LogEventLevel.Information,
                LoogLevel.Success => LogEventLevel.Information,
                LoogLevel.Warning => LogEventLevel.Warning,
                LoogLevel.Error => LogEventLevel.Error,
                LoogLevel.Raw => LogEventLevel.Information,
                _ => LogEventLevel.Information
            };
        }
    }
}
