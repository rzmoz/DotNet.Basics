using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ILogger = DotNet.Basics.Diagnostics.ILogger;

namespace DotNet.Basics.Pipelines.Dispatching
{
    public class PipelineDispatcher : ILogger
    {
        private readonly ArgsFactory _argsFactory = new ArgsFactory();
        public event LogDispatcher.MessageLoggedEventHandler MessageLogged;
        public event ManagedTask.TaskStartedEventHandler PipelineStarted;
        public event ManagedTask.TaskEndedEventHandler PipelineEnded;

        public PipelineDispatcher(Dictionary<string, PipelineDispatchInfo> pipelines)
        {
            Pipelines = pipelines ?? new Dictionary<string, PipelineDispatchInfo>();
        }

        public IReadOnlyDictionary<string, PipelineDispatchInfo> Pipelines { get; }

        public async Task<object> RunAsync(string pipelineName, params string[] argStrings)
        {
            if (pipelineName == null)
                throw new PipelineDispatchException($"Pipeline name not set");

            Log.Information($"Dispatching Pipeline: {pipelineName}");
            if (Pipelines.ContainsKey(pipelineName.ToLowerInvariant()) == false)
                throw new PipelineDispatchException($"Pipeline not found: {pipelineName }");

            var pipeline = Pipelines[pipelineName.ToLowerInvariant()].Pipeline;

            try
            {
                pipeline.MessageLogged += FireMessageLogged;
                pipeline.Started += FirePipelineStarted;
                pipeline.Ended += FirePipelineEnded;
                var pipelineType = pipeline.GetType();
                var args = _argsFactory.Create(pipeline, argStrings);
                var runAsyncMethodInfo = pipelineType.GetMethods().FirstOrDefault(methodInfo =>
                    methodInfo.Name == "RunAsync" && methodInfo.GetParameters().Length == 2);
                LogPipelineStartingInfo(pipelineName, args);
                var task = ((Task)runAsyncMethodInfo.Invoke(pipeline, new[] { (object)args, CancellationToken.None }));
                await task.ConfigureAwait(false);
                return (object)((dynamic)task).Result;
            }
            finally
            {
                pipeline.Ended -= FirePipelineEnded;
                pipeline.Started -= FirePipelineStarted;
                pipeline.MessageLogged -= FireMessageLogged;
            }
        }

        private void FireMessageLogged(LogLevel level, string message, Exception e)
        {
            MessageLogged?.Invoke(level, message, e);
        }
        private void FirePipelineStarted(string taskName)
        {
            PipelineStarted?.Invoke(taskName);
        }
        private void FirePipelineEnded(string taskName, Exception e)
        {
            PipelineEnded?.Invoke(taskName, e);
        }

        private void LogPipelineStartingInfo(string pipelineName, object args)
        {
            Log.Debug($"Starting Pipeline: {pipelineName}");
            var jsonResolver = new DirPathContractResolver();
            var jsonSettings = new JsonSerializerSettings { ContractResolver = jsonResolver };
            Log.Debug($"With args:\r\n{JsonConvert.SerializeObject(args, Formatting.Indented, jsonSettings)}");
        }
    }
}
