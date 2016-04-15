using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Pipelines
{
    public class PipelineRunner
    {
        private readonly IocContainer _container;
        private readonly MediatorDiagnostics _diagnostics;

        public event PipelineEventHandler PipelineStarting;
        public event PipelineEventHandler PipelineEnded;
        public event PipelineEventHandler BlockStarting;
        public event PipelineEventHandler BlockEnded;
        public event PipelineEventHandler StepStarting;
        public event PipelineEventHandler StepEnded;

        public delegate void PipelineEventHandler(string name, PipelineType pipelineType);

        public PipelineRunner()
            : this(null, null)
        {
        }

        public PipelineRunner(IocContainer container)
            : this(container, null)
        {
        }

        public PipelineRunner(IDiagnostics logger)
            : this(null, logger)
        {
        }

        public PipelineRunner(IocContainer container, IDiagnostics logger)
        {
            _container = container ?? new IocContainer();
            _diagnostics = new MediatorDiagnostics();
            if (logger != null)
                _diagnostics.Add(Guid.NewGuid().ToString(), logger);
        }

        public async Task<PipelineResult<TArgs>> RunAsync<TTaskPipeline, TArgs>(TArgs args = null, IDiagnostics logger = null)
            where TTaskPipeline : Pipeline<TArgs>, new()
            where TArgs : EventArgs, new()
        {
            var pipeline = new TTaskPipeline();
            return await RunAsync<TArgs>(pipeline, args, logger);
        }


        public async Task<PipelineResult<TArgs>> RunAsync<TArgs>(Pipeline<TArgs> pipeline, TArgs args = null, IDiagnostics logger = null)
            where TArgs : EventArgs, new()
        {
            if (args == null)
                args = new TArgs();

            if (logger != null)
                _diagnostics.Add(Guid.NewGuid().ToString(), logger);

            var pipelineName = pipeline.GetType().Name;

            try
            {
                _diagnostics.Log($"Pipeline starting:{pipelineName }", LogLevel.Debug);
                PipelineStarting?.Invoke(pipelineName, PipelineType.Pipeline);

                var blockCount = 0;
                foreach (var block in pipeline.PipelineBlocks)
                {
                    var blockName = string.IsNullOrWhiteSpace(block.Name) ? $"Block {blockCount++}" : block.Name;
                    _diagnostics.Log($"Block starting:{blockName}", LogLevel.Debug);
                    BlockStarting?.Invoke(blockName, PipelineType.Block);

                    await Task.WhenAll(block.Select(async step =>
                    {
                        step.Container = _container;
                        step.Init();//must run before resolving step name for lazy bound steps
                        var stepName = string.IsNullOrWhiteSpace(step.DisplayName) ? step.GetType().Name : step.DisplayName;

                        _diagnostics.Log($"Step starting:{stepName}", LogLevel.Debug);
                        StepStarting?.Invoke(stepName, PipelineType.Step);

                        await step.RunAsync(args, _diagnostics).ConfigureAwait(false);

                        _diagnostics.Log($"Step ended:{stepName}", LogLevel.Debug);
                        StepEnded?.Invoke(stepName, PipelineType.Step);

                    })).ConfigureAwait(false);

                    _diagnostics.Log($"Block ended:{blockName}", LogLevel.Debug);
                    BlockEnded?.Invoke(blockName, PipelineType.Block);
                }
            }
            catch (Exception exc)
            {
                _diagnostics.Log(exc.Message, LogLevel.Error, exc);
            }
            finally
            {
                _diagnostics.Log($"Pipeline ended:{pipelineName}", LogLevel.Debug);
                PipelineEnded?.Invoke(pipelineName, PipelineType.Pipeline);
            }

            return new PipelineResult<TArgs>(args);
        }
    }
}
