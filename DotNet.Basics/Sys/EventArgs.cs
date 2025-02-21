using System;

namespace DotNet.Basics.Sys
{
    public class EventArgs<T>(T? value) : EventArgs
    {
        public EventArgs()
            : this(default)
        { }

        public T? Value { get; set; } = value;

        public override string ToString()
        {
            return Value == null ? string.Empty : Value.ToString();
        }
    }
}
