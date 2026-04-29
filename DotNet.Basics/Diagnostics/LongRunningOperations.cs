using DotNet.Basics.Sys;
using Humanizer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace DotNet.Basics.Diagnostics
{
    public sealed class LongRunningOperations
    {
        private readonly ILogger _loog;
        private readonly ConcurrentDictionary<string, LongRunningOperation> _operations = new();
        private readonly Timer? _timer = null;

        public LongRunningOperationsOptions Options { get; }

        public LongRunningOperations(ILogger loog, LongRunningOperationsOptions? o)
        {
            _loog = loog;
            Options = o ?? new LongRunningOperationsOptions();

            if (Options.PingInterval > TimeSpan.Zero)
            {
                _timer = new Timer(Options.PingInterval.TotalMilliseconds)
                {
                    AutoReset = true,
                    Enabled = true
                };
                _timer.Elapsed += _timer_Elapsed;
                _loog.Trace("Long running operations initialized with ping interval: {int}", o.PingInterval.Humanize());
            }
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var message = _operations.Values
                .OrderBy(o => o.StartTime)
                .Select(o => o.GetPingMessage())
                .JoinString("\r\n");

            if (!string.IsNullOrEmpty(message))
                _loog?.Debug(message.WithGutter(gutterSize: 10));
        }
        public async Task<int> RunAsync(string name, Func<ILogger, Task<int>> action)
        {
            return await RunAsync(new LongRunningOperation(name, action)).ConfigureAwait(false);
        }

        public async Task<int> RunAsync(LongRunningOperation operation)
        {
            var exitCode = int.MinValue;
            bool added = _operations.TryAdd(operation.Id, operation);
            try
            {

                _loog.Info("{operationName} started", operation.Name.Highlight());
                exitCode = await operation.StartAsync(_loog).ConfigureAwait(false);
                return exitCode;
            }
            catch (Exception e)
            {
                _loog.Timing(LogLevel.Critical, operation.Name, $"FAILED with exception {e.Message}", operation.Duration);
                throw;
            }
            finally
            {
                if (added && !_operations.TryRemove(operation.Id, out _))
                    _loog.Error("{operationName} not removed from {operatorName} stack :-(", operation.Name, nameof(LongRunningOperations));
                else
                    _loog.Info("{operationName} finished in {operationDurationNow}", operation.Name.Highlight(), operation.Duration.Humanize().Highlight());

                if (exitCode != 0)
                    _loog.Critical("{operationName} FAILED with exit code: {exitCode}", operation.Name.Highlight(), exitCode.ToString().Highlight());
            }
        }
    }
}
