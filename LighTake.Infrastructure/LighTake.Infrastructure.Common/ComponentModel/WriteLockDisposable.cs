using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LighTake.Infrastructure.Common
{
    public class WriteLockDisposable : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock;

        public WriteLockDisposable(ReaderWriterLockSlim rwLock)
        {
            _lock = rwLock;
            _lock.EnterWriteLock();
        }

        public void Dispose()
        {
            _lock.ExitWriteLock();
        }
    }
}
