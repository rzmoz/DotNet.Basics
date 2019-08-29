using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics
{
    public static class LongRunningOperations
    {
        private static ILogDispatcher _log;
        private static readonly ConcurrentDictionary<string, LongRunningOperation> _operations = new ConcurrentDictionary<string, LongRunningOperation>();
        private static Timer _timer;

        public static void Init(ILogDispatcher log)
        {
            Init(log, 20.Seconds());
        }

        public static void Init(ILogDispatcher log, TimeSpan pingInterval)
        {
            _log = log;
            _timer = new Timer(pingInterval.TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += _timer_Elapsed;
        }

        private static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_operations.Any() == false)
                return;

            var message = string.Empty;
            foreach (var operation in _operations.Values.OrderBy(o => o.StartTime))
                message += $"[ {operation.Name.Highlight()} has been running for {operation.DurationNowFormatted.Highlight()} ]\r\n".WithGutter();

            _log.Verbose(message);
        }

        public static async Task StartAsync(string name, Func<Task> action)
        {
            var operation = new LongRunningOperation(name);
            try
            {
                _operations.TryAdd(operation.Id, operation);
                await action.Invoke().ConfigureAwait(false);
                _log.Timing(operation.Name, "finished", operation.DurationNow);
            }
            finally
            {
                _operations.TryRemove(operation.Id, out var op);
            }
        }
    }
}
