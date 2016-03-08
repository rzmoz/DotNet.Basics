using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public class RepeaterAsyncTask : RepeaterTask
    {
        public RepeaterAsyncTask(Func<Task> action)
            : base(action)
        {
        }

        public async Task<bool> NowAsync()
        {
            try
            {
                return await RunAsync().ConfigureAwait(false);
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }
    }
}
