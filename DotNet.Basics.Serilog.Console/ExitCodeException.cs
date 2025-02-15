namespace DotNet.Basics.Serilog.Console
{
    public class ExitCodeException(int exitCode) : Exception
    {
        public int ExitCode { get; set; } = exitCode;
    }
}
