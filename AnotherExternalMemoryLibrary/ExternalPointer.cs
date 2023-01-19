using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public class ExternalPointer<T> : ExternalAlloc where T : unmanaged
    {
        public ExternalPointer(IntPtrEx Handle) : base(Handle, Marshal.SizeOf<T>())
        {
        }

        public ExternalPointer(IntPtrEx Handle, T Value) : base(Handle, Marshal.SizeOf<T>())
        {
            this.Value = Value;
        }

        public ExternalPointer(IntPtrEx Handle, IntPtrEx Address) : base(Handle, Marshal.SizeOf<T>(), Address)
        {
        }

        public T Value
        {
            get => ReadProcessMemory.Read<T>(Handle, Address);
            set => WriteProcessMemory.Write<T>(Handle, Address, value);
        }
    }
}
