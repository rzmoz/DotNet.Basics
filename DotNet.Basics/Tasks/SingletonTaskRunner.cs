using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class SingletonTaskRunner : TaskRunner
    {
        public bool IsRunning(string taskId)
        {
            return TaskFactory.Create<SingletonTask>(taskId).IsRunning();
        }

        public void Run(string id, Action<string> task)
        {
            Run(TaskFactory.Create<SingletonTask>(id, task));
        }

        public async Task RunAsync(string id, Func<string, Task> task)
        {
            await RunAsync(TaskFactory.Create<SingletonTask>(id, task)).ConfigureAwait(false);
        }
    }
}
