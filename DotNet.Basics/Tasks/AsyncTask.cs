using System;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public class AsyncTask : TaskInfo, IAsyncTask
    {
        private readonly Func<KeyValueCollection, Task> _action;
        
        public AsyncTask(string name, Action<KeyValueCollection> action, params KeyValue[] metadata) : base(name, metadata)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = async md => await Task.Factory.StartNew(() => action.Invoke(md));
        }

        public AsyncTask(string name, Func<KeyValueCollection, Task> action, params KeyValue[] metadata) : base(name, metadata)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = action;
        }
        
        public async Task RunAsync()
        {
            try
            {
                await _action.Invoke(Metadata).ConfigureAwait(false);
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }
    }
}
