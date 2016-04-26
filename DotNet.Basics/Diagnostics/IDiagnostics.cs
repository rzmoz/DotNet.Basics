using System;

namespace DotNet.Basics.Diagnostics
{
    public interface IDiagnostics
    {
        void Metric(string name, double value);
        void Profile(string taskName, TimeSpan duration);
    }
}

