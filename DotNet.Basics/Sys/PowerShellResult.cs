using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Sys
{
    public class PowerShellResult
    {
        public PowerShellResult(bool hadErrors, IEnumerable<object> passThru, IEnumerable<string> errorMessages = null)
        {
            HadErrors = hadErrors;
            PassThru = passThru?.ToArray();
            ErrorMessages = errorMessages?.ToArray();
        }

        public bool HadErrors { get; }
        public object[] PassThru { get; }
        public string[] ErrorMessages { get; }
    }
}
