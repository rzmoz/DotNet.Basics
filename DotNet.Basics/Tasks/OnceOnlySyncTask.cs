using System;

namespace DotNet.Basics.Tasks
{
    public class OnceOnlySyncTask
    {
        public OnceOnlySyncTask(Action task)
        {
            _task = () =>
            {
                _task = () => { };
                task.Invoke();
            };
        }

        private Action _task;

        public void Run()
        {
            _task.Invoke();
        }
    }
}
