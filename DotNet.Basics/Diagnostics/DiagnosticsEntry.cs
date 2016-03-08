using System;

namespace DotNet.Basics.Diagnostics
{
    public abstract class DiagnosticsEntry
    {
        protected DiagnosticsEntry(DateTime timestamp, DiagnosticsType diagnosticsType)
        {
            Timestamp = timestamp;
            DiagnosticsType = diagnosticsType;
        }

        public DateTime Timestamp { get; }
        public DiagnosticsType DiagnosticsType { get; }

        protected string TimestampToStringFormatted => Timestamp.ToString("yyyy-MM-dd hh:mm:ss");

        protected abstract string GetMessage();

        public override string ToString()
        {
            return $"{TimestampToStringFormatted} {GetMessage()}";
        }
    }
}
