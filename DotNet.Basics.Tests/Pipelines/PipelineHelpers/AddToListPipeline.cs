using DotNet.Basics.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class AddToListPipeline : Pipeline<List<int>>
    {
        public static int StepCount { get; } = 7;
        public static IReadOnlyList<int> ItemsRange { get; } = Enumerable.Range(1, StepCount).Reverse().ToList();
        public AddToListPipeline(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            foreach (var i in ItemsRange)
                AddStep<AddToListStep>(i.ToString());//add reversed delay to see if tasks are properly awaited
        }

    }
}
