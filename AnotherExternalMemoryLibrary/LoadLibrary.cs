using System.Text;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class LoadLibrary
    {
        public static void LoadLibraryA(IntPtrEx pHandle, string dllPath)
        {
            IntPtrEx loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtrEx allocMemAddress = VirtualAllocEx(pHandle, 0, dllPath.Length + 1, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
            WriteProcessMemory(pHandle, allocMemAddress, Encoding.Default.GetBytes(dllPath), dllPath.Length + 1, out IntPtrEx _);
            CreateRemoteThread(pHandle, 0, 0, loadLibraryAddr, allocMemAddress, 0, 0);
        }
    }
}
