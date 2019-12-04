using System;
using System.Collections.Generic;
using System.Text;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli
{
    public interface ICliHost : ICliConfiguration
    {
        ILogger Log { get; }
    }
}
