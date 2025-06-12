using System;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public static class LoggerExtensions
    {
        private static readonly EventId _emptyEventId = new EventId(0);

        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <param name="log"></param>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void L0G(this ILogger log, LogLevel level, string msg, Exception? e = null)
        {
            LoggerMessage.Define(level, _emptyEventId, msg).Invoke(log, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="log"></param>
        /// <param name="level"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="e"></param>
        public static void L0G<T>(this ILogger log, LogLevel level, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(level, _emptyEventId, template).Invoke(log, arg1, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="log"></param>
        /// <param name="level"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="e"></param>
        public static void L0G<T1, T2>(this ILogger log, LogLevel level, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(level, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="log"></param>
        /// <param name="level"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="e"></param>
        public static void L0G<T1, T2, T3>(this ILogger log, LogLevel level, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(level, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="log"></param>
        /// <param name="level"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="e"></param>
        public static void L0G<T1, T2, T3, T4>(this ILogger log, LogLevel level, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(level, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="log"></param>
        /// <param name="level"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="e"></param>
        public static void L0G<T1, T2, T3, T4, T5>(this ILogger log, LogLevel level, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(level, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }

        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <param name="log"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void Trace(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Trace, _emptyEventId, msg).Invoke(log, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="e"></param>
        public static void Trace<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="e"></param>
        public static void Trace<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="e"></param>
        public static void Trace<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="e"></param>
        public static void Trace<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="e"></param>
        public static void Trace<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Trace, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <param name="log"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void Debug(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Debug, _emptyEventId, msg).Invoke(log, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="e"></param>
        public static void Debug<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="e"></param>
        public static void Debug<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="e"></param>
        public static void Debug<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="e"></param>
        public static void Debug<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="e"></param>
        public static void Debug<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Debug, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <param name="log"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void Info(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Information, _emptyEventId, msg).Invoke(log, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="e"></param>
        public static void Info<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="e"></param>
        public static void Info<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="e"></param>
        public static void Info<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="e"></param>
        public static void Info<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="e"></param>
        public static void Info<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Information, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }

        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <param name="log"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void Warn(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Warning, _emptyEventId, msg).Invoke(log, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="e"></param>
        public static void Warn<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="e"></param>
        public static void Warn<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="e"></param>
        public static void Warn<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="e"></param>
        public static void Warn<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="e"></param>
        public static void Warn<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Warning, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }

        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <param name="log"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void Error(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Error, _emptyEventId, msg).Invoke(log, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="e"></param>
        public static void Error<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="e"></param>
        public static void Error<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="e"></param>
        public static void Error<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="e"></param>
        public static void Error<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="e"></param>
        public static void Error<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Error, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }

        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <param name="log"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public static void Critical(this ILogger log, string msg, Exception? e = null)
        {
            LoggerMessage.Define(LogLevel.Critical, _emptyEventId, msg).Invoke(log, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="e"></param>
        public static void Critical<T>(this ILogger log, string template, T arg1, Exception? e = null)
        {
            LoggerMessage.Define<T>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="e"></param>
        public static void Critical<T1, T2>(this ILogger log, string template, T1 arg1, T2 arg2, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, arg2, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="e"></param>
        public static void Critical<T1, T2, T3>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="e"></param>
        public static void Critical<T1, T2, T3, T4>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, e);
        }
        /// <summary>
        /// High performance logging: https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="log"></param>
        /// <param name="template"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="e"></param>
        public static void Critical<T1, T2, T3, T4, T5>(this ILogger log, string template, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception? e = null)
        {
            LoggerMessage.Define<T1, T2, T3, T4, T5>(LogLevel.Critical, _emptyEventId, template).Invoke(log, arg1, arg2, arg3, arg4, arg5, e);
        }

        public static void Timing(this ILogger log, LogLevel level, string name, string @event, Exception? e = null)
        {
            log.Timing(level, name, @event, TimeSpan.MinValue);
        }

        public static void Timing(this ILogger log, LogLevel level, string name, string @event, TimeSpan elapsed, Exception? e = null)
        {
            LoggerMessage.Define<string, string, string>(level, _emptyEventId, "{name}: {@event} [{duration}]").Invoke(log, name, @event, elapsed.Humanize(), e);
        }
    }
}

