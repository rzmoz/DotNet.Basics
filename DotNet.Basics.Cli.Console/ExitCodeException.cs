namespace DotNet.Basics.Cli.Console
{
    public class ExitCodeException(int exitCode) : Exception
    {
        public int ExitCode { get; set; } = exitCode;
    }
}
