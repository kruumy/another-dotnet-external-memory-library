using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AnotherExternalMemoryLibrary
{
    public static class Win32
    {
        public enum ProcessAccess
        {
            PROCESS_CREATE_THREAD = 0x02,
            PROCESS_ACCESS = PROCESS_QUERY_INFORMATION | PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_CREATE_THREAD | 0x0040,
            PROCESS_QUERY_INFORMATION = 0x0400,
            PAGE_READWRITE = 0x04,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020,
            PROCESS_VM_OPERATION = 0x0008
        }
        [Flags]
        public enum AllocationType : uint
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            ResetUndo = 0x1000000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }
        private enum State : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000
        }
        private enum Type : uint
        {
            MEM_IMAGE = 0x1000000,
            MEM_MAPPED = 0x40000,
            MEM_PRIVATE = 0x20000
        }
        public struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public PointerEx lpMinimumApplicationAddress;
            public PointerEx lpMaximumApplicationAddress;
            public PointerEx dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        public struct MEMORY_BASIC_INFORMATION
        {
            public PointerEx BaseAddress;
            public PointerEx AllocationBase;
            public uint AllocationProtect;
            public uint __alignment1;
            public PointerEx RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
            public uint __alignment2;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(PointerEx hProcess, PointerEx lpBaseAddress, byte[] lpBuffer, int dwSize, ref PointerEx lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(PointerEx hProcess, PointerEx lpBaseAddress, [Out] byte[] lpBuffer, PointerEx dwSize, ref PointerEx lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern PointerEx VirtualAllocEx(PointerEx hProcess, PointerEx lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(PointerEx processHandle, PointerEx address, int size, MemoryProtection protectionType, out int oldProtectionType);
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern PointerEx LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern PointerEx OpenThread(ProcessAccess dwDesiredAccess, bool bInheritHandle, int dwThreadId);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadContext(IntPtr hThread, IntPtr lpContext);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadContext(IntPtr hThread, IntPtr lpContext);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint ResumeThread(PointerEx hThread);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint QueueUserAPC2(PointerEx pfnAPC, PointerEx hThread, PointerEx dwData, int flags);
        [DllImport("kernel32.dll")]
        public static extern PointerEx OpenProcess(ProcessAccess dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(PointerEx hProcess, out uint ExitCode);

        [DllImport("kernel32.dll")]
        public static extern int GetProcessId(PointerEx handle);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern PointerEx VirtualQueryEx(PointerEx hProcess, PointerEx lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(PointerEx hProcess, PointerEx lpAddress, uint dwSize, int dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(PointerEx hHandle);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] PointerEx processHandle,
             [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern void SetLastError(uint dwErrorCode);
    }
}
