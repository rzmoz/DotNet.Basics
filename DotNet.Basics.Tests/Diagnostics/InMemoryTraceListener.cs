using System.Diagnostics;
using System.Text;

namespace DotNet.Basics.Tests.Diagnostics
{
    public class InMemoryTraceListener : TraceListener
    {
        private readonly StringBuilder _buffer;

        public InMemoryTraceListener()
        {
            _buffer = new StringBuilder();
        }

        public override void Write(string message)
        {
            //we discard the process information
        }

        public override void WriteLine(string message)
        {
            _buffer.AppendLine(message);
        }

        public string Content => _buffer.ToString();

        public void Clear()
        {
            _buffer.Clear();
        }
    }
}
