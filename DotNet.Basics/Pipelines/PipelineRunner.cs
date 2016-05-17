using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Ioc;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Pipelines
{
    public class PipelineRunner
    {
        private const string _errorLoggerName = "ErrorLogger";
        private readonly ILogger _objectScopeLogger;
        private readonly IocContainer _container;

        public event PipelineEventHandler PipelineStarting;
        public event PipelineEventHandler PipelineEnded;
        public event PipelineEventHandler BlockStarting;
        public event PipelineEventHandler BlockEnded;
        public event PipelineEventHandler StepStarting;
        public event PipelineEventHandler StepEnded;

        public delegate void PipelineEventHandler(string name);

        public PipelineRunner()
            : this(null, null)
        {
        }

        public PipelineRunner(ILogger logger)
            : this(null, logger)
        {
        }

        public PipelineRunner(IocContainer container, ILogger objectScopeLogger = null)
        {
            _objectScopeLogger = objectScopeLogger;
            _container = container ?? new IocContainer();
        }

        public async Task<PipelineResult<TArgs>> RunAsync<TTaskPipeline, TArgs>(TArgs args = null, ILogger logger = null)
            where TTaskPipeline : Pipeline<TArgs>, new()
            where TArgs : EventArgs, new()
        {
            var pipeline = new TTaskPipeline();
            return await RunAsync<TArgs>(pipeline, args, logger);
        }


        public async Task<PipelineResult<TArgs>> RunAsync<TArgs>(Pipeline<TArgs> pipeline, TArgs args = null, ILogger logger = null)
            where TArgs : EventArgs, new()
        {
            if (args == null)
                args = new TArgs();
            MediatorLogger mediatorLogger = new MediatorLogger();
            //add object scope objectScopeLogger
            if (_objectScopeLogger != null)
                mediatorLogger.Add(_objectScopeLogger);
            //add run scope objectScopeLogger
            if (logger != null)
                mediatorLogger.Add(logger);
            //add error detector
            var errorLogger = new EventLogger();
            var success = true;
            errorLogger.EntryLogged += (e, logEntry) =>
            {
                if (logEntry.Level >= LogLevel.Error)
                    success = false;
            };

            mediatorLogger.Add(errorLogger, _errorLoggerName);
            var pipelineName = pipeline.GetType().Name;

            try
            {
                mediatorLogger.LogTrace($"Pipeline starting:{pipelineName }");
                PipelineStarting?.Invoke(pipelineName);

                var blockCount = 0;
                foreach (var block in pipeline.PipelineBlocks)
                {
                    var blockName = string.IsNullOrWhiteSpace(block.Name) ? $"Block {blockCount++}" : block.Name;
                    mediatorLogger.LogTrace($"Block starting:{blockName}");
                    BlockStarting?.Invoke(blockName);

                    await Task.WhenAll(block.Select(async step =>
                    {
                        step.Container = _container;
                        step.Init();//must run before resolving step name for lazy bound steps
                        var stepName = string.IsNullOrWhiteSpace(step.DisplayName) ? step.GetType().Name : step.DisplayName;

                        mediatorLogger.LogTrace($"Step starting:{stepName}", LogLevel.Debug);
                        StepStarting?.Invoke(stepName);

                        await step.RunAsync(args, mediatorLogger).ConfigureAwait(false);

                        mediatorLogger.LogTrace($"Step ended:{stepName}", LogLevel.Debug);
                        StepEnded?.Invoke(stepName);

                    })).ConfigureAwait(false);

                    mediatorLogger.LogTrace($"Block ended:{blockName}", LogLevel.Debug);
                    BlockEnded?.Invoke(blockName);
                }
            }
            catch (Exception exc)
            {
                mediatorLogger.LogError(exc.Message, exc);
            }
            finally
            {
                mediatorLogger.LogTrace($"Pipeline ended:{pipelineName}");
                PipelineEnded?.Invoke(pipelineName);
            }

            return new PipelineResult<TArgs>(success, args);
        }
    }
}
