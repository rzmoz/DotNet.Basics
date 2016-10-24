using System.Collections.Generic;

namespace DotNet.Basics.Tasks
{
    public class TaskStartedEventArgs
    {
        public TaskStartedEventArgs(string name, string taskType, IReadOnlyDictionary<string, string> taskProperties)
        {
            Name = name;
            TaskType = taskType;
            TaskProperties = taskProperties;
        }

        public string Name { get; }
        public string TaskType { get; }
        public IReadOnlyDictionary<string, string> TaskProperties { get; }
    }
}
