﻿using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tests.Pipelines
{
    public class IncrementArgsStep : PipelineStep<TEventArgs<int>>
    {
        private readonly ClassThatIncrementArgsDependOn _classThatIncrementArgsDependOn;

        public IncrementArgsStep(ClassThatIncrementArgsDependOn classThatIncrementArgsDependOn)
        {
            _classThatIncrementArgsDependOn = classThatIncrementArgsDependOn;
            DisplayName = "MyIncrementArgsStep";
        }

        public override async Task RunAsync(TEventArgs<int> args, IDiagnostics logger)
        {
            await Task.Delay(1.MilliSeconds()).ConfigureAwait(false);//silence compiler warning
            args.Value = _classThatIncrementArgsDependOn.IncrementByOne(args.Value);
            logger.Log($"Value is now: {args.Value}");
        }
    }
}
