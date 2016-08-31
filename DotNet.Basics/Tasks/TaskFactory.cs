using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class TaskFactory
    {
        public RunTask<T> Create<T>(Action task, T options = null, string id = null) where T : TaskOptions, new()
        {
            return new SyncTask<T>(task, options, id);
        }
        
        public RunTask<T> Create<T>(Func<CancellationToken, Task> task, T options = null, string id = null) where T : TaskOptions, new()
        {
            return new AsyncTask<T>(task, options, id);
        }
    }
}
