using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public class SyncTask : TaskInfo, ISyncTask
    {
        private readonly Action<KeyValueCollection> _action;
        
        public SyncTask(string name, Action<KeyValueCollection> action, params KeyValue[] metadata) : base(name, metadata)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _action = action;
        }

        public void Run()
        {
            try
            {
                _action.Invoke(Metadata);
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }
    }
}
