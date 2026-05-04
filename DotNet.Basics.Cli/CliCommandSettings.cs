using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace DotNet.Basics.Cli
{
    public class CliCommandSettings : CommandSettings
    {
        [CommandOption("-d|--debug")]
        [Description("Pauses execution so debugger can be attached")]
        public bool Debug { get; init; } = false;

        [CommandOption("-v|--verbose")]
        [Description("Sets LogLevel to Trace. Verbose overrides LogLevel if set!")]
        public bool Verbose { get; init; } = false;

        [CommandOption("-l|--logLevel")]
        [Description("Minimum level for logging. Verbose overrides LogLevel if set!")]
        public LogLevel LogLevel { get; set; } = LogLevel.Information;        
    }
}
