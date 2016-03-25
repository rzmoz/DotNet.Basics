using DotNet.Basics.Collections;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public class TaskInfo : TaskId, ITaskInfo
    {
        public TaskInfo(string name = null, params StringKeyValue[] metadata)
            : base(name)
        {
            Metadata = new StringDictionary(metadata, KeyNotFoundMode.ReturnNull);
        }

        public StringDictionary Metadata { get; }
    }
}
