using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Pipelines
{
    public class TaskPipelineRunner
    {
        private readonly IDotNetContainer _container;

        public event ProfileEventHandler PipelineStarting;
        public event ProfileEventHandler PipelineEnded;
        public event ProfileEventHandler BlockStarting;
        public event ProfileEventHandler BlockEnded;
        public event ProfileEventHandler StepStarting;
        public event ProfileEventHandler StepEnded;

        public delegate void ProfileEventHandler(Profile profile);

        public TaskPipelineRunner(IDotNetContainer container = null)
        {
            _container = container ?? new DotNetContainer();
        }

        public bool IsRunning { get; private set; }

        public static async Task<TaskPipelineResult<T>> RunAsync<T>(Action<TaskPipeline<T>> pipelineActions, T args = null, IDiagnostics logger = null, IDotNetContainer container = null) where T : EventArgs, new()
        {
            var validatePipeline = new TaskPipeline<T>();
            pipelineActions(validatePipeline);

            var runner = new TaskPipelineRunner(container);
            var result = await runner.RunAsync(validatePipeline, args, logger).ConfigureAwait(false);
            return result;
        }

        public async Task<TaskPipelineResult<T>> RunAsync<TTaskPipeline, T>(T args = null, IDiagnostics logger = null)
            where TTaskPipeline : TaskPipeline<T>, new()
            where T : EventArgs, new()
        {
            var pipeline = new TTaskPipeline();
            return await RunAsync<T>(pipeline, args, logger);
        }

        public async Task<TaskPipelineResult<T>> RunAsync<T>(TaskPipeline<T> pipeline, T args = null, IDiagnostics logger = null)
            where T : EventArgs, new()
        {
            if (args == null)
                args = new T();

            if (IsRunning)
                return null;

            var pipelineProfile = new TaskPipelineProfile(pipeline.GetType().Name);
            if (logger == null)
                logger = new VoidDiagnostics();
            try
            {
                StartProfile(pipelineProfile, logger, PipelineStarting);

                var blockCount = 0;
                foreach (var block in pipeline.StepBlocks)
                {
                    var blockName = string.IsNullOrWhiteSpace(block.Name) ? $"TaskBlock {blockCount++}" : block.Name;
                    var blockProfile = new StepBlockProfile(blockName);
                    pipelineProfile.BlockProfiles.Enqueue(blockProfile);
                    StartProfile(blockProfile, logger, BlockStarting);

                    await Task.WhenAll(block.Select(async step =>
                    {
                        step.Container = _container;
                        step.Init();

                        var stepProfile = new Profile
                        {
                            Name = string.IsNullOrWhiteSpace(step.DisplayName) ? step.GetType().Name : step.DisplayName
                        };
                        blockProfile.StepProfiles.Enqueue(stepProfile);
                        StartProfile(stepProfile, logger, StepStarting);

                        await step.RunAsync(args, logger).ConfigureAwait(false);

                        StopProfile(stepProfile, logger, StepEnded);
                    })).ConfigureAwait(false);

                    StopProfile(blockProfile, logger, BlockEnded);
                }
            }
            catch (Exception exc)
            {
                logger.Log(exc.Message, LogLevel.Error, exc);
            }
            finally
            {
                StopProfile(pipelineProfile, logger, PipelineEnded);
            }

            return new TaskPipelineResult<T>(args, pipelineProfile);
        }

        private void StartProfile(Profile profile, IDiagnostics logger, ProfileEventHandler profileEventHandler)
        {
            profile.Start();
            logger.Profile(profile.Name, profile.Duration);
            profileEventHandler?.Invoke(profile);
        }

        private void StopProfile(Profile profile, IDiagnostics logger, ProfileEventHandler profileEventHandler)
        {
            profile.Stop();
            logger.Profile(profile.Name, profile.Duration);
            profileEventHandler?.Invoke(profile);
        }
    }
}
