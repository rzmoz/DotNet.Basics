using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public static class RepeaterTaskExtensions
    {
        public static async Task<bool> NowAsync(this RepeaterTask task)
        {
            try
            {
                var runner = new RepeaterTaskRunner();
                return await runner.RunAsync(task).ConfigureAwait(false);
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }

        public static bool Now(this RepeaterTask task)
        {
            try
            {
                var runner = new RepeaterTaskRunner();
                var asyncTask = runner.RunAsync(task);
                asyncTask.Wait();
                return asyncTask.Result;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }
    }
}
