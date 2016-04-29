using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.Sys
{
    public class PSResult
    {
        public PSResult(bool hadErrors, object[] passThru)
        {
            HadErrors = hadErrors;
            PassThru = passThru;
        }

        public bool HadErrors { get; }
        public object[] PassThru { get; }
    }
}
