using System;
using System.Collections.Generic;

namespace DotNet.Basics.Cli
{
    public class UnknownArgumentsException(IReadOnlyList<string> argNames) : Exception
    {
        public IReadOnlyList<string> ArgNames { get; set; } = argNames;
    }
}
