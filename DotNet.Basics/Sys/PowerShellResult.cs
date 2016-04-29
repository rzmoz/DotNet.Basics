namespace DotNet.Basics.Sys
{
    public class PowerShellResult
    {
        public PowerShellResult(bool hadErrors, object[] passThru)
        {
            HadErrors = hadErrors;
            PassThru = passThru;
        }

        public bool HadErrors { get; }
        public object[] PassThru { get; }
    }
}
