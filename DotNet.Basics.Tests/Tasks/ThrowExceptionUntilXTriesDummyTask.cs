using System;

namespace DotNet.Basics.Tests.Tasks
{
    public class ThrowExceptionUntilXTriesDummyTask<T> where T : Exception, new()
    {
        private int _tries;
        public int SwitchStateAfterTries { get; }

        public ThrowExceptionUntilXTriesDummyTask(int switchStateAfterTries)
        {
            SwitchStateAfterTries = switchStateAfterTries;
        }

        public void Reset()
        {
            _tries = 0;
        }

        public Action DoSomething()
        {
            _tries++;
            if (_tries <= SwitchStateAfterTries)
                throw new T();
            return () => { };
        }
    }
}
