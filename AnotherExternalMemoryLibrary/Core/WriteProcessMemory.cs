using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Core
{
    public static class WriteProcessMemory
    {
        public static void Write<T>(PointerEx pHandle, PointerEx addr, T value) where T : struct
        {
            byte[] data = value.ToByteArray();
            Win32.WriteProcessMemory(pHandle, addr, data);
        }
        public static void Write<T>(PointerEx pHandle, PointerEx addr, params T[] array) where T : struct
        {
            if (array is byte[] ba) { Win32.WriteProcessMemory(pHandle, addr, ba); return; }

            int size = Marshal.SizeOf(typeof(T));
            byte[] writeData = new byte[size * array.Length];
            for (int i = 0; i < array.Length; i++)
                array[i].ToByteArray().CopyTo(writeData, i * size);
            Win32.WriteProcessMemory(pHandle, addr, writeData);
        }
    }
}
