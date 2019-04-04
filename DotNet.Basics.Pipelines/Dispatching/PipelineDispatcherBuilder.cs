using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using Newtonsoft.Json;

namespace DotNet.Basics.Pipelines.Dispatching
{
    public class PipelineDispatcherBuilder
    {
        private readonly ArgsFactory _argsFactory = new ArgsFactory();

        public PipelineDispatcherBuilder(string pipelineIndicator = "Pipelines")
        {
            PipelineIndicator = pipelineIndicator?.EnsureSuffix(".") ?? string.Empty;
        }

        public string PipelineIndicator { get; }
        public bool IsDebug { get; private set; }
        public string RootNamespace { get; private set; }

        public PipelineDispatcherBuilder WithDebug(bool isDebug = true)
        {
            IsDebug = isDebug;
            return this;
        }
        public PipelineDispatcherBuilder InNamespace(string rootNamespace)
        {
            RootNamespace = rootNamespace?.EnsureSuffix(".");
            return this;
        }

        public PipelineDispatcher Build(params Assembly[] assemblies)
        {
            var pipelineTypes = assemblies.SelectMany(assembly => assembly.GetTypesOf(typeof(Pipeline<>))
                .Where(pipelineType => pipelineType.IsAbstract == false));
            return Build(pipelineTypes.ToArray());
        }

        public PipelineDispatcher Build(params Type[] pipelineTypes)
        {
            var pipelines = new Dictionary<string, PipelineDispatchInfo>();

            foreach (var pipelineType in pipelineTypes)
            {
                if (RootNamespace != null && pipelineType.Namespace.EnsureSuffix(".").StartsWith($"{RootNamespace}") == false)
                {
                    if (IsDebug)
                        Log.Debug($"Pipeline {pipelineType.Name} not in search namespace: {RootNamespace}. Skipping...");
                    continue;
                }

                var pipelineName = pipelineType.FullName.RemoveSuffix("Pipeline");
                if (string.IsNullOrWhiteSpace(PipelineIndicator) == false && pipelineName.IndexOf(PipelineIndicator) >= 0)
                    pipelineName = pipelineName.Substring(pipelineType.FullName.IndexOf(PipelineIndicator, StringComparison.OrdinalIgnoreCase) + PipelineIndicator.Length);

                var ctor = pipelineType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0);

                if (ctor == null)
                {
                    if (IsDebug)
                        Log.Error($"No default Ctor() found on {pipelineType.FullName}. Skipping");
                    continue;
                }

                var pipeline = (ManagedTask)ctor.Invoke(new object[] { });
                var args = _argsFactory.Create(pipeline);
                var pipelineInfo = new PipelineDispatchInfo(pipelineName, pipeline, JsonConvert.SerializeObject(args));

                pipelines.Add(pipelineInfo.Key, pipelineInfo);
            }

            if (IsDebug)
            {
                Log.Debug($"Available pipelines:");
                foreach (var p in pipelines.Values.OrderBy(p => p.Key))
                    Log.Debug($"    {p.Name} --args=\"{p.ArgsInfo}\"");
            }

            return new PipelineDispatcher(pipelines);
        }
    }
}
