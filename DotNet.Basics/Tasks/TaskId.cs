using System.Security.Cryptography;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public class TaskId : ITaskId
    {
        public TaskId(string name = null)
        {
            Name = name ?? string.Empty;
            Id = Name.ToLower().ToHash(new SHA512Managed());
        }

        public string Id { get; }
        public string Name { get; }

        public override string ToString()
        {
            return $"{Name}:{Id}";
        }
    }
}
