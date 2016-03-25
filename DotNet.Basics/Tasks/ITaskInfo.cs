using DotNet.Basics.Collections;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public interface ITaskInfo : ITaskId
    {
        StringDictionary Metadata { get; }
    }
}
