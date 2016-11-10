using System.Collections.Generic;

namespace DotNet.Basics.Tasks
{
    public class TaskStartedEventArgs
    {
        public TaskStartedEventArgs(string name, IReadOnlyDictionary<string, string> taskProperties)
        {
            Name = name;
            TaskProperties = taskProperties;
        }

        public string Name { get; }
        public IReadOnlyDictionary<string, string> TaskProperties { get; }
    }
}
