using System;
using System.Threading.Tasks;

namespace CSharp.Basics.Sys.RepeaterTasks
{
    public class AwaitTask
    {
        //http://msdn.microsoft.com/en-us/library/x13ttww7.aspx
        private volatile bool _awaitingResult;

        public async Task AwaitAsync(Action doWhenTaskIsdone = null)
        {
            _awaitingResult = true;

            //TODO: Use DO.This to introduce a timeout?
            while (_awaitingResult)
            {
                await Task.Delay(1.Seconds()).ConfigureAwait(false);
            }

            doWhenTaskIsdone?.Invoke();
        }

        public void IsDone()
        {
            _awaitingResult = false;
        }
    }

    public class AwaitTask<T>
    {
        private readonly AwaitTask _voidAwaitTask;

        public AwaitTask()
        {
            _voidAwaitTask = new AwaitTask();
        }

        private T _result;

        public async Task<T> AwaitAsync(Action doWhenTaskIsdone = null)
        {
            await _voidAwaitTask.AwaitAsync(doWhenTaskIsdone).ConfigureAwait(false);
            return _result;
        }

        public void IsDone(T result)
        {
            _result = result;
            _voidAwaitTask.IsDone();
        }
    }
}
