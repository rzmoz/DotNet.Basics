using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Cli
{
    public class MissingArgumentException(Type argsType, params IEnumerable<(string ArgName, string ArgType)> missingTypes) : Exception
    {
        public Type ArgsType { get; } = argsType;
        public (string ArgName, string ArgType)[] MissingArgs { get; set; } = missingTypes.ToArray();
    }
}
