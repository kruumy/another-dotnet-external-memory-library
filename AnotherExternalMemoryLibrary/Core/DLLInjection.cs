using System.Text;
using static AnotherExternalMemoryLibrary.Core.Win32;

namespace AnotherExternalMemoryLibrary.Core
{
    public static class DLLInjection
    {
        public static void LoadLibraryA(PointerEx pHandle, string dllPath)
        {
            PointerEx loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            PointerEx allocMemAddress = VirtualAllocEx(pHandle, 0, dllPath.Length + 1, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
            WriteProcessMemory(pHandle, allocMemAddress, Encoding.Default.GetBytes(dllPath), dllPath.Length + 1, out PointerEx _);
            CreateRemoteThread(pHandle, 0, 0, loadLibraryAddr, allocMemAddress, 0, 0);
        }
    }
}
