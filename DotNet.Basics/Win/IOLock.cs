using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;
using System;
using System.IO;
using DotNet.Basics.IO;

namespace DotNet.Basics.Win
{
    public class IoLock(DirPath lockDir, string lockName) : IDisposable
    {
        private FileStream _lockHandle;

        public string Name { get; } = lockName ?? throw new ArgumentNullException(nameof(lockName));

        public bool HasLock()
        {
            return _lockHandle != null;
        }

        public bool TryAcquire(int? maxTries = 10)
        {
            lockDir.CreateIfNotExists();
            //try to acquire exclusive handle ownership
            try
            {
                //try to get lock handle
                return Repeat.Task(() =>
                {
                    _lockHandle = File.Create(lockDir.ToFile(Name).FullName, 128, FileOptions.DeleteOnClose);
                }).WithOptions(o =>
                {
                    o.MaxTries = maxTries ?? 10;
                    o.RetryDelay = 1.Seconds();
                    o.MuteExceptions.Add<IOException>();
                }).UntilNoExceptions();
            }
            catch (Exception)
            {
                Dispose();
                return false;
            }
        }

        public void Dispose()
        {
            try
            {
                _lockHandle?.Dispose();
            }
            finally
            {
                _lockHandle = null;
            }
        }
    }
}
