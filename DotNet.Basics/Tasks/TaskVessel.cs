namespace DotNet.Basics.Tasks
{
    public class TaskVessel<T> where T : class
    {
        public TaskVessel(ITask task, T options = null)
        {
            Task = task;
            Options = options;
        }

        public ITask Task { get; }
        public T Options { get; }
    }
}
