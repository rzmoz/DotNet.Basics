using System;

namespace DotNet.Standard.Sys
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs()
            : this(default(T))
        { }

        public EventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; set; }

        public override string ToString()
        {
            return Value == null ? string.Empty : Value.ToString();
        }
    }
}
