using System.Security.Principal;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class Utils
    {
        public static PointerEx OffsetCalculator(ProcessEx mem, PointerEx baseAddr, PointerEx baseOffset, PointerEx[] offsets)
        {
            PointerEx result = baseAddr + baseOffset;
            foreach (PointerEx offset in offsets)
                result = offset + mem.Read<PointerEx>(result);

            return result;
        }
        public static byte[] NOP(PointerEx NumOfBytes)
        {
            return Enumerable.Repeat((byte)0x90, NumOfBytes).ToArray();
        }
        public static void DumpProcessMemory(ProcessEx mem, string? path = null)
        {
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

                byte[] bytes = mem.Read(i, memInfo.RegionSize);
                AppendAllBytes(path, bytes);

                i += memInfo.RegionSize;
            }
        }
        public static void AppendAllBytes(string path, byte[] bytes)
        {
            using var stream = new FileStream(path, FileMode.Append);
            stream.Write(bytes, 0, bytes.Length);
        }
        public static void EnableDebugMode()
        {
            // might be unnessesary because Process.EnableDebugMode();
            PointerEx hToken;
            LUID luidSEDebugNameValue;
            TOKEN_PRIVILEGES tkpPrivileges;

            if (!OpenProcessToken(GetCurrentProcess(), (uint)(Privileges.TOKEN_ADJUST_PRIVILEGES | Privileges.TOKEN_QUERY), out hToken))
            {
                return;
            }
            if (!LookupPrivilegeValue(null, "SeDebugPrivilege", out luidSEDebugNameValue))
            {
                CloseHandle(hToken);
                return;
            }
            tkpPrivileges.PrivilegeCount = 1;
            tkpPrivileges.Luid = luidSEDebugNameValue;
            tkpPrivileges.Attributes = (uint)SE_PRIVILEGE.ENABLED;
            AdjustTokenPrivileges(hToken, false, ref tkpPrivileges, 0, IntPtr.Zero, IntPtr.Zero);
            CloseHandle(hToken);
        }
        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}

