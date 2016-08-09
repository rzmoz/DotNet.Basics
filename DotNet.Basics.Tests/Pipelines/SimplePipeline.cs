﻿using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tests.Pipelines
{
    public class SimplePipeline : Pipeline<EventArgs<int>>
    {
        public SimplePipeline()
        {
            AddBlock().AddStep<IncrementArgsStep>();
        }
    }
}
