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
            Init(log, 15.Seconds());
        }

        public static void Init(ILogDispatcher log, TimeSpan pingInterval)
        {
            _log = log ?? LogDispatcher.NullLogger;

            if (pingInterval > TimeSpan.Zero)
            {
                _timer = new Timer(pingInterval.TotalMilliseconds)
                {
                    AutoReset = true,
                    Enabled = true
                };
                _timer.Elapsed += _timer_Elapsed;
                _log.Verbose($"Long running operations initialized with ping feedback: {pingInterval:hh\\:mm\\:ss}");
            }
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
                _log.Timing(LogLevel.Success, operation.Name, "finished", operation.DurationNow);
            }
            catch (Exception e)
            {
                _log.Timing(LogLevel.Error, $"{operation.Name}", $"FAILED: {e.Message}", operation.DurationNow);
                throw;
            }
            finally
            {
                if (_operations.TryRemove(operation.Id, out var op) == false)
                    _log.Verbose($"{operation.Name} not removed from {nameof(LongRunningOperations)} stack :-(");
            }
        }
    }
}
