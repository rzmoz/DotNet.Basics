using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Pipelines
{
    public class PipelineRunner
    {
        private readonly SimpleContainer _container;

        public event PipelineEventHandler PipelineStarting;
        public event PipelineEventHandler PipelineEnded;
        public event PipelineEventHandler BlockStarting;
        public event PipelineEventHandler BlockEnded;
        public event PipelineEventHandler StepStarting;
        public event PipelineEventHandler StepEnded;

        public delegate void PipelineEventHandler(string name);

        public PipelineRunner()
            : this(null)
        {
        }

        public PipelineRunner(SimpleContainer container)
        {
            _container = container ?? new SimpleContainer();
        }

        public async Task<PipelineResult<TArgs>> RunAsync<TTaskPipeline, TArgs>(TArgs args = null, CancellationToken ct = default(CancellationToken))
            where TTaskPipeline : Pipeline<TArgs>, new()
            where TArgs : EventArgs, new()
        {
            var pipeline = new TTaskPipeline();
            return await RunAsync<TArgs>(pipeline, args, ct);
        }

        public async Task<PipelineResult<TArgs>> RunAsync<TArgs>(Pipeline<TArgs> pipeline, TArgs args = null, CancellationToken ct = default(CancellationToken))
            where TArgs : EventArgs, new()
        {
            if (args == null)
                args = new TArgs();

            var pipelineName = pipeline.GetType().Name;
            Exception lastException = null;

            try
            {
                Trace.WriteLine($"Pipeline starting:{pipelineName }");
                PipelineStarting?.Invoke(pipelineName);

                var blockCount = 0;
                foreach (var block in pipeline.PipelineBlocks)
                {
                    var blockName = string.IsNullOrWhiteSpace(block.Name) ? $"Block {blockCount++}" : block.Name;
                    Trace.WriteLine($"Block starting:{blockName}");
                    BlockStarting?.Invoke(blockName);

                    await block.ParallelForEachAsync(async (step) =>
                    {
                        step.Container = _container;
                        step.Init();//must run before resolving step name for lazy bound steps
                        var stepName = string.IsNullOrWhiteSpace(step.DisplayName) ? step.GetType().Name : step.DisplayName;

                        Trace.WriteLine($"Step starting:{stepName}");
                        StepStarting?.Invoke(stepName);

                        await step.RunAsync(args, ct).ConfigureAwait(false);

                        Trace.WriteLine($"Step ended:{stepName}");
                        StepEnded?.Invoke(stepName);
                    }).ConfigureAwait(false);

                    Trace.WriteLine($"Block ended:{blockName}");
                    BlockEnded?.Invoke(blockName);
                }
            }
            catch (Exception exc)
            {
                lastException = exc;
                Trace.WriteLine(exc.ToString());
            }
            finally
            {
                Trace.WriteLine($"Pipeline ended:{pipelineName}");
                PipelineEnded?.Invoke(pipelineName);
            }

            return new PipelineResult<TArgs>(lastException, args);
        }
    }
}
