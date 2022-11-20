using AnotherExternalMemoryLibrary.Extensions;
using System.Runtime.InteropServices;
using System.Text;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary.ExternalCall
{
    internal static class Shared
    {
        internal static readonly byte[] CallPrologue64 = new byte[8]
        {
            85, 72, 139, 236, 72, 131, 236, 8
        };

        internal static readonly byte[] CallEpilogue64 = new byte[19]
        {
            72, 131, 196, 8, 72, 163, 0, 0, 0, 0,
            0, 0, 0, 0, 72, 139, 229, 93, 195
        };

        internal static readonly byte[] CallPrologue86 = new byte[6]
        {
            85, 139, 236, 131, 236, 8
        };

        internal static readonly byte[] CallEpilogue86 = new byte[14]
        {
            255, 208, 131, 196, 8, 163, 0, 0, 0, 0,
            139, 229, 93, 195
        };

        internal static readonly byte[] UserCallEpilogue86 = new byte[22]
        {
            199, 69, 252, 0, 0, 0, 0, 255, 85, 252,
            163, 0, 0, 0, 0, 131, 196, 8, 139, 229,
            93, 195
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
        public static byte[] StructToByteArray(object obj)
        {
            int num = Marshal.SizeOf(obj);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr(obj, intPtr, fDeleteOld: true);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }
        internal static byte[] AssembleRegister(object register, Register type, PointerEx handle)
        {
            if (register == null) throw new ArgumentNullException(nameof(register));

            byte[] array = new byte[1] { (byte)(184 + (byte)type) };
            if (register.GetType() == typeof(int) || register.GetType() == typeof(float) || register.GetType() == typeof(bool))
            {
                byte[] array2 = StructToByteArray(register);
                array = array.Add(array2);
            }
            else if (register.GetType() == typeof(string))
            {
                IntPtr intPtr = VirtualAllocEx(handle, IntPtr.Zero, (uint)(((string)register).Length + 1), (AllocationType)12288, MemoryProtection.ExecuteReadWrite);
                Win32.WriteProcessMemory(handle, intPtr, Encoding.ASCII.GetBytes((string)register));
                byte[] array3 = StructToByteArray(intPtr);
                array = array.Add(array3);
            }
            return array;
        }
    }
}
