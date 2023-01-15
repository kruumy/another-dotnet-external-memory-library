using System;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public class ExternalPointer<T> : ExternalAlloc where T : struct
    {
        public T Value
        {
            get => ReadProcessMemory.Read<T>(Handle, Address);
            set => WriteProcessMemory.Write<T>(Handle, Address, value);
        }

        public ExternalPointer(IntPtrEx Handle) : base(Handle, (UIntPtr)Marshal.SizeOf<T>())
        {
        }
    }
}
