using System;
using DotNet.Basics.Serilog.Formatting;
using DotNet.Basics.Serilog.Looging;
using Serilog;

namespace DotNet.Basics.Serilog.Sinks
{
    public class SerilogLoogTarget : ILoogTarget
    {
        public bool ADO { get; }
        public bool Verbose { get; }

        public SerilogLoogTarget(bool verbose, bool ado)
        {
            ADO = ado;
            Verbose = verbose;
            LogTarget += LogHandler;
            TimingTarget += TimingHandler;
        }

        private void TimingHandler(LoogLevel lvl, string name, string @event, TimeSpan elapsed)
        {
            Log.Logger.Write(lvl.ToLogEventLevel(),
                elapsed > TimeSpan.MinValue
                    ? $"{name}: {@event} has been running for: {elapsed.ToHumanReadableString()}"
                    : $"{name}: {@event}");
        }

        private void LogHandler(LoogLevel lvl, string msg, Exception? e)
        {
            if (!Verbose && lvl < LoogLevel.Info)
                return;

            Log.Logger.Write(lvl.ToLogEventLevel(), e, lvl == LoogLevel.Success ? $"{ConsoleMarkers.SuccessPrefix}{msg}" : msg);
        }

        public Action<LoogLevel, string, Exception?> LogTarget { get; }
        public Action<LoogLevel, string, string, TimeSpan> TimingTarget { get; }
    }
}
