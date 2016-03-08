using System;

namespace DotNet.Basics.Tasks.Repeating
{
    public class RepeaterSyncTask : RepeaterTask
    {
        public RepeaterSyncTask(Action action)
            : base(action)
        {
        }

        public bool Now()
        {
            try
            {
                var runTask = RunAsync();
                runTask.Wait();
                return runTask.Result;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }
    }
}
