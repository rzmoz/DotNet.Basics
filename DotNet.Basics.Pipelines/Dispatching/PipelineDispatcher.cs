using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace DotNet.Basics.Pipelines.Dispatching
{
    public class PipelineDispatcher
    {
        private readonly ArgsFactory _argsFactory = new ArgsFactory();

        public PipelineDispatcher(Dictionary<string, PipelineDispatchInfo> pipelines)
        {
            Pipelines = pipelines;
        }

        public IReadOnlyDictionary<string, PipelineDispatchInfo> Pipelines { get; }

        public async Task RunAsync(string pipelineName, string argsString, CancellationToken ct = default(CancellationToken))
        {
            if (pipelineName == null)
            {
                Log.Error($"Pipeline name not set");
                return;
            }

            Log.Information("Dispatching Pipeline: {Pipeline}", pipelineName);
            if (Pipelines.ContainsKey(pipelineName.ToLowerInvariant()) == false)
            {
                Log.Error($"Pipeline not found: {pipelineName }");
                return;
            }

            Log.Debug($"Pipeline found: {pipelineName }");
            var pipeline = Pipelines[pipelineName.ToLowerInvariant()].Pipeline;
            var pipelineType = pipeline.GetType();
            var args = _argsFactory.Create(pipeline, argsString);
            var runAsyncMethodInfo = pipelineType.GetMethods().FirstOrDefault(methodInfo => methodInfo.Name == "RunAsync" && methodInfo.GetParameters().Length == 2);
            LogPipelineStartingInfo(pipelineName, args);
            await ((Task)runAsyncMethodInfo.Invoke(pipeline, new[] { (object)args, CancellationToken.None })).ConfigureAwait(false);
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
