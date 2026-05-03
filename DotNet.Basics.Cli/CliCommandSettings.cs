using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace DotNet.Basics.Cli
{
    public class CliCommandSettings : CommandSettings
    {
        [CommandOption("-d|--debug")]
        [Description("Pauses execution so debugger can be attached")]
        [DefaultValue(false)]
        public bool Debug { get; init; }

        [CommandOption("-l|--logLevel")]
        [Description("Minimum level for logging")]
        [DefaultValue(LogLevel.Information)]
        public LogLevel LogLevel { get; set; }
    }
}
