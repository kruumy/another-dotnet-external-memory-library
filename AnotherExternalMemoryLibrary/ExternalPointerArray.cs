using System;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public class ExternalPointerArray<T> : ExternalAlloc where T : unmanaged
    {
        public UIntPtr SizeOfItem { get; }
        public UIntPtr Length { get; }

        public T[] Values
        {
            get => ReadProcessMemory.Read<T>(Handle, Address, (int)Length);
            set => WriteProcessMemory.Write<T>(Handle, Address, value);
        }

        public ExternalPointerArray(IntPtrEx Handle, UIntPtr Length) : base(Handle, new UIntPtr((ulong)Marshal.SizeOf<T>() * Length.ToUInt64()))
        {
            this.Length = Length;
            SizeOfItem = (UIntPtr)Marshal.SizeOf<T>();
        }
        public ExternalPointerArray(IntPtrEx Handle, params T[] Values) : base(Handle, new UIntPtr((uint)(Marshal.SizeOf<T>() * Values.Length)))
        {
            this.Length = (UIntPtr)Values.Length;
            SizeOfItem = (UIntPtr)Marshal.SizeOf<T>();
            this.Values = Values;
        }

        public ExternalPointerArray(IntPtrEx Handle, UIntPtr Length, IntPtrEx Address) : base(Handle, new UIntPtr((ulong)Marshal.SizeOf<T>() * Length.ToUInt64()), Address)
        {
            this.Length = Length;
            SizeOfItem = (UIntPtr)Marshal.SizeOf<T>();
        }
    }
}
