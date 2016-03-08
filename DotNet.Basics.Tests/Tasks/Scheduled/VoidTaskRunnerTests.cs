using System;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Scheduling;

namespace DotNet.Basics.Tests.Tasks.Scheduled
{
    public class VoidTaskRunnerTests : ScheduledTaskRunner
    {
        public VoidTaskRunnerTests(ISyncTask task, TimeSpan interval) : base(task, interval)
        {
        }

        protected override void Run(object state)
        {
            //do nothing
        }
    }
}
