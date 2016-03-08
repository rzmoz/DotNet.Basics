using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public interface IAsyncTask : ITaskInfo
    {
        Task RunAsync();
    }
}
