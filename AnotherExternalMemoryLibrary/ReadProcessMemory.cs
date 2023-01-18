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
            Win32.ReadProcessMemory(hProcess, lpBaseAddress, data, NumOfBytes, out IntPtrEx _);
            return data;
        }
        public static T Read<T>(IntPtrEx pHandle, IntPtrEx addr) where T : unmanaged
        {
            int size = Marshal.SizeOf<T>();
            byte[] data = ReadProcessMemory_(pHandle, addr, size);
            return data.ToStruct<T>();
        }

        public static T[] Read<T>(IntPtrEx pHandle, IntPtrEx addr, int NumOfItems) where T : unmanaged
        {
            if (typeof(T) == typeof(byte)) { return ReadProcessMemory_(pHandle, addr, NumOfItems) as T[]; }

            T[] arr = new T[NumOfItems];
            int size = Marshal.SizeOf<T>();
            IEnumerable<byte> data = ReadProcessMemory_(pHandle, addr, arr.Length * size);
            for (int i = 0; i < arr.Length; i++)
                arr[i] = data.GetRange(i * size, size).ToArray().ToStruct<T>();
            return arr;
        }

        public static string ReadString(IntPtrEx pHandle, IntPtrEx addr, int maxLength = 1023, int buffSize = 256)
        {
            byte[] buffer;
            byte[] rawString = new byte[maxLength + 1];
            int bytesRead = 0;
            while (bytesRead < maxLength)
            {
                buffer = ReadProcessMemory_(pHandle, addr + bytesRead, buffSize);
                for (int i = 0; i < buffer.Length && i + bytesRead < maxLength; i++)
                {
                    if (buffer[i] == 0)
                    {
                        return rawString.GetString();
                    }
                    rawString[bytesRead + i] = buffer[i];
                }
                bytesRead += buffSize;
            }
            return rawString.GetString();
        }
    }
}
