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
        private static ILogDispatcher _log = LogDispatcher.NullLogger;
        private static readonly ConcurrentDictionary<string, LongRunningOperation> _operations = new ConcurrentDictionary<string, LongRunningOperation>();
        private static Timer _timer;

        static LongRunningOperations()
        {
            Init(1.Minutes());
        }

        public static void Init(ILogDispatcher log)
        {
            _log = log ?? LogDispatcher.NullLogger;
        }
        public static void Init(TimeSpan pingInterval)
        {
            if (pingInterval <= TimeSpan.Zero)
                return;

            _timer?.Dispose();
            _timer = new Timer(pingInterval.TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += _timer_Elapsed;
            _log.Verbose($"Long running operations initialized with ping feedback: {$"{pingInterval:hh\\:mm\\:ss}".Highlight()}");
        }

        public static void Init(ILogDispatcher log, TimeSpan pingInterval)
        {
            Init(log);
            Init(pingInterval);
        }

        private static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_operations.Any() == false)
                return;

            var message = string.Empty;
            foreach (var operation in _operations.Values.OrderBy(o => o.StartTime))
                message += $"[ {operation.Name.Highlight()} has been running for {operation.DurationNowFormatted.Highlight()} ]\r\n";

            _log.Verbose(message.WithGutter());
        }

        public static async Task<int> StartAsync(string name, Func<Task<int>> action)
        {
            var operation = new LongRunningOperation(name);

            var exitCode = int.MinValue;
            try
            {
                if (_operations.TryAdd(operation.Id, operation))
                    _log.Timing(LogLevel.Debug, operation.Name, "starting", TimeSpan.MinValue);
                else
                    _log.Error($"Failed to start {operation.Name} with Id: {operation.Id}");
                exitCode = await action.Invoke().ConfigureAwait(false);
                return exitCode;
            }
            catch (Exception e)
            {
                _log.Timing(LogLevel.Error, $"{operation.Name}", $"FAILED with exception: {e.Message}", operation.DurationNow);
                throw;
            }
            finally
            {
                if (_operations.TryRemove(operation.Id, out var op) == false)
                    _log.Verbose($"{operation.Name} not removed from {nameof(LongRunningOperations)} stack :-(");

                if (exitCode == 0)
                    _log.Timing(LogLevel.Success, operation.Name, "DONE", operation.DurationNow);
                else if (exitCode != int.MinValue)
                    _log.Timing(LogLevel.Error, $"{operation.Name}", $"FAILED with exit code {exitCode}. See log for details", operation.DurationNow);
            }
        }
    }
}
