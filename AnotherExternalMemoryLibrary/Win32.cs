using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AnotherExternalMemoryLibrary
{
    public static class Win32
    {
        [Flags]
        public enum ProcessAccess : uint
        {
            PROCESS_ALL_ACCESS = 0x001F0FFF,
            PROCESS_CREATE_PROCESS = 0x0080,
            PROCESS_CREATE_THREAD = 0x0002,
            PROCESS_DUP_HANDLE = 0x0040,
            PROCESS_QUERY_INFORMATION = 0x0400,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
            PROCESS_SET_INFORMATION = 0x0200,
            PROCESS_SET_QUOTA = 0x0100,
            PROCESS_SUSPEND_RESUME = 0x0800,
            PROCESS_TERMINATE = 0x0001,
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020,
            SYNCHRONIZE = 0x00100000
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
            LargePages = 0x20000000,
            PRIVATE = 0x20000,
            IMAGE = 0x1000000,
            MAPPED = 0x40000

        }

        [Flags]
        public enum MemoryProtection : uint
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
        [Flags]
        public enum Privileges : uint
        {
            STANDARD_RIGHTS_REQUIRED = 0x000F0000,
            STANDARD_RIGHTS_READ = 0x00020000,
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            TOKEN_DUPLICATE = 0x0002,
            TOKEN_IMPERSONATE = 0x0004,
            TOKEN_QUERY = 0x0008,
            TOKEN_QUERY_SOURCE = 0x0010,
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            TOKEN_ADJUST_GROUPS = 0x0040,
            TOKEN_ADJUST_DEFAULT = 0x0080,
            TOKEN_ADJUST_SESSIONID = 0x0100,
            TOKEN_READ = STANDARD_RIGHTS_READ | TOKEN_QUERY,
            TOKEN_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
                TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
                TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
                TOKEN_ADJUST_SESSIONID
        }
        [Flags]
        public enum SetWindowPosFlags : uint
        {
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID Luid;
            public uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public uint Attributes;
        }
        public struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }
        public enum FreeType
        {
            Decommit = 0x4000,
            Release = 0x8000
        }
        public struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtrEx lpMinimumApplicationAddress;
            public IntPtrEx lpMaximumApplicationAddress;
            public IntPtrEx dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtrEx BaseAddress;
            public IntPtrEx AllocationBase;
            public uint AllocationProtect;
            public uint __alignment1;
            public IntPtrEx RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
            public uint __alignment2;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtrEx hProcess, IntPtrEx lpBaseAddress, byte[] lpBuffer, IntPtrEx dwSize, out IntPtrEx lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtrEx hProcess, IntPtrEx lpBaseAddress, [Out] byte[] lpBuffer, IntPtrEx dwSize, out IntPtrEx lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtrEx VirtualAllocEx(IntPtrEx hProcess, IntPtrEx lpAddress, IntPtrEx dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtrEx processHandle, IntPtrEx address, IntPtrEx size, MemoryProtection protectionType, out MemoryProtection oldProtectionType);
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtrEx LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtrEx OpenThread(ProcessAccess dwDesiredAccess, bool bInheritHandle, IntPtrEx dwThreadId);
        [DllImport("kernel32.dll")]
        public static extern int SuspendThread(IntPtrEx hThread);
        [DllImport("kernel32.dll")]
        public static extern int ResumeThread(IntPtrEx hThread);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadContext(IntPtrEx hThread, IntPtrEx lpContext);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadContext(IntPtrEx hThread, IntPtrEx lpContext);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtrEx QueueUserAPC2(IntPtrEx pfnAPC, IntPtrEx hThread, IntPtrEx dwData, IntPtrEx flags);
        [DllImport("kernel32.dll")]
        public static extern IntPtrEx OpenProcess(ProcessAccess dwDesiredAccess, bool bInheritHandle, IntPtrEx dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtrEx hProcess, out IntPtrEx ExitCode);

        [DllImport("kernel32.dll")]
        public static extern IntPtrEx GetProcessId(IntPtrEx handle);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtrEx VirtualQueryEx(IntPtrEx hProcess, IntPtrEx lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, IntPtrEx dwLength);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtrEx hProcess, IntPtrEx lpAddress, IntPtrEx dwSize, IntPtrEx dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtrEx hHandle);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtrEx processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);
        [DllImport("kernel32.dll")]
        public static extern IntPtrEx GetLastError();
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void SetLastError(IntPtrEx dwErrorCode);
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(IntPtrEx ProcessHandle, IntPtrEx DesiredAccess, out IntPtrEx TokenHandle);
        [DllImport("kernel32.dll")]
        public static extern IntPtrEx GetCurrentThread();
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtrEx GetCurrentProcess();
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(IntPtrEx TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, IntPtrEx Zero, IntPtrEx Null1, IntPtrEx Null2);
        [DllImport("kernel32.dll")]
        public static extern IntPtrEx CreateRemoteThread(IntPtrEx hProcess, IntPtrEx lpThreadAttributes, IntPtrEx dwStackSize, IntPtrEx lpStartAddress, IntPtrEx lpParameter, IntPtrEx dwCreationFlags, IntPtrEx lpThreadId);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtrEx GetModuleHandle(string lpModuleName);
        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtrEx WaitForSingleObject(IntPtrEx handle, IntPtrEx milliseconds);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern IntPtrEx GetProcAddress(IntPtrEx hModule, string procName);
        [DllImport("kernel32")]
        public static extern bool VirtualFree(IntPtrEx lpAddress, IntPtrEx dwSize, AllocationType dwFreeType);
        [DllImport("USER32.DLL")]
        public static extern bool PostMessageA(IntPtrEx hWnd, IntPtrEx Msg, IntPtrEx wParam, IntPtrEx lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtrEx hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtrEx hwnd, IntPtrEx hwndInsertAfter, int X, int Y, int width, int height, SetWindowPosFlags flags);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtrEx hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern IntPtrEx GetWindowLongPtr(IntPtrEx hWnd, int nIndex);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtrEx hWnd, out RECT rect);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtrEx FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtrEx hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtrEx hWnd);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(IntPtrEx dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetProcessIdOfThread(IntPtrEx handle);
        public delegate bool EnumWindowProc(IntPtrEx hwnd, IntPtrEx lparam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtrEx hwnd, EnumWindowProc func, IntPtrEx lParam);
    }
}
