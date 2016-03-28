using System;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public class SyncTask : TaskInfo, ISyncTask
    {
        private readonly Action<StringDictionary> _action;
        
        public SyncTask(string name, Action<StringDictionary> action, params StringPair[] metadata) : base(name, metadata)
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
