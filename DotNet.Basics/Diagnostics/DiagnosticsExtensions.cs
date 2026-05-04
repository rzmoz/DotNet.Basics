using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public static partial class DiagnosticsExtensions
    {
        private static readonly EventId _emptyEventId = new EventId(0);
        private const string _successMarker = "✓";

        public const char HighlightStart = '»';
        public const char HighlightEnd = '«';

        public static string Highlight(this string str)
        {
            return $"{HighlightStart}{str}{HighlightEnd}";
        }

        public static bool IsSuccess(this string str)
        {
            return str.EndsWith(_successMarker);
        }
        private static string AddSuccess(this string str, bool isSuccess = true)
        {
            return isSuccess ? $"{str} {_successMarker}" : str;
        }

        // L0G
        public static void L0G(this ILogger log, LogLevel level, string msg, Exception? e = null)
        {
            LoggerMessage.Define(level, _emptyEventId, msg).Invoke(log, e);
        }
        public static void L0G<T>(this ILogger log, LogLevel level, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(level, _emptyEventId, template).Invoke(log, arg1, e);
        }
        public static void L0G<T1, T2>(this ILogger log, LogLevel level, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(level, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        public static void L0G<T1, T2, T3>(this ILogger log, LogLevel level, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(level, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        public static void L0G<T1, T2, T3, T4>(this ILogger log, LogLevel level, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(level, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        public static void L0G<T1, T2, T3, T4, T5>(this ILogger log, LogLevel level, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(level, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }
        public static void L0G<T1, T2, T3, T4, T5, T6>(this ILogger log, LogLevel level, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(level, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, arg6, e);
        }

        // TRACE
        public static void Trace(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Trace, _emptyEventId, msg).Invoke(log, e);
        }
        public static void Trace<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, e);
        }
        public static void Trace<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        public static void Trace<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        public static void Trace<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        public static void Trace<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }
        public static void Trace<T1, T2, T3, T4, T5, T6>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, arg6, e);
        }

        // DEBUG
        public static void Debug(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Debug, _emptyEventId, msg).Invoke(log, e);
        }
        public static void Debug<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, e);
        }
        public static void Debug<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        public static void Debug<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        public static void Debug<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        public static void Debug<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }
        public static void Debug<T1, T2, T3, T4, T5, T6>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, arg6, e);
        }

        // INFO
        public static void Info(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Information, _emptyEventId, msg).Invoke(log, e);
        }
        public static void Info<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, e);
        }
        public static void Info<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        public static void Info<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        public static void Info<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        public static void Info<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }
        public static void Info<T1, T2, T3, T4, T5, T6>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, arg6, e);
        }
        //SUCCESS
        public static void Success(this ILogger log, string msg, Exception? e = null)
        {
            DiagnosticsExtensions.Info(log, msg.AddSuccess(), e);
        }
        public static void Success<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            DiagnosticsExtensions.Info(log, template.AddSuccess(), arg1, e);
        }
        public static void Success<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            DiagnosticsExtensions.Info(log, template.AddSuccess(), arg1, arg2, e);
        }
        public static void Success<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            DiagnosticsExtensions.Info(log, template.AddSuccess(), arg1, arg2, arg3, e);
        }
        public static void Success<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            DiagnosticsExtensions.Info(log, template.AddSuccess(), arg1, arg2, arg3, arg4, e);
        }
        public static void Success<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            DiagnosticsExtensions.Info(log, template.AddSuccess(), arg1, arg2, arg3, arg4, arg5, e);
        }
        //WARN
        public static void Warn(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Warning, _emptyEventId, msg).Invoke(log, e);
        }
        public static void Warn<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, e);
        }
        public static void Warn<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        public static void Warn<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        public static void Warn<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        public static void Warn<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }
        public static void Warn<T1, T2, T3, T4, T5, T6>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, arg6, e);
        }

        // ERROR
        public static void Error(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Error, _emptyEventId, msg).Invoke(log, e);
        }
        public static void Error<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, e);
        }
        public static void Error<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        public static void Error<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        public static void Error<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        public static void Error<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }
        public static void Error<T1, T2, T3, T4, T5, T6>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, arg6, e);
        }

        // CRITICAL
        public static void Critical(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Critical, _emptyEventId, msg).Invoke(log, e);
        }
        public static void Critical<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, e);
        }
        public static void Critical<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        public static void Critical<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        public static void Critical<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        public static void Critical<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }
        public static void Critical<T1, T2, T3, T4, T5, T6>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5, T6>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, arg6, e);
        }
    }
}