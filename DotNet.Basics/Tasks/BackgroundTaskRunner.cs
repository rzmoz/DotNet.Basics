using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTaskRunner
    {
        public event ManagedTask.TaskStartingEventHandler TaskStarting;
        public event ManagedTask.TaskEndedEventHandler TaskEnded;

        public void Start(Action task, bool asSingleton = false)
        {
            Start(null, task, asSingleton);
        }
        public void Start(string id, Action task, bool asSingleton = false)
        {
            Start(new ManagedTask(id, task), asSingleton);
        }

        public void Start(Func<Task> task, bool asSingleton = false)
        {
            Start(null, task, asSingleton);
        }
        public void Start(string id, Func<Task> task, bool asSingleton = false)
        {
            Start(new ManagedTask(id, task), asSingleton);
        }

        private void Start(ManagedTask task, bool asSingleton)
        {
            if (asSingleton)
                task = new SingletonTask(task.Id, task.Run, task.RunAsync);
            task = new BackgroundTask(task.Id, task.Run, task.RunAsync);
            task.TaskStarting += (tid, rid, started, reason) => { TaskStarting?.Invoke(tid, rid, started, reason); };
            task.TaskEnded += (tid, rid, e) => { TaskEnded?.Invoke(tid, rid, e); };
            task.RunAsync().Wait(CancellationToken.None);
        }
    }
}
