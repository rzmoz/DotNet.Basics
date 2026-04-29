using Humanizer;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Diagnostics
{
    public class LongRunningOperation(string name, Func<ILogger, Task<int>> action)
    {
        public string Name { get; } = name;
        public string Id { get; } = (name + Guid.NewGuid().ToString()).ToLower();
        public int ExitCode { get; private set; } = int.MinValue;
        public DateTime StartTime { get; private set; } = DateTime.MaxValue;
        public DateTime EndTime { get; private set; } = DateTime.MinValue;
        public TimeSpan Duration => EndTime - StartTime;

        public async Task<int> StartAsync(ILogger log)
        {
            StartTime = DateTime.Now;
            try
            {
                ExitCode = await action(log);
            }
            finally
            {
                EndTime = DateTime.Now;
            }
            return ExitCode;
        }

        public bool HasStarted() => StartTime < DateTime.MaxValue;
        public bool HasStopped() => EndTime > DateTime.MinValue;

        public virtual string GetPingMessage() => $"[ {Name.Highlight()} has been running for {Duration.Humanize().Highlight()} ]";
    }
}
