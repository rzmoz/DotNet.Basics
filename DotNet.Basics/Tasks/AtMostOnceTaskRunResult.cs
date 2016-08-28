using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class AtMostOnceTaskRunResult
    {
        public AtMostOnceTaskRunResult(string id, bool ran, string reason = null)
        {
            Id = id;
            Ran = ran;
            Reason = reason ?? string.Empty;
        }

        public string Id { get; }
        public bool Ran { get; }
        public string Reason { get; }
    }
}
