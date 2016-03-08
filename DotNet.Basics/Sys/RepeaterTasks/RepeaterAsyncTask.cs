using System;
using System.Threading.Tasks;

namespace CSharp.Basics.Sys.RepeaterTasks
{
    public class RepeaterAsyncTask : RepeaterTask
    {
        public RepeaterAsyncTask(Func<Task> action)
            : base(action)
        {
        }

        public async Task<bool> NowAsync()
        {
            return await RunAsync().ConfigureAwait(false);
        }
    }
}
