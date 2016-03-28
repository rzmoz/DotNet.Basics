using System;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public class AsyncTask : TaskInfo, IAsyncTask
    {
        private readonly Func<StringDictionary, Task> _action;
        
        public AsyncTask(string name, Action<StringDictionary> action, params StringPair[] metadata) : base(name, metadata)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = async md => await Task.Factory.StartNew(() => action.Invoke(md));
        }

        public AsyncTask(string name, Func<StringDictionary, Task> action, params StringPair[] metadata) : base(name, metadata)
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
