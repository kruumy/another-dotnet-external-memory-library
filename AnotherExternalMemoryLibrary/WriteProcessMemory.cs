using AnotherExternalMemoryLibrary.Extensions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public static class WriteProcessMemory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(IntPtrEx pHandle, IntPtrEx addr, T value) where T : unmanaged
        {
            WriteProcessMemory_(pHandle, addr, value.ToByteArray());
        }

        public static void Write<T>(IntPtrEx pHandle, IntPtrEx addr, params T[] array) where T : unmanaged
        {
            if (array is byte[] ba)
            {
                WriteProcessMemory_(pHandle, addr, ba);
                return;
            }
            int size = Marshal.SizeOf<T>();
            byte[] writeData = new byte[size * array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i].ToByteArray().CopyTo(writeData, i * size);
            }
            WriteProcessMemory_(pHandle, addr, writeData);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUnsafe<T>(IntPtrEx pHandle, IntPtrEx addr, T value)
        {
            WriteProcessMemory_(pHandle, addr, value.ToByteArrayUnsafe());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteProcessMemory_(IntPtrEx hProcess, IntPtrEx lpBaseAddress, byte[] bytes)
        {
            Win32.VirtualProtectEx(hProcess, lpBaseAddress, bytes.Length, Win32.MemoryProtection.ExecuteReadWrite, out Win32.MemoryProtection oldProtection);
            Win32.WriteProcessMemory(hProcess, lpBaseAddress, bytes, bytes.Length, out UIntPtrEx _);
            Win32.VirtualProtectEx(hProcess, lpBaseAddress, bytes.Length, oldProtection, out Win32.MemoryProtection _);
        }
    }
}
