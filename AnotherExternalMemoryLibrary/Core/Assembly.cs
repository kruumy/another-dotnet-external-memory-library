using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Runtime.InteropServices;
using System.Text;
using static AnotherExternalMemoryLibrary.Core.Win32;

namespace AnotherExternalMemoryLibrary.Core
{
    public static class Assembly
    {
        public static byte[] NOP(PointerEx NumOfBytes)
        {
            return Enumerable.Repeat((byte)0x90, NumOfBytes).ToArray();
        }
        public static byte[] NOP<T>() where T : struct
        {
            return NOP(Marshal.SizeOf(typeof(T)));
        }
        public enum Register
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
        public static byte[] AssembleRegister(object register, Register type, PointerEx handle)
        {
            if (register == null) throw new ArgumentNullException(nameof(register));
            byte[] array = { (byte)(0xB8 + type) };
            if (register is string s)
            {
                PointerEx intPtr = VirtualAllocEx(handle, IntPtr.Zero, s.Length + 1, (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
                WriteProcessMemory.Write(handle, intPtr, Encoding.ASCII.GetBytes(s));
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

