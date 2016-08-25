using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class AtMostOnceTask
    {
        public AtMostOnceTask(string id, Func<Task> action)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            Id = id;
            Action = action ?? Void;
        }

        public string Id { get; }
        public Func<Task> Action { get; }

        private Task Void()
        {
            return Task.FromResult("");
        }
    }
}
