using System;
using System.Linq;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.IO;
using DotNet.Basics.Tasks;
using Newtonsoft.Json;

namespace DotNet.Basics.Pipelines.Dispatching
{
    public class ArgsFactory
    {
        public object Create(ManagedTask pipeline, params string[] argStrings)
        {
            var pipelineType = pipeline.GetType();
            var argsContainingType = pipelineType;
            while (argsContainingType.GetGenericArguments().Any() == false)
                argsContainingType = argsContainingType.BaseType;

            var argsType = argsContainingType.GetGenericArguments().Single();

            Log.Debug($"Resolving args for : {pipelineType.FullName}<{argsType.Name}>");

            var args = Activator.CreateInstance(argsType);

            foreach (var arg in argStrings)
            {
                string argsJson;

                if (arg.ToFile().Exists())
                {

                    Log.Debug($"args found at {arg.ToFile().FullName()}");
                    argsJson = arg.ToFile().ReadAllText(IfNotExists.Mute);
                }
                else
                {
                    argsJson = arg;
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
