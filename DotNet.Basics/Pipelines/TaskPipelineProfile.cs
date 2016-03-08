using System.Collections.Concurrent;
using DotNet.Basics.Diagnostics;


namespace DotNet.Basics.Pipelines
{
    public class TaskPipelineProfile : Profile
    {
        public TaskPipelineProfile(string name = null)
            : base(name)
        {
            BlockProfiles = new ConcurrentQueue<StepBlockProfile>();
        }

        public ConcurrentQueue<StepBlockProfile> BlockProfiles { get; private set; }
    }
}
