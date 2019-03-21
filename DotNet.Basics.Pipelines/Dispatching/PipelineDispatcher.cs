using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace DotNet.Basics.Pipelines.Dispatching
{
    public class PipelineDispatcher
    {
        private readonly IDictionary<string, PipelineDispatchInfo> _pipelines = new Dictionary<string, PipelineDispatchInfo>();

        public async Task RunAsync(string pipelineName, string argsString, CancellationToken ct = default(CancellationToken))
        {
            if (pipelineName == null)
            {
                Log.Error($"Pipeline name not set");
                return;
            }

            Log.Information("Dispatching Pipeline: {Pipeline}", pipelineName);
            if (_pipelines.ContainsKey(pipelineName.ToLowerInvariant()) == false)
            {
                Log.Error($"Pipeline not found: {pipelineName }");
                return;
            }

            Log.Debug($"Pipeline found: {pipelineName }");
            var pipeline = _pipelines[pipelineName.ToLowerInvariant()].Pipeline;
            var pipelineType = pipeline.GetType();
            var args = GetArgsInstance(pipeline, argsString);
            var runAsyncMethodInfo = pipelineType.GetMethods().FirstOrDefault(methodInfo => methodInfo.Name == "RunAsync" && methodInfo.GetParameters().Length == 2);
            LogPipelineStartingInfo(pipelineName, args);
            await ((Task)runAsyncMethodInfo.Invoke(pipeline, new[] { (object)args, CancellationToken.None })).ConfigureAwait(false);
        }

        public PipelineDispatcher Build(Type rootType)
        {

            var rootNamespace = rootType.Assembly.GetName().Name;
            var rootProbingPath = rootType.Assembly.Location.ToFile().Directory();

            var pipelinesRootDir = rootProbingPath.ToDir("Extensions");
            var pipelinesIndicator = "Pipelines.";
#if DEBUG
            Log.Information("Loading pipelines in {Namespace} from {PipelinesLocation}", rootNamespace, pipelinesRootDir);
#endif
            var pipelineTypes = pipelinesRootDir.EnumerateFiles($"{rootNamespace.TrimEnd('.')}.{pipelinesIndicator}*.dll", SearchOption.AllDirectories)
                .Select(assemblyFile =>
                {
#if DEBUG
                    Log.Information("Loading {AssemblyName}", assemblyFile.FullName());
#endif
                    return Assembly.LoadFrom(assemblyFile.FullName());
                })
                .SelectMany(assembly => assembly.GetTypesOf(typeof(Pipeline<>))
                    .Where(pipelineType => pipelineType.IsAbstract == false));

            _pipelines.Clear();

            foreach (var pipelineType in pipelineTypes)
            {
                if (pipelineType.Namespace.StartsWith($"{rootNamespace}.") == false)
                {
#if DEBUG
                    Log.Debug($"Pipeline {pipelineType.Name} not in search namespace: {rootNamespace}. Skipping...");
#endif
                    continue;
                }

                var pipelineName = pipelineType.FullName.Substring(pipelineType.FullName.IndexOf(pipelinesIndicator, StringComparison.OrdinalIgnoreCase) + pipelinesIndicator.Length).RemoveSuffix("Pipeline");
                var ctor = pipelineType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0);

                if (ctor == null)
                {
#if DEBUG
                    Log.Fatal($"No matching Ctor found on {pipelineType.FullName}. Looking for either Ctor() or Ctor(Func<IServiceProvider>");
#endif
                    return this;
                }

                var pipeline = (ManagedTask)ctor.Invoke(new object[] { });

                pipeline.Started += name => Log.Verbose($"{name} started");
                pipeline.Ended += (name, e) => Log.Verbose($"{name} ended {e}");
                var args = GetArgsInstance(pipeline);
                var pipelineInfo = new PipelineDispatchInfo(pipelineName, pipeline, JsonConvert.SerializeObject(args));

                _pipelines.Add(pipelineInfo.Key, pipelineInfo);
            }
#if DEBUG
            Log.Debug($"Available pipelines:");
            foreach (var p in _pipelines.Values.OrderBy(p => p.Key))
            {
                Log.Debug($"    {{Name}} --args=\"{p.ArgsInfo}\"", p.Name);
            }

            Log.Debug(@"  USAGE:");
            Log.Debug(@"    name:   Name of pipeline");
            Log.Debug(@"    args:   | (pipe) separated paths to json files or raw json containing pipeline args");
#endif
            return this;
        }

        private void LogPipelineStartingInfo(string pipelineName, object args)
        {
            Log.Debug($"Starting Pipeline: {pipelineName}");
            var jsonResolver = new DirPathContractResolver();
            var jsonSettings = new JsonSerializerSettings { ContractResolver = jsonResolver };
            Log.Debug($"With args:\r\n{JsonConvert.SerializeObject(args, Formatting.Indented, jsonSettings)}");
        }

        private object GetArgsInstance(ManagedTask pipeline, string argsString = null)
        {
            var pipelineType = pipeline.GetType();
            var argsContainingType = pipelineType;
            while (argsContainingType.GetGenericArguments().Any() == false)
                argsContainingType = argsContainingType.BaseType;

            var argsType = argsContainingType.GetGenericArguments().Single();
            Log.Debug($"Resolving args for : {pipelineType.FullName}<{argsType.Name}>");

            var args = Activator.CreateInstance(argsType);

            var argsParams = argsString == null ? new string[0] : argsString.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var argsParam in argsParams)
            {
                string argsJson;

                if (argsParam.ToFile().Exists())
                {
                    Log.Debug($"args found at {argsParam.ToFile().FullName()}");
                    argsJson = argsParam.ToFile().ReadAllText(IfNotExists.Mute);
                }
                else
                {
                    argsJson = argsParam;
                }

                if (argsJson != null)
                {
                    Log.Debug($"Populating args with: {argsJson}");
                    JsonConvert.PopulateObject(argsJson, args);
                }
            }
            return args;
        }
    }
}
