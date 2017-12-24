namespace DotNet.Basics.Sys
{
    public static class CmdPrompt
    {
        public static (string Input, int ExitCode, string Output) Run(string commandString)
        {
            return Executable.Run("cmd.exe", $"/c {commandString}");
        }
    }
}
