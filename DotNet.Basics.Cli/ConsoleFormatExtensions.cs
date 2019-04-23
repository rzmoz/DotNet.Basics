using DotNet.Basics.Sys;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli
{
    public static class ConsoleFormatExtensions
    {
        public static string ToOutputString(this LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "VRB";
                case LogLevel.Debug:
                    return "DBG";
                case LogLevel.Information:
                    return "INF";
                case LogLevel.Warning:
                    return "WARNING";
                case LogLevel.Error:
                case LogLevel.Critical:
                    return $"  *****  {level.ToName().ToUpperInvariant()}  *****  ";
                default:
                    return level.ToString("u3");
            }
        }
    }
}
