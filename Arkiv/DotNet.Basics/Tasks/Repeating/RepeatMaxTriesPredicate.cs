namespace DotNet.Basics.Tasks.Repeating
{
    public class RepeatMaxTriesPredicate
    {
        private uint _tryCount;

        public RepeatMaxTriesPredicate(uint maxTries = 10)
        {
            MaxTries = maxTries;
            Init();
        }

        public uint MaxTries { get; }

        public void Init()
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
        }
    }
}
