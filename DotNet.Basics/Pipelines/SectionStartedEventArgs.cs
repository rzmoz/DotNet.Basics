using System;

namespace DotNet.Basics.Pipelines
{
    public class SectionStartedEventArgs<T> : EventArgs where T : EventArgs
    {
        public SectionStartedEventArgs(string name, SectionType type, T args)
        {
            Name = name;
            Type = type;
            Args = args;
        }

        public string Name { get; }
        public SectionType Type { get; }
        public T Args { get; }
    }
}
