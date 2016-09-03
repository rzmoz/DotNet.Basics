using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class AtMostOnceTaskRunner : TaskRunner
    {
        public async Task StartTaskAsync(string taskId, Func<CancellationToken, Task> task)
        {
            await StartTaskAsync(taskId, task, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task StartTaskAsync(string taskId, Func<CancellationToken, Task> task, CancellationToken ct)
        {
            if (taskId == null) throw new ArgumentNullException(nameof(taskId));
            if (string.IsNullOrWhiteSpace(taskId)) throw new ArgumentException(nameof(taskId));
            await RunAsync(new AsyncTask(task, taskId), ct, runAsSingleton: true, runInBackground: true).ConfigureAwait(false);
        }
    }
}
