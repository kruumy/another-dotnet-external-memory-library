using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AnotherExternalMemoryLibrary
{
    public static class Win32
    {
        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
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

        [Flags]
        public enum ProcessAccess
        {
            CreateThread = 0x0002,
            SetSessionId = 0x0004,
            VmOperation = 0x0008,
            VmRead = 0x0010,
            VmWrite = 0x0020,
            DupHandle = 0x0040,
            CreateProcess = 0x0080,
            SetQuota = 0x0100,
            SetInformation = 0x0200,
            QueryInformation = 0x0400,
            SuspendResume = 0x0800,
            QueryLimitedInformation = 0x1000,
            Synchronize = 0x100000,
            Delete = 0x00010000,
            ReadControl = 0x00020000,
            WriteDac = 0x00040000,
            WriteOwner = 0x00080000,
            StandardRightsRequired = 0x000F0000,
            AllAccess = StandardRightsRequired | Synchronize | 0xFFFF
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

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = 0x0001,
            SUSPEND_RESUME = 0x0002,
            GET_CONTEXT = 0x0008,
            SET_CONTEXT = 0x0010,
            SET_INFORMATION = 0x0020,
            QUERY_INFORMATION = 0x0040,
            SET_THREAD_TOKEN = 0x0080,
            IMPERSONATE = 0x0100,
            DIRECT_IMPERSONATION = 0x0200
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtrEx CreateRemoteThread(IntPtr hProcess, IntPtrEx lpThreadAttributes, uint dwStackSize, IntPtrEx lpStartAddress, IntPtrEx lpParameter, uint dwCreationFlags, out IntPtrEx lpThreadId);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtrEx GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtrEx GetProcAddress(IntPtr hModule, string procName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern bool IsWow64Process([In] IntPtrEx processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtrEx lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtrEx lpNumberOfBytesRead);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtrEx hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtrEx VirtualAllocEx(IntPtr hProcess, IntPtrEx lpAddress, UIntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtrEx lpAddress, UIntPtr dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtrEx VirtualQueryEx(IntPtrEx hProcess, IntPtrEx lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, IntPtrEx dwLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtrEx VirtualQueryEx(IntPtrEx hProcess, IntPtrEx lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, IntPtrEx dwLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtrEx lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtrEx lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtrEx lpAddress, int dwSize, AllocationType dwFreeType);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtrEx BaseAddress;
            public IntPtrEx AllocationBase;
            public uint AllocationProtect;
            public IntPtrEx RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION64
        {
            public ulong BaseAddress;
            public ulong AllocationBase;
            public int AllocationProtect;
            public int __alignment1;
            public ulong RegionSize;
            public int State;
            public int Protect;
            public int Type;
            public int __alignment2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
