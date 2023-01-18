using System;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public class ExternalAlloc : IDisposable
    {
        public IntPtrEx Handle { get; }
        public UIntPtrEx Size { get; }
        public IntPtrEx Address { get; }

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

        public void Dispose()
        {
            VirtualFreeEx(Handle, Address, Size, AllocationType.Release);
        }
    }
}
