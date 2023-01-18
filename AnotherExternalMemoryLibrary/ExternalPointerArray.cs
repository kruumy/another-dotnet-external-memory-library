using System;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public class ExternalPointerArray<T> : ExternalAlloc where T : unmanaged
    {
        public UIntPtrEx SizeOfItem { get; }
        public int Length { get; }

        public T[] Values
        {
            get => ReadProcessMemory.Read<T>(Handle, Address, (int)Length);
            set => WriteProcessMemory.Write<T>(Handle, Address, value);
        }

        public ExternalPointerArray(IntPtrEx Handle, int Length) : base(Handle, Marshal.SizeOf<T>() * Length)
        {
            this.Length = Length;
            SizeOfItem = Marshal.SizeOf<T>();
        }
        public ExternalPointerArray(IntPtrEx Handle, params T[] Values) : base(Handle, Marshal.SizeOf<T>() * Values.Length)
        {
            this.Length = Values.Length;
            SizeOfItem = Marshal.SizeOf<T>();
            this.Values = Values;
        }

        public ExternalPointerArray(IntPtrEx Handle, int Length, IntPtrEx Address) : base(Handle, Marshal.SizeOf<T>() * Length, Address)
        {
            this.Length = Length;
            SizeOfItem = Marshal.SizeOf<T>();
        }
    }
}
