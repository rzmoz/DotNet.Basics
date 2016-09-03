using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class AtMostOnceTaskRunner : TaskRunner<TaskOptions>
    {
        public async Task StartTaskAsync(string taskId, Func<CancellationToken, Task> task)
        {
            await StartTaskAsync(taskId, task, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task StartTaskAsync(string taskId, Func<CancellationToken, Task> task, CancellationToken ct)
        {
            await RunAsync(new AsyncTask(task, taskId), runAsSingleton: false, runInBackground: true).ConfigureAwait(false);
        }
    }
}
