using System.Diagnostics;

namespace DotNet.Basics.Tasks.Repeating
{
    public class CountLoopBreakPredicate
    {
        private uint _tryCount;

        public CountLoopBreakPredicate(uint maxTries = 10)
        {
            MaxTries = maxTries;
            Reset();
        }

        public uint MaxTries { get; }

        public void Reset()
        {
            _tryCount = 0;
        }

        public bool ShouldBreak()
        {
            return _tryCount >= MaxTries;
        }

        public void LoopCallback()
        {
            _tryCount++;
            Debug.WriteLine("Looping try {0}", _tryCount);
        }
    }
}
