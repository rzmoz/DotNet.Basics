using System;

namespace DotNet.Basics.Sys
{
    public class TEventArgs<T> : EventArgs
    {
        public TEventArgs()
            : this(default(T))
        {
        }

        public TEventArgs(T value)
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
