using AnotherExternalMemoryLibrary.Extensions;
using System.Text;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary.ExternalCall
{
    internal static class Shared
    {
        internal static readonly byte[] CallPrologue64 = new byte[8]
        {
            0x55, 0x48, 0x8B, 0xEC, 0x48, 0x83, 0xEC, 0x8
        };

        internal static readonly byte[] CallEpilogue64 = new byte[19]
        {
            0x48, 0x83, 0xC4, 0x8, 0x48, 0xA3, 0x0, 0x0, 0x0, 0x0,
            0x0, 0x0, 0x0, 0x0, 0x48, 0x8B, 0xE5, 0x5D, 0xC3
        };

        internal static readonly byte[] CallPrologue86 = new byte[6]
        {
            0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x8
        };
        internal static readonly byte[] CallEpilogue86 = new byte[14]
        {
            0xFF, 0xD0, 0x83, 0xC4, 0x8, 0xA3, 0x0, 0x0, 0x0, 0x0,
            0x8B, 0xE5, 0x5D, 0xC3
        };
        internal static readonly byte[] UserCallEpilogue86 = new byte[22]
        {
            0xC7, 0x45, 0xFC, 0x0, 0x0, 0x0, 0x0, 0xFF, 0x55, 0xFC,
            0xA3, 0x0, 0x0, 0x0, 0x0, 0x83, 0xC4, 0x8, 0x8B, 0xE5,
            0x5D, 0xC3
        };
        internal enum Register
        {
            eax,
            ecx,
            edx,
            ebx,
            esp,
            ebp,
            esi,
            edi
        }
        internal static byte[] AssembleRegister(object register, Register type, PointerEx handle)
        {
            if (register == null) throw new ArgumentNullException(nameof(register));
            byte[] array = { (byte)(0xB8 + type) };
            if (register is string s)
            {
                PointerEx intPtr = VirtualAllocEx(handle, IntPtr.Zero, s.Length + 1, (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
                WriteProcessMemory(handle, intPtr, Encoding.ASCII.GetBytes(s));
                array = array.Add(intPtr);
            }
            else
            {
                array = array.Add(register.ToByteArray());
            }
            return array;
        }
    }
}
