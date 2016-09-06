using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTaskRunner : ManagedTaskRunner
    {
        public void Start(Action<string> task, bool asSingleton = false)
        {
            Start(null, task, asSingleton);
        }
        public void Start(string id, Action<string> task, bool asSingleton = false)
        {
            var mTtask = new ManagedTask(id, task);
            if (asSingleton)
                mTtask = new SingletonTask(mTtask.Id, (Action<string>)mTtask.Run);
            mTtask = new BackgroundTask(mTtask.Id, (Action<string>)mTtask.Run);
            Run(mTtask);
        }

        public void Start(Func<string, Task> task, bool asSingleton = false)
        {
            Start(null, task, asSingleton);
        }
        public void Start(string id, Func<string, Task> task, bool asSingleton = false)
        {
            var mTtask = new ManagedTask(id, task);
            if (asSingleton)
                mTtask = new SingletonTask(mTtask.Id, mTtask.RunAsync);
            mTtask = new BackgroundTask(mTtask.Id, mTtask.RunAsync);
            RunAsync(mTtask).Wait(CancellationToken.None);
        }

    }
}
