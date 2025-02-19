namespace DotNet.Basics.Cli
{
    public class CliHostBuilderOptions(string[] args, IArgsParser argsParser)
    {
        public int FatalExitCode { get; set; } = 500;
        
        public ArgsDictionary Args => argsParser.Parse(args);
        public bool Verbose => Args.ContainsKey(nameof(Verbose));
        public bool ADO => Args.ContainsKey(nameof(ADO));
        public bool Debug => Args.ContainsKey(nameof(Debug));
    }
}
