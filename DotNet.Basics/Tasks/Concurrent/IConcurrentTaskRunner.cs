using System;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks.Concurrent
{
    public interface IConcurrentTaskRunner : IConcurrentTaskInfoReader
    {
        bool IsLocked(string taskName);
        StartResult RunAtMostOnceInBackground(string taskName, Action<StringDictionary> action);
        StartResult RunAtMostOnceInBackground(string taskName, Func<StringDictionary, Task> action);
        StartResult RunAtMostOnceInBackground(IAsyncTask task);

        StartResult EraseIfNotRunning(string taskName);
        StartResult EraseIfNotRunning(ITaskId taskId);

        void ForceReleaseLock(string taskName);
    }
}
