using System;

namespace CSharp.Basics.Sys.RepeaterTasks
{
    public class RepeaterSyncTask : RepeaterTask
    {
        public RepeaterSyncTask(Action action)
            : base(action)
        {
        }

        public bool Now()
        {
            var runTask = RunAsync();
            runTask.Wait();
            return runTask.Result;
        }
    }
}
