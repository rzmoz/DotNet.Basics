using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace DotNet.Basics.Cli
{
    public class CliCommandSettings : CommandSettings
    {
        [CommandOption("-d|--debug")]
        [Description("Sets LogLevel to Debug. Debug overrides LogLevel if set.")]
        public bool Debug { get; init; } = false;

        [CommandOption("-v|--verbose")]
        [Description("Sets LogLevel to Trace. Verbose overrides LogLevel and Debug if set.")]
        public bool Verbose { get; init; } = false;

        [CommandOption("-p|--pause")]
        [Description("Pauses execution at start to wait for debugger attach. Wait times out if no key is pressed.")]
        public bool PauseForDebuggerAttach { get; init; } = false;

        [CommandOption("-l|--logLevel")]
        [Description("Minimum level for logging.")]
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
    }
}
