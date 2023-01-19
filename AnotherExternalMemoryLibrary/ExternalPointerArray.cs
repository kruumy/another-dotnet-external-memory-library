using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public class ExternalPointerArray<T> : ExternalAlloc, IEnumerable<T> where T : unmanaged
    {
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

        public int Length { get; }
        public UIntPtrEx SizeOfItem { get; }

        private T[] Values
        {
            get => ReadProcessMemory.Read<T>(Handle, Address, Length);
            set => WriteProcessMemory.Write<T>(Handle, Address, value);
        }

        public T this[int index]
        {
            get => ReadProcessMemory.Read<T>(Handle, GetAddressOfIndex(index));
            set => WriteProcessMemory.Write<T>(Handle, GetAddressOfIndex(index), value);
        }

        public IntPtrEx GetAddressOfIndex(int index)
        {
            if (index >= Length)
            {
                throw new System.IndexOutOfRangeException();
            }
            else
            {
                return Address + (IntPtrEx)(SizeOfItem * index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }
}
