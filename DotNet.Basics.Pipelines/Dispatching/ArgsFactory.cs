using System;
using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace DotNet.Basics.Pipelines.Dispatching
{
    public class ArgsFactory
    {
        public object Create(ManagedTask pipeline, string argsString = null, bool isDebug = false)
        {
            var pipelineType = pipeline.GetType();
            var argsContainingType = pipelineType;
            while (argsContainingType.GetGenericArguments().Any() == false)
                argsContainingType = argsContainingType.BaseType;

            var argsType = argsContainingType.GetGenericArguments().Single();
            if (isDebug)
                Log.Debug($"Resolving args for : {pipelineType.FullName}<{argsType.Name}>");

            var args = Activator.CreateInstance(argsType);

            var argsParams = argsString == null ? new string[0] : argsString.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var argsParam in argsParams)
            {
                string argsJson;

                if (argsParam.ToFile().Exists())
                {
                    if (isDebug)
                        Log.Debug($"args found at {argsParam.ToFile().FullName()}");
                    argsJson = argsParam.ToFile().ReadAllText(IfNotExists.Mute);
                }
                else
                {
                    argsJson = argsParam;
                }

                if (argsJson != null)
                {
                    if (isDebug)
                        Log.Debug($"Populating args with: {argsJson}");
                    JsonConvert.PopulateObject(argsJson, args);
                }
            }
            return args;
        }
    }
}
