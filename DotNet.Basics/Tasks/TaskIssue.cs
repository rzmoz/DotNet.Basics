using System;

namespace DotNet.Basics.Tasks
{
    public class TaskIssue
    {
        public TaskIssue(string message)
            : this(message, null)
        { }

        public TaskIssue(Exception exception)
            : this(exception?.Message, exception)
        { }

        public TaskIssue(string message, Exception exception)
        {
            Message = message ?? string.Empty;
            Exception = exception;
            HasException = Exception != null;
        }

        public string Message { get; }
        public Exception Exception { get; }

        public bool HasException { get; }

        public override string ToString()
        {
            return $"{Message}";
        }
    }
}
