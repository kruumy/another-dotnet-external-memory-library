using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public static class WriteProcessMemory
    {
        private static void WriteProcessMemory_(IntPtrEx hProcess, IntPtrEx lpBaseAddress, byte[] bytes)
        {
            Win32.VirtualProtectEx(hProcess, lpBaseAddress, new UIntPtr((uint)bytes.Length), Win32.MemoryProtection.ExecuteReadWrite, out Win32.MemoryProtection oldProtection);
            Win32.WriteProcessMemory(hProcess, lpBaseAddress, bytes, bytes.Length, out IntPtrEx _);
            Win32.VirtualProtectEx(hProcess, lpBaseAddress, new UIntPtr((uint)bytes.Length), oldProtection, out Win32.MemoryProtection _);
        }
        public static void Write<T>(IntPtrEx pHandle, IntPtrEx addr, T value) where T : unmanaged
        {
            byte[] data = value.ToByteArray();
            WriteProcessMemory_(pHandle, addr, data);
        }
        public static void Write<T>(IntPtrEx pHandle, IntPtrEx addr, params T[] array) where T : unmanaged
        {
            if (array is byte[] ba) { WriteProcessMemory_(pHandle, addr, ba); return; }

            int size = Marshal.SizeOf(typeof(T));
            byte[] writeData = new byte[size * array.Length];
            for (int i = 0; i < array.Length; i++)
                array[i].ToByteArray().CopyTo(writeData, i * size);
            WriteProcessMemory_(pHandle, addr, writeData);
        }
    }
}
