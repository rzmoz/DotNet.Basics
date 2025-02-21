using System;

namespace DotNet.Basics.Tests.Tasks
{
    public class ThrowExceptionUntilXTriesDummyTask<T>(int switchStateAfterTries) where T : Exception, new()
    {
        private int _tries;
        public int SwitchStateAfterTries { get; } = switchStateAfterTries;

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
