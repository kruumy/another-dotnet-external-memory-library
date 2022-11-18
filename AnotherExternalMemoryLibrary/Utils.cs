using AnotherExternalMemoryLibrary.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class Utils
    {
        public static PointerEx OffsetCalculator(PointerEx handle, PointerEx baseAddr, PointerEx baseOffset, params PointerEx[] offsets)
        {
            PointerEx result = baseAddr + baseOffset;
            foreach (PointerEx offset in offsets)
                result = offset + Win32.ReadProcessMemory(handle, result, PointerEx.Size).ToStruct<PointerEx>();

            return result;
        }
        public static byte[] NOP(PointerEx NumOfBytes)
        {
            return Enumerable.Repeat((byte)0x90, NumOfBytes).ToArray();
        }
        public static byte[] NOP<T>() where T : struct
        {
            return NOP(Marshal.SizeOf(typeof(T)));
        }
        public static void DumpProcessMemory(ProcessEx mem, string? path = null)
        {
            //TODO: make sure it works
            path ??= $"{mem.BaseProcess.ProcessName}_{mem.BaseProcess.UserProcessorTime.ToString().Replace(':', '_')}.dmp";
            if (File.Exists(path)) File.Delete(path);

            Win32.SYSTEM_INFO sys_info = new Win32.SYSTEM_INFO();
            Win32.GetSystemInfo(out sys_info);

            PointerEx proc_min_address = mem.BaseAddress;
            PointerEx proc_max_address = mem.BaseProcess.WorkingSet64 + proc_min_address;

            PointerEx i = proc_min_address;
            MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();
            while (i < proc_max_address)
            {
                VirtualQueryEx(mem.BaseProcess.Handle, sys_info.lpMinimumApplicationAddress, out memInfo, sys_info.dwPageSize);

                byte[] bytes = mem.Read<byte>(i, memInfo.RegionSize);
                AppendAllBytes(path, bytes);

                i += memInfo.RegionSize;
            }
        }
        public static void AppendAllBytes(string path, params byte[] bytes)
        {
            using FileStream stream = new FileStream(path, FileMode.Append);
            stream.Write(bytes, 0, bytes.Length);
        }
        public static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent())
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }
        public static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length != 0;
        }
    }
}

