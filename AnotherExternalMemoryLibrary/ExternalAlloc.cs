using System;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public class ExternalAlloc : IDisposable
    {
        private bool disposedValue;

        public ExternalAlloc(IntPtrEx Handle, UIntPtrEx Size)
        {
            this.Handle = Handle;
            this.Size = Size;
            Address = VirtualAllocEx(Handle, 0x0, Size, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
        }

        public ExternalAlloc(IntPtrEx Handle, UIntPtrEx Size, IntPtrEx Address)
        {
            this.Handle = Handle;
            this.Size = Size;
            this.Address = Address;
        }

        ~ExternalAlloc()
        {
            Dispose(disposing: false);
        }

        public IntPtrEx Address { get; }
        public IntPtrEx Handle { get; }
        public UIntPtrEx Size { get; }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Nothing managed to dispose
                }
                VirtualFreeEx(Handle, Address, Size, AllocationType.Release);
                GC.SuppressFinalize(this);
                disposedValue = true;
            }
        }
    }
}
