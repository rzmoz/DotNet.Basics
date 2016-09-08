using System;

namespace DotNet.Basics.Pipelines
{
    public class SectionEndedEventArgs<T> : SectionStartedEventArgs<T> where T : EventArgs
    {
        public SectionEndedEventArgs(string name, SectionType type, T args, bool wasCancelled, Exception exception) : base(name, type, args)
        {
            WasCancelled = wasCancelled;
            Exception = exception;
        }

        public bool WasCancelled { get; }
        public Exception Exception { get; }
    }
}
