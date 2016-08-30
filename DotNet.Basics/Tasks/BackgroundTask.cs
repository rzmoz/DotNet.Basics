using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTask
    {
        private readonly Func<CancellationToken, Task> _action;

        public BackgroundTask(string id, Func<CancellationToken, Task> action)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (action == null) throw new ArgumentNullException(nameof(action));
            Id = id;
            _action = action;
            Properties = new ConcurrentDictionary<string, string>();
            StartAsync = WhenIdle;
        }
        public string Id { get; }
        public Func<CancellationToken, Task> StartAsync;

        private Task WhenRunning(CancellationToken ct)
        {
            return Task.FromResult(false);
        }

        private async Task WhenIdle(CancellationToken ct)
        {

            StartAsync = WhenRunning;
            await _action.Invoke(ct).ConfigureAwait(false);
        }

        public ConcurrentDictionary<string, string> Properties { get; }
    }
}
