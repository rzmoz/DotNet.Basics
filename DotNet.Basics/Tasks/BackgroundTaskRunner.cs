using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTaskRunner
    {
        public event ManagedTask.TaskStartingEventHandler Starting;
        public event ManagedTask.TaskEndedEventHandler Ended;

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
            task.Starting += (tid, rid, started, reason) => { Starting?.Invoke(tid, rid, started, reason); };
            task.Ended += (tid, rid, e) => { Ended?.Invoke(tid, rid, e); };
            task.Run();
        }
    }
}
