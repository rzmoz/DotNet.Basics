﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace DotNet.Basics.Serilog.Diagnostics
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
            _loog.Verbose($"Long running operations initialized with ping interval: {$@"{pingInterval:hh\:mm\:ss}".Highlight()}");
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_operations.Any() == false)
                return;

            var message = string.Empty;
            foreach (var operation in _operations.Values.OrderBy(o => o.StartTime))
                message += $"[ {operation.Name.Highlight()} has been running for {operation.DurationNowFormatted.Highlight()} ]\r\n";

            _loog.Verbose(message.WithGutter());
        }

        public async Task<int> StartAsync(string name, Func<Task<int>> action)
        {
            var operation = new LongRunningOperation(name);

            var exitCode = int.MinValue;
            try
            {
                if (_operations.TryAdd(operation.Id, operation))
                    _loog.Timing(LoogLevel.Debug, operation.Name, "starting", TimeSpan.MinValue);
                else
                    _loog.Error($"Failed to start {operation.Name} with Id: {operation.Id}");
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
                if (_operations.TryRemove(operation.Id, out var op) == false)
                    _loog.Verbose($"{operation.Name} not removed from {nameof(LongRunningOperations)} stack :-(");

                if (exitCode == 0)
                    _loog.Timing(LoogLevel.Success, operation.Name, "DONE", operation.DurationNow);
                else if (exitCode != int.MinValue)
                    _loog.Timing(LoogLevel.Error, $"{operation.Name}", $"FAILED with exit code {exitCode}. See loog for details", operation.DurationNow);
            }
        }
    }
}
