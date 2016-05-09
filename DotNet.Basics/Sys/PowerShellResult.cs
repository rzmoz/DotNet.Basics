namespace DotNet.Basics.Sys
{
    public class PowerShellResult
    {
        public PowerShellResult(bool hadErrors, object[] passThru, string[] errorMessages)
        {
            HadErrors = hadErrors;
            PassThru = passThru;
            ErrorMessages = errorMessages;
        }

        public bool HadErrors { get; }
        public object[] PassThru { get; }
        public string[] ErrorMessages { get; }
    }
}
