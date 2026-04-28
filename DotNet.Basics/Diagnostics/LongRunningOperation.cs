using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Diagnostics
{
    public class LongRunningOperation(string name, Func<Task<int>> action)
    {
        public string Name { get; } = name;
        public string Id { get; } = Guid.NewGuid().ToString();
        public Func<Task<int>> Action { get; } = action;
        public DateTime StartTime { get; } = DateTime.Now;
        public TimeSpan DurationNow => DateTime.Now - StartTime;
    }
}
