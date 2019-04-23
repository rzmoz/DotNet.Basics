using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class VoidLogger : ILogDispatcher
    {
        public event LogDispatcher.MessageLoggedEventHandler MessageLogged;
        public ILogDispatcher InContext(string context, bool floatMessageLogged = true)
        {
            return this;
        }

        public void Verbose(string message)
        {
        }

        public void Verbose(string message, Exception e)
        {
        }

        public void Debug(string message)
        {
        }

        public void Debug(string message, Exception e)
        {
        }

        public void Information(string message)
        {
        }

        public void Information(string message, Exception e)
        {
        }

        public void Warning(string message)
        {
        }

        public void Warning(string message, Exception e)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(string message, Exception e)
        {
        }

        public void Critical(string message)
        {
        }

        public void Critical(string message, Exception e)
        {
        }

        public void Write(LogLevel level, string message)
        {
        }

        public void Write(LogLevel level, string message, Exception e)
        {
        }
    }
}
