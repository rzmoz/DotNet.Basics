using System;
using System.Data;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class ReadOnlyInMemLogger : InMemLogger
    {
        public override void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            throw new ReadOnlyException();
        }
        protected override void Log(LogEntry entry)
        {
            throw new ReadOnlyException();
        }
    }
}
