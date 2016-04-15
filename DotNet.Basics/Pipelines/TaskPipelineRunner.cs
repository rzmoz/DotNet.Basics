using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Pipelines
{
    public class TaskPipelineRunner
    {
        private readonly IIocContainer _container;

        public event PipelineEventHandler PipelineStarting;
        public event PipelineEventHandler PipelineEnded;
        public event PipelineEventHandler BlockStarting;
        public event PipelineEventHandler BlockEnded;
        public event PipelineEventHandler StepStarting;
        public event PipelineEventHandler StepEnded;

        public delegate void PipelineEventHandler(string name, PipelineType pipelineType);

        public TaskPipelineRunner()
            : this(null)
        {
        }

        public TaskPipelineRunner(IIocContainer container)
        {
            _container = container ?? new IocContainer();
        }

        public async Task<TaskPipelineResult<TArgs>> RunAsync<TTaskPipeline, TArgs>(TArgs args = null, IDiagnostics logger = null)
            where TTaskPipeline : TaskPipeline<TArgs>, new()
            where TArgs : EventArgs, new()
        {
            var pipeline = new TTaskPipeline();
            return await RunAsync<TArgs>(pipeline, args, logger);
        }


        public async Task<TaskPipelineResult<TArgs>> RunAsync<TArgs>(TaskPipeline<TArgs> pipeline, TArgs args = null, IDiagnostics logger = null)
            where TArgs : EventArgs, new()
        {
            if (args == null)
                args = new TArgs();

            if (logger == null)
                logger = new NullDiagnostics();

            var pipelineName = pipeline.GetType().Name;

            try
            {
                logger.Log($"Pipeline starting:{pipelineName }", LogLevel.Debug);
                PipelineStarting?.Invoke(pipelineName, PipelineType.Pipeline);

                var blockCount = 0;
                foreach (var block in pipeline.StepBlocks)
                {
                    var blockName = string.IsNullOrWhiteSpace(block.Name) ? $"TaskBlock {blockCount++}" : block.Name;
                    logger.Log($"Block starting:{blockName}", LogLevel.Debug);
                    BlockStarting?.Invoke(blockName, PipelineType.Block);

                    await Task.WhenAll(block.Select(async step =>
                    {
                        var stepName = string.IsNullOrWhiteSpace(step.DisplayName) ? step.GetType().Name : step.DisplayName;
                        logger.Log($"Step starting:{stepName}", LogLevel.Debug);
                        StepStarting?.Invoke(stepName, PipelineType.Step);

                        step.Container = _container;
                        step.Init();

                        await step.RunAsync(args, logger).ConfigureAwait(false);

                        logger.Log($"Step ended:{stepName}", LogLevel.Debug);
                        StepEnded?.Invoke(stepName, PipelineType.Step);

                    })).ConfigureAwait(false);

                    logger.Log($"Block ended:{blockName}", LogLevel.Debug);
                    BlockEnded?.Invoke(blockName, PipelineType.Block);
                }
            }
            catch (Exception exc)
            {
                logger.Log(exc.Message, LogLevel.Error, exc);
            }
            finally
            {
                logger.Log($"Pipeline ended:{pipelineName}", LogLevel.Debug);
                BlockEnded?.Invoke(pipelineName, PipelineType.Pipeline);
            }

            return new TaskPipelineResult<TArgs>(args, null);
        }

        private void TryStartProfile(Profile profile, IDiagnostics logger, ProfileEventHandler profileEventHandler)
        {
            if (profile == null)
                return;
            logger.Profile(profile.Name, profile.Duration);
            profileEventHandler?.Invoke(profile);
        }

        private void TryStopProfile(Profile profile, IDiagnostics logger, ProfileEventHandler profileEventHandler)
        {
            if (profile == null)
                return;
            profile.Stop();
            logger.Profile(profile.Name, profile.Duration);
            profileEventHandler?.Invoke(profile);
        }
    }
}
