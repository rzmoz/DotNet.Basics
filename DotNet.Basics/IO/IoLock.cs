using System;
using System.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

namespace DotNet.Basics.IO
{
    public class IoLock : IDisposable
    {
        private readonly DirPath _lockDir;
        FileStream _lockHandle;

        public IoLock(DirPath lockDir, string lockName)
        {
            Name = lockName ?? throw new ArgumentNullException(nameof(lockName));
            _lockDir = lockDir;
            _lockHandle = null;
        }

        public string Name { get; }

        public bool HasLock()
        {
            return _lockHandle != null;
        }

        public bool TryAcquire(int? maxTries = 10)
        {
            _lockDir.CreateIfNotExists();
            //try to acquire exclusive handle ownership
            try
            {
                //try get lock handle
                var lockAcquired = Repeat.Task(() =>
                {
                    _lockHandle = File.Create(_lockDir.ToFile(Name).FullName(), 128, FileOptions.DeleteOnClose);
                }).WithOptions(o =>
                {
                    o.MaxTries = maxTries ?? 10;
                    o.RetryDelay = 1.Seconds();
                    o.MuteExceptions.Add<IOException>();
                }).UntilNoExceptions();

                return lockAcquired;
            }
            catch (Exception)
            {
                Dispose();
                return false;
            }
        }

        public void Dispose()
        {
            _lockHandle?.Dispose();
            _lockHandle = null;
        }
    }
}
