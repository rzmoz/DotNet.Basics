﻿using System;
using DotNet.Basics.Serilog.Diagnostics;
using DotNet.Basics.Serilog.Formatting;
using DotNet.Basics.Sys;
using Serilog;

namespace DotNet.Basics.Serilog.Sinks
{
    public class SerilogLoogTarget : ILoogTarget
    {
        public SerilogLoogTarget()
        {
            LogTarget += LogHandler;
            TimingTarget += TimingHandler;
        }

        private void TimingHandler(LoogLevel lvl, string name, string @event, TimeSpan elapsed)
        {
            Log.Logger.Write(lvl.ToLogEventLevel(),
                elapsed > TimeSpan.MinValue
                    ? $"{name}:{@event} has been running for: {elapsed.ToHumanReadableString()}"
                    : $"{name}:{@event}");
        }

        private void LogHandler(LoogLevel lvl, string msg, Exception? e)
        {
            Log.Logger.Write(lvl.ToLogEventLevel(), e, msg);
        }

        public Action<LoogLevel, string, Exception?> LogTarget { get; }
        public Action<LoogLevel, string, string, TimeSpan> TimingTarget { get; }
    }
}
