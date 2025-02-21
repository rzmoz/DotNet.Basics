using System;

namespace DotNet.Basics.Serilog.Looging
{
    public class NullLoog : ILoog
    {
#pragma warning disable 0067
        public event Loog.MessageLoggedEventHandler? MessageLogged;
        public event Loog.TimingLoggedEventHandler? TimingLogged;
#pragma warning restore 0067
        public static ILoog Instance { get; } = new NullLoog();

        public ILoog WithLogTarget(ILoogTarget target) { return this; }
        public ILoog InContext(string context, bool floatMessageLogged = true) { return this; }
        public void Raw(string message) { }
        public void Raw(string message, Exception e) { }
        public void Verbose(string message) { }
        public void Verbose(string message, Exception e) { }
        public void Debug(string message) { }
        public void Debug(string message, Exception e) { }
        public void Info(string message) { }
        public void Info(string message, Exception e) { }
        public void Success(string message) { }
        public void Success(string message, Exception e) { }
        public void Warning(string message) { }
        public void Warning(string message, Exception e) { }
        public void Error(string message) { }
        public void Error(string message, Exception e) { }
        public void Fatal(string message) { }
        public void Fatal(string message, Exception e) { }
        public void Write(LoogLevel level, string message) { }
        public void Write(LoogLevel level, string message, Exception e) { }
        public void Timing(LoogLevel level, string name, string @event) { }
        public void Timing(LoogLevel level, string name, string @event, TimeSpan duration) { }
    }
}
