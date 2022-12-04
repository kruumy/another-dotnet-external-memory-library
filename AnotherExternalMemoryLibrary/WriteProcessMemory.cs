﻿using AnotherExternalMemoryLibrary.Extensions;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public static class WriteProcessMemory
    {
        private static void WriteProcessMemory_(PointerEx hProcess, PointerEx lpBaseAddress, byte[] bytes)
        {
            Win32.VirtualProtectEx(hProcess, lpBaseAddress, bytes.Length, Win32.MemoryProtection.ExecuteReadWrite, out Win32.MemoryProtection oldProtection);
            Win32.WriteProcessMemory(hProcess, lpBaseAddress, bytes, bytes.Length, out PointerEx bytesWritten);
            Win32.VirtualProtectEx(hProcess, lpBaseAddress, bytes.Length, oldProtection, out Win32.MemoryProtection _);
        }
        public static void Write<T>(PointerEx pHandle, PointerEx addr, T value) where T : struct
        {
            byte[] data = value.ToByteArray();
            WriteProcessMemory_(pHandle, addr, data);
        }
        public static void Write<T>(PointerEx pHandle, PointerEx addr, params T[] array) where T : struct
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