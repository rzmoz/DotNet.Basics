using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTaskRunner : TaskRunner
    {
        public bool IsRunning(string id)
        {
            return TaskFactory.Create<BackgroundTask>(id).IsRunning();
        }

        public void StartAsSingleton(string id, Action<string> task)
        {
            var sTask = TaskFactory.Create<SingletonTask>(id, task);
            Run(TaskFactory.Create<BackgroundTask>(sTask));
        }

        public void Start(Action<string> task)
        {
            Start(null, task);
        }
        public void Start(string id, Action<string> task)
        {
            var bgTask = TaskFactory.Create<BackgroundTask>(id, task);
            Run(bgTask);
        }
        public void StartAsSingleton(string id, Func<string, Task> task)
        {
            var sTask = TaskFactory.Create<SingletonTask>(id, task);
            RunAsync(TaskFactory.Create<BackgroundTask>(sTask)).Wait(CancellationToken.None);
        }
        public void Start(Func<string, Task> task)
        {
            Start(null, task);
        }
        public void Start(string id, Func<string, Task> task)
        {
            var bgTask = TaskFactory.Create<BackgroundTask>(id, task);
            RunAsync(bgTask).Wait(CancellationToken.None);
        }
    }
}
