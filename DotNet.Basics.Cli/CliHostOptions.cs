using System;
using System.Net;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli
{
    public sealed class CliHostOptions
    {
        public Action<CliException, ILogDispatcher> LogOnCliException { get; set; } = (e, log) =>
        {
            log.Error(e.Message, e.LogOptions == LogOptions.IncludeStackTrace ? e : null);
        };
        public Action<Exception, ILogDispatcher> LogOnException { get; set; } = (e, log) =>
         {
             log.Critical(e.Message, e);
         };

        public int ReturnCodeOnError { get; set; } = (int)HttpStatusCode.InternalServerError;
        public TimeSpan LongRunningOperationsPingInterval { get; set; } = 20.Seconds();
    }
}
