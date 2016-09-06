using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTaskRunner
    {
        public event ManagedTask.TaskStartingEventHandler TaskStarting;
        public event ManagedTask.TaskEndedEventHandler TaskEnded;

        public void Start(Action task, string id = null, bool asSingleton = false)
        {
            Start(new ManagedTask(task, id), asSingleton);
        }
        public void Start(Func<CancellationToken, Task> task, string id = null, bool asSingleton = false)
        {
            Start(new ManagedTask(task, id), asSingleton);
        }

        private void Start(ManagedTask task, bool asSingleton)
        {
            if (asSingleton)
                task = new SingletonTask(task.Run, task.RunAsync, task.Id);
            task = new BackgroundTask(task.Run, task.RunAsync, task.Id);
            task.TaskStarting += (tid, rid, started, reason) => { TaskStarting?.Invoke(tid, rid, started, reason); };
            task.TaskEnded += (tid, rid, e) => { TaskEnded?.Invoke(tid, rid, e); };
            task.Run();
        }
    }
}
