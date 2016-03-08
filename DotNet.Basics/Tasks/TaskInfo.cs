using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public class TaskInfo : TaskId, ITaskInfo
    {
        public TaskInfo(string name = null, params KeyValue[] metadata)
            : base(name)
        {
            Metadata = new KeyValueCollection(metadata);
        }

        public KeyValueCollection Metadata { get; }
    }
}
