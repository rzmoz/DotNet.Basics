using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTaskFactory
    {
        public T Create<T>(string id = null) where T : ManagedTask
        {
            return Create<T>(new ManagedTask(id, rid => { }));
        }

        public T Create<T>(Action<string> syncTask, Func<string, TaskEndedReason> preconditionsMet = null) where T : ManagedTask
        {
            return Create<T>(null, syncTask, preconditionsMet);
        }

        public T Create<T>(Func<string, Task> asyncTask, Func<string, TaskEndedReason> preconditionsMet = null) where T : ManagedTask
        {
            return Create<T>(null, asyncTask, preconditionsMet);
        }
        public T Create<T>(string id, Action<string> syncTask, Func<string, TaskEndedReason> preconditionsMet = null) where T : ManagedTask
        {
            return Create<T>(new ManagedTask(id, syncTask, preconditionsMet));
        }

        public T Create<T>(string id, Func<string, Task> asyncTask, Func<string, TaskEndedReason> preconditionsMet = null) where T : ManagedTask
        {
            return Create<T>(new ManagedTask(id, asyncTask, preconditionsMet));
        }

        public T Create<T>(ManagedTask task) where T : ManagedTask
        {
            var createType = typeof(T);

            if (createType == typeof(RepeaterTask))
                return (T)(object)new RepeaterTask(task);
            if (createType == typeof(BackgroundTask))
                return (T)(object)new BackgroundTask(task);
            if (createType == typeof(SingletonTask))
                return (T)(object)new SingletonTask(task);
            if (createType == typeof(OnceOnlyTask))
                return (T)(object)new OnceOnlyTask(task);

            if (createType == typeof(ManagedTask))
                return (T)(object)task;
            throw new ArgumentException($"Type not supported: {createType.FullName}");
        }
    }
}
