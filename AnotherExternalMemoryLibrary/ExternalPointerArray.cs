using System;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public class ExternalPointerArray<T> : ExternalAlloc where T : unmanaged
    {
        public UIntPtr SizeOfItem { get; }
        public UIntPtr Length { get; }

        public T[] Value
        {
            get => ReadProcessMemory.Read<T>(Handle, Address, (int)Length);
            set => WriteProcessMemory.Write<T>(Handle, Address, value);
        }

        public ExternalPointerArray(IntPtrEx Handle, UIntPtr Length) : base(Handle, new UIntPtr((ulong)Marshal.SizeOf<T>() * Length.ToUInt64()))
        {
            this.Length = Length;
            SizeOfItem = (UIntPtr)Marshal.SizeOf<T>();
        }
    }
}
