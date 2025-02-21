using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Humanizer;

namespace DotNet.Basics.Serilog.Looging
{
    public class LongRunningOperations
    {
        private readonly ILoog _loog;
        private readonly ConcurrentDictionary<string, LongRunningOperation> _operations = new();
        private readonly Timer _timer;

        public LongRunningOperations(ILoog loog)
        : this(loog, TimeSpan.FromMinutes(1))
        { }
        public LongRunningOperations(ILoog loog, TimeSpan pingInterval)
        {
            _loog = loog;
            if (pingInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(pingInterval), "Must be bigger than 0");

            _timer = new Timer(pingInterval.TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += _timer_Elapsed;
            _loog.Verbose($"Long running operations initialized with ping interval: {$@"{pingInterval.Humanize()}".Highlight()}");
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_operations.Any() == false)
                return;

            var message = string.Empty;
            foreach (var operation in _operations.Values.OrderBy(o => o.StartTime))
                message += $"[ {operation.Name.Highlight()} has been running for {operation.DurationNow.Humanize().Highlight()} ]\r\n";

            _loog.Verbose(message.WithGutter());
        }

        public async Task<int> StartAsync(string name, Func<Task<int>> action)
        {
            var operation = new LongRunningOperation(name);

            var exitCode = int.MinValue;
            try
            {
                if (_operations.TryAdd(operation.Id, operation))
                    _loog.Debug($"{operation.Name.Highlight()} started");
                else
                    _loog.Error($"Failed to start {operation.Name.Highlight()} with Id: {operation.Id}");
                exitCode = await action.Invoke().ConfigureAwait(false);
                return exitCode;
            }
            catch (Exception e)
            {
                _loog.Timing(LoogLevel.Error, $"{operation.Name}", $"FAILED with exception: {e.Message}", operation.DurationNow);
                throw;
            }
            finally
            {
                if (_operations.TryRemove(operation.Id, out _) == false)
                    _loog.Verbose($"{operation.Name} not removed from {nameof(LongRunningOperations)} stack :-(");

                _loog.Debug($"{operation.Name.Highlight()} finished in {operation.DurationNow.Humanize().Highlight()}");

                if (exitCode != 0)
                    _loog.Fatal($"{operation.Name.Highlight()} FAILED with exit code: {exitCode.ToString().Highlight()}");
            }
        }
    }
}
