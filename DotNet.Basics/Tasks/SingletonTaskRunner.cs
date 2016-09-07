using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class SingletonTaskRunner : TaskRunner
    {
        public bool IsRunning(string taskId)
        {
            return new SingletonTask(taskId, rid => { }).IsRunning();
        }

        public void Run(string id, Action<string> task)
        {
            Run(new SingletonTask(id, task));
        }

        public async Task RunAsync(string id, Func<string, Task> task)
        {
            await RunAsync(new SingletonTask(id, task)).ConfigureAwait(false);
        }
    }
}
