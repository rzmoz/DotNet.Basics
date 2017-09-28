namespace DotNet.Basics.Shell
{
    public static class CmdPrompt
    {
        public static (int ExitCode, string Output) Run(string commandString)
        {
            #if DEBUG
            DebugOut.WriteLine($"CmdPrompt invoked: {commandString}");
            #endif
            return Executable.Run("cmd.exe", $"/c {commandString}");
        }
    }
}
