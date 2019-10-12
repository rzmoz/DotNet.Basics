using System;
using System.Net;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli
{
    public sealed class CliHostOptions
    {
        public Action<CliException, ILogDispatcher> LogOnCliException { get; set; } = (e, log) =>
        {
            log.Error(e.Message.Highlight(), e.LogOptions == LogOptions.IncludeStackTrace ? e : null);
        };
        public Action<Exception, ILogDispatcher> LogOnException { get; set; } = (e, log) =>
         {
             log.Error(e.Message.Highlight(), e);
         };

        public int ReturnCodeOnError { get; set; } = (int)HttpStatusCode.InternalServerError;
        public TimeSpan LongRunningOperationsPingInterval { get; set; } = TimeSpan.MinValue;
    }
}
