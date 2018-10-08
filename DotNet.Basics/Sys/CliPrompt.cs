namespace DotNet.Basics.Sys
{
    public static class CliPrompt
    {
        public static (string Input, int ExitCode, string Output) Run(string commandString)
        {
            return ExternalProcess.Run("cmd.exe", $"/c {commandString}");
        }
    }
}
