using System;

namespace DotNet.Basics.ConsoleApp
{
    public class CmdLineParam
    {
        public CmdLineParam(string name)
        {
            Name = name;
            Help = string.Empty;
            Required = true;
            AllowEmptyValue = false;
            Value = string.Empty;
            Exists = false;
            ReadAction = param => { };
        }

        public void SetValue(string value)
        {
            Value = value;
            Exists = true;
        }

        public string Value { get; private set; }
        public string Help { get; set; }
        public bool Exists { get; private set; }
        public bool Required { get; set; }
        public bool AllowEmptyValue { get; set; }
        public string Name { get; private set; }
        internal Action<CmdLineParam> ReadAction { get; set; }
    }
}
