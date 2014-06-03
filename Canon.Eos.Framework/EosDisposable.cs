using System;
using Canon.Eos.Framework.Interfaces;

namespace Canon.Eos.Framework
{
    public abstract class EosDisposable : IDisposable, IEosAssertable
    {
        ~EosDisposable()
        {
            this.Dispose(false);
        }

        internal protected void CheckDisposed()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        internal protected bool IsDisposed { get; private set; }

        internal protected virtual void DisposeManaged() { }

        internal protected virtual void DisposeUnmanaged() { }

        private void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                    this.DisposeManaged();
                this.DisposeUnmanaged();
                this.IsDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
