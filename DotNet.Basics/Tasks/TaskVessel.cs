namespace DotNet.Basics.Tasks
{
    public class TaskVessel<T> where T : TaskOptions, new()
    {
        public TaskVessel(ITask task)
        {
            Task = task;
        }

        public TaskVessel(ITask task, T options)
            :this(task)
        {
            Options = options;
        }

        public ITask Task { get; }
        public string TaskName { get; set; }
        public T Options { get; set; }

        /// <summary>
        /// Task will run as singleton if set
        /// </summary>
        public string SingletonId { get; set; }
    }
}
