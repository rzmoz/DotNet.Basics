using Humanizer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace DotNet.Basics.Diagnostics
{
    public class LongRunningOperations
    {
        private readonly ILogger? _loog;
        private readonly ConcurrentDictionary<string, LongRunningOperation> _operations = new();
        private readonly Timer _timer;

        public LongRunningOperations(ILogger? loog)
        : this(loog, new LongRunningOperationsOptions())
        { }
        public LongRunningOperations(ILogger? loog, LongRunningOperationsOptions o)
        {
            _loog = loog;
            if (o.PingInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(o.PingInterval), "Must be bigger than 0");

            _timer = new Timer(o.PingInterval.TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += _timer_Elapsed;
            _loog?.Trace("Long running operations initialized with ping interval: {}", o.PingInterval.Humanize());
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_operations.Any() == false)
                return;

            var message = string.Empty;
            foreach (var operation in _operations.Values.OrderBy(o => o.StartTime))
                message += $"[ {operation.Name.Highlight()} has been running for {operation.DurationNow.Humanize().Highlight()} ]\r\n";

            _loog?.Debug(message.WithGutter());
        }

        public async Task<int> StartAsync(string name, Func<Task<int>> action)
        {
            var operation = new LongRunningOperation(name);

            var exitCode = int.MinValue;
            try
            {
                if (_operations.TryAdd(operation.Id, operation))
                    _loog?.Debug("{operationName} started", operation.Name.Highlight());
                else
                    _loog?.Error("Failed to start {operationName} with Id: {operationId}", operation.Name.Highlight(), operation.Id);
                exitCode = await action.Invoke().ConfigureAwait(false);
                return exitCode;
            }
            catch (Exception e)
            {
                _loog?.Timing(LogLevel.Critical, operation.Name, $"FAILED with exception {e.Message}", operation.DurationNow);
                throw;
            }
            finally
            {
                if (_operations.TryRemove(operation.Id, out _) == false)
                    _loog?.Warn("{operationName} not removed from {operatorName} stack :-(", operation.Name, nameof(LongRunningOperations));

                _loog?.Debug("{operationName} finished in {operationDurationNow}", operation.Name.Highlight(), operation.DurationNow.Humanize().Highlight());

                if (exitCode != 0)
                    _loog?.Critical("{operationName} FAILED with exit code: {exitCode}", operation.Name.Highlight(), exitCode.ToString().Highlight());
            }
        }
    }
}
