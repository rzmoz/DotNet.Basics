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

        [CommandOption("-l|--logLevel")]
        [Description("Minimum level for logging")]
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
    }
}
