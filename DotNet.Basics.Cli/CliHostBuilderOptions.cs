namespace DotNet.Basics.Cli
{
    public class CliHostBuilderOptions(string[] args, IArgsParser argsParser)
    {   
        public int FatalExitCode { get; set; } = 500;
        public ArgsDictionary Args { get; } = argsParser.Parse(args);
        public bool WithSerilogDevConsole { get; set; } = true;
    }
}
