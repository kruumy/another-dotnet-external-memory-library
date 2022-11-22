using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Core
{
    public static class ReadProcessMemory
    {
        public static T Read<T>(PointerEx pHandle, PointerEx addr) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] data = Win32.ReadProcessMemory(pHandle, addr, size);
            return data.ToStruct<T>();
        }

        public static T[] Read<T>(PointerEx pHandle, PointerEx addr, int NumOfItems) where T : struct
        {
            if (typeof(T) == typeof(byte)) { return (dynamic)Win32.ReadProcessMemory(pHandle, addr, NumOfItems); }

            T[] arr = new T[NumOfItems];
            int size = Marshal.SizeOf(typeof(T));
            IEnumerable<byte> data = Win32.ReadProcessMemory(pHandle, addr, arr.Length * size);
            for (int i = 0; i < arr.Length; i++)
                arr[i] = data.Skip(i * size).Take(size).ToArray().ToStruct<T>();
            return arr;
        }
    }
}
