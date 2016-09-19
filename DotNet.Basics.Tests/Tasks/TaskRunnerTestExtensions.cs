using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using FluentAssertions;

namespace DotNet.Basics.Tests.Tasks
{
    internal static class TaskRunnerTestExtensions
    {
        public static async Task TillTaskIsFinished(this TaskRunner runner, string taskId)
        {
            while (runner.IsRunning(taskId))
                await Task.Delay(100.Milliseconds()).ConfigureAwait(false);
        }
    }
}
