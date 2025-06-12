using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public static class DevConsoleExtensions
    {
        public static IServiceCollection AddDevConsole(this IServiceCollection services, Action<DevConsoleOptions>? configureOptions = null)
        {
            var options = new DevConsoleOptions();
            configureOptions?.Invoke(options);

            var loggerProvider = new DevConsoleLoggerProvider(options);
            services.AddSingleton<ILogger>(s => loggerProvider.CreateLogger(string.Empty));
            services.AddSingleton<ILoggerProvider>(s => loggerProvider);
            return services;
        }
        public static ILoggingBuilder AddDevConsole(this ILoggingBuilder builder, Action<DevConsoleOptions>? configureOptions = null)
        {
            builder.Services.AddDevConsole(configureOptions);
            return builder;
        }


        public static bool IsSuccess(this string str)
        {
            return str.StartsWith(ConsoleMarkers.SuccessPrefix);
        }
        public static string StripSuccess(this string str)
        {
            return str.TrimStart(ConsoleMarkers.SuccessPrefix);
        }

        public static string Success(this string str)
        {
            return $"{ConsoleMarkers.SuccessPrefix}{str}";
        }
        public static string Highlight(this string str)
        {
            return $"{ConsoleMarkers.HighlightPrefix}{str}{ConsoleMarkers.HighlightSuffix}";
        }
        public static string StripHighlight(this string str)
        {
            return str.Replace($"{ConsoleMarkers.HighlightPrefix}", string.Empty).Replace($"{ConsoleMarkers.HighlightSuffix}", string.Empty);
        }
        public static string WithGutter(this string msg, int gutterSize = 26)
        {
            return msg.WithIndent(gutterSize);
        }
        public static string WithIndent(this string msg, int indent = 4)
        {
            var indentString = new string(' ', indent);
            if (msg.Contains("\r\n"))
                msg = Environment.NewLine + msg;
            return indentString + msg.Replace("\r\n", $"\r\n{indentString}");
        }
    }
    public static class ConsoleMarkers
    {
        public const char HighlightPrefix = '\u0086';
        public const char HighlightSuffix = '\u0087';
        public static readonly char SuccessPrefix = '\u00FE';
        public static readonly string SuccessPrefixString = $"{SuccessPrefix}";
    }
}
