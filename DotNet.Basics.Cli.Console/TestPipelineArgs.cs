namespace DotNet.Basics.Cli.Console
{
    public class TestPipelineArgs
    {
        public int ExitCode { get; set; }//required since it has set
        public int OtherCode { get; } = 500;//optional since it has no set
    }
}
