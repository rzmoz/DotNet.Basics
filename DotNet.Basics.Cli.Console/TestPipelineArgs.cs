namespace DotNet.Basics.Cli.Console
{
    public class TestPipelineArgs
    {
        public int ExitCode { get; set; }//optional
        public int OtherCode { get; set; } = 3939393;//optional
        public int? Mandatory { get; set; } = null; //mandatory when nullable and null
    }
}
