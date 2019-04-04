using System;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines.Dispatching
{
    public class PipelineDispatchInfo
    {
        public PipelineDispatchInfo(string name, ManagedTask pipeline, string argsInfo = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            ArgsInfo = argsInfo ?? "{}";
        }

        public string Name { get; }
        public string Key => Name.ToLowerInvariant();
        public ManagedTask Pipeline { get; }
        public string ArgsInfo { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
