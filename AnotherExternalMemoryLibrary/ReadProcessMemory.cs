using AnotherExternalMemoryLibrary.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public static class ReadProcessMemory
    {
        private static byte[] ReadProcessMemory_(IntPtrEx hProcess, IntPtrEx lpBaseAddress, int NumOfBytes)
        {
            byte[] data = new byte[NumOfBytes];
            Win32.ReadProcessMemory(hProcess, lpBaseAddress, data, NumOfBytes, out IntPtrEx bytesRead);
            return data;
        }
        public static T Read<T>(IntPtrEx pHandle, IntPtrEx addr) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] data = ReadProcessMemory_(pHandle, addr, size);
            return data.ToStruct<T>();
        }

        public static T[] Read<T>(IntPtrEx pHandle, IntPtrEx addr, int NumOfItems) where T : struct
        {
            if (typeof(T) == typeof(byte)) { return ReadProcessMemory_(pHandle, addr, NumOfItems) as T[]; }

            T[] arr = new T[NumOfItems];
            int size = Marshal.SizeOf(typeof(T));
            IEnumerable<byte> data = ReadProcessMemory_(pHandle, addr, arr.Length * size);
            for (int i = 0; i < arr.Length; i++)
                arr[i] = data.GetRange(i * size, size).ToArray().ToStruct<T>();
            return arr;
        }
    }
}
