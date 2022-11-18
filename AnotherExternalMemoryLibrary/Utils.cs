using AnotherExternalMemoryLibrary.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

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

