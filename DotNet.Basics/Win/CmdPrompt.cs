using DotNet.Basics.Sys;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Win
{
    public static class CmdPrompt
    {
        public static int Run(string commandString, ILogger? logger = null)
        {
            return ExternalProcess.Run("cmd.exe", $"/c {commandString}", logger);
        }
    }
}
