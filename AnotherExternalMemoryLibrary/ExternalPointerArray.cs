using System.Collections;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public class ExternalPointerArray<T> : ExternalAlloc, IEnumerable where T : unmanaged
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
            WriteFullArray(Values);
        }

        public ExternalPointerArray(IntPtrEx Handle, int Length, IntPtrEx Address) : base(Handle, Marshal.SizeOf<T>() * Length, Address)
        {
            this.Length = Length;
            SizeOfItem = Marshal.SizeOf<T>();
        }

        public int Length { get; }
        public UIntPtrEx SizeOfItem { get; }

        public T this[int index]
        {
            get => ReadProcessMemory.Read<T>(Handle, GetAddressOfIndex(index));
            set => WriteProcessMemory.Write<T>(Handle, GetAddressOfIndex(index), value);
        }

        public IntPtrEx GetAddressOfIndex(int index)
        {
            return index >= Length ? throw new System.IndexOutOfRangeException() : Address + (IntPtrEx)(SizeOfItem * index);
        }

        public IEnumerator GetEnumerator()
        {
            return ReadFullArray().GetEnumerator();
        }

        public T[] ReadFullArray()
        {
            return ReadRange(0, Length);
        }

        public T[] ReadRange(int startIndex, int amount)
        {
            return ReadProcessMemory.Read<T>(Handle, GetAddressOfIndex(startIndex), amount);
        }

        public void WriteFullArray(T[] values)
        {
            WriteRange(0, values);
        }

        public void WriteRange(int startIndex, T[] values)
        {
            WriteProcessMemory.Write<T>(Handle, GetAddressOfIndex(startIndex), values);
        }
    }
}
