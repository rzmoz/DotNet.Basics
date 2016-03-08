using System.Collections.Concurrent;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Pipelines
{
    public class StepBlockProfile : Profile
    {
        public StepBlockProfile(string name = null)
            : base(name)
        {
            StepProfiles = new ConcurrentQueue<Profile>();
        }

        public ConcurrentQueue<Profile> StepProfiles { get; private set; }
    }
}
