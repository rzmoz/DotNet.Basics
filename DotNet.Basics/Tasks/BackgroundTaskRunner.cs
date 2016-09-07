using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTaskRunner : TaskRunner
    {
        public void StartAsSingleton(string id, Action<string> task)
        {
            var sTask = new SingletonTask(id, task);
            Run(new BackgroundTask(sTask));
        }

        public void Start(Action<string> task)
        {
            Start(null, task);
        }
        public void Start(string id, Action<string> task)
        {
            var bgTask = new BackgroundTask(id, task);
            Run(bgTask);
        }

        public void StartAsSingleton(string id, Func<string, Task> task)
        {
            var sTask = new SingletonTask(id, task);
            RunAsync(new BackgroundTask(sTask)).Wait(CancellationToken.None);
        }
        public void Start(Func<string, Task> task)
        {
            Start(null, task);
        }
        public void Start(string id, Func<string, Task> task)
        {
            var bgTask = new BackgroundTask(id, task);
            RunAsync(bgTask).Wait(CancellationToken.None);
        }

    }
}
