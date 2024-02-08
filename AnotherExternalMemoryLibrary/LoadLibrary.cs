using AnotherExternalMemoryLibrary.Extensions;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class LoadLibrary
    {
        public static IntPtrEx LoadLibraryA(IntPtrEx pHandle, string dllPath)
        {
            byte[] dllPathBytes = dllPath.ToByteArray(true);
            IntPtrEx loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtrEx allocMemAddress = VirtualAllocEx(pHandle, 0, dllPathBytes.Length, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
            WriteProcessMemory(pHandle, allocMemAddress, dllPathBytes, dllPathBytes.Length, out UIntPtrEx _);
            CreateRemoteThread(pHandle, 0, 0, loadLibraryAddr, allocMemAddress, 0, out IntPtrEx _);
            return allocMemAddress;
        }
    }
}
