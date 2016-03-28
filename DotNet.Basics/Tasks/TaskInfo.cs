using DotNet.Basics.Collections;

namespace DotNet.Basics.Tasks
{
    public class TaskInfo : TaskId, ITaskInfo
    {
        public TaskInfo(string name = null, params StringPair[] metadata)
            : base(name)
        {
            Metadata = new StringDictionary(metadata, KeyMode.CaseInsensitive, KeyNotFoundMode.ReturnDefault);
        }

        public StringDictionary Metadata { get; }
    }
}
