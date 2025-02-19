using System;

namespace DotNet.Basics.Cli
{
    public interface IArgParser
    {
        public bool CanParse(Type t);
        public object Parse(string arg);
    }
}
