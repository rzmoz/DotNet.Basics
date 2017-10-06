namespace DotNet.Basics.Shell
{
    public static class CommandPrompt
    {
        public static (int ExitCode, string Output) Run(string commandString)
        {
            return Executable.Run("cmd.exe", $"/c {commandString}");
        }
    }
}
