using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Text;
using static AnotherExternalMemoryLibrary.Core.Assembly;
using static AnotherExternalMemoryLibrary.Core.Win32;
namespace AnotherExternalMemoryLibrary.Core
{
    // Most code from
    // https://github.com/Airyzz
    // I just reworked it
    public static class Call
    {
        private static readonly byte[] CallPrologue64 = new byte[8]
        {
            0x55, 0x48, 0x8B, 0xEC, 0x48, 0x83, 0xEC, 0x8
        };

        private static readonly byte[] CallEpilogue64 = new byte[19]
        {
            0x48, 0x83, 0xC4, 0x8, 0x48, 0xA3, 0x0, 0x0, 0x0, 0x0,
            0x0, 0x0, 0x0, 0x0, 0x48, 0x8B, 0xE5, 0x5D, 0xC3
        };

        private static readonly byte[] CallPrologue86 = new byte[6]
        {
            0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x8
        };
        private static readonly byte[] CallEpilogue86 = new byte[14]
        {
            0xFF, 0xD0, 0x83, 0xC4, 0x8, 0xA3, 0x0, 0x0, 0x0, 0x0,
            0x8B, 0xE5, 0x5D, 0xC3
        };
        private static readonly byte[] UserCallEpilogue86 = new byte[22]
        {
            0xC7, 0x45, 0xFC, 0x0, 0x0, 0x0, 0x0, 0xFF, 0x55, 0xFC,
            0xA3, 0x0, 0x0, 0x0, 0x0, 0x83, 0xC4, 0x8, 0x8B, 0xE5,
            0x5D, 0xC3
        };

        public static void UserCallx86(PointerEx Handle, PointerEx Address, object? eax = null, object? ecx = null, object? edx = null, object? ebx = null, object? esp = null, object? ebp = null, object? esi = null, object? edi = null)
        {
            uint num = 2048u;
            PointerEx ptr = VirtualAllocEx(Handle, 0x0, num * 2, (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
            byte[] array = CallPrologue86;
            if (eax != null) array = array.Add(AssembleRegister(eax, Register.eax, Handle));
            if (ecx != null) array = array.Add(AssembleRegister(ecx, Register.ecx, Handle));
            if (edx != null) array = array.Add(AssembleRegister(edx, Register.edx, Handle));
            if (ebx != null) array = array.Add(AssembleRegister(ebx, Register.ebx, Handle));
            if (esp != null) array = array.Add(AssembleRegister(esp, Register.esp, Handle));
            if (ebp != null) array = array.Add(AssembleRegister(ebp, Register.ebp, Handle));
            if (esi != null) array = array.Add(AssembleRegister(esi, Register.esi, Handle));
            if (edi != null) array = array.Add(AssembleRegister(edi, Register.edi, Handle));
            Buffer.BlockCopy(Address, 0, UserCallEpilogue86, 3, 4);
            Buffer.BlockCopy(ptr + num, 0, UserCallEpilogue86, 11, 4);
            array = array.Add(UserCallEpilogue86);
            WriteProcessMemory.Write(Handle, ptr, array);
            Win32.CreateRemoteThread(Handle, 0x0, 0x0, ptr, 0x0, 0x0, 0x0);
        }
        public static void Callx86(PointerEx Handle, PointerEx Address, params object[] parameters)
        {
            byte[] array = CallPrologue86;
            PointerEx ptr = VirtualAllocEx(Handle, 0x0, 2048u, (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
            int num = 1024;
            int num2 = parameters.Length;
            while (num2-- > 0)
            {
                if (parameters[num2] is string s)
                {
                    byte[] array3 = { 0x68 };
                    int num3 = ptr + num;
                    WriteProcessMemory.Write(Handle, num3, Encoding.ASCII.GetBytes(s));
                    array3 = array3.Add(BitConverter.GetBytes(num3));
                    num += s.Length + 1;
                    array = array.Add(array3);
                }
                else
                {
                    array = array.Add(new byte[] { 0x68 }, parameters[num2].ToByteArray());
                }
            }
            byte[] array4 = new byte[1] { 184 };
            array4 = array4.Add(Address);
            array = array.Add(array4);
            byte[] callEpilogue = CallEpilogue86;
            int num4 = ptr + num;
            Buffer.BlockCopy(BitConverter.GetBytes(num4), 0, callEpilogue, 6, 4);
            array = array.Add(callEpilogue);
            WriteProcessMemory.Write(Handle, ptr, array);
            int num5 = -1;
            WriteProcessMemory.Write(Handle, num4, BitConverter.GetBytes(num5));
            CreateRemoteThread(Handle, IntPtr.Zero, 0u, ptr, IntPtr.Zero, 0u, IntPtr.Zero);
            VirtualFreeEx(Handle, ptr, 2048, (uint)FreeType.Release);
        }
        public static void Callx64(PointerEx Handle, PointerEx Address, params object[] parameters)
        {
            byte[] array = CallPrologue64;
            PointerEx ptr = VirtualAllocEx(Handle, 0x0, 2048u, (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
            int num = 1024;
            int num2 = parameters.Length;
            while (num2-- > 0)
            {
                if (parameters[num2] is string s)
                {
                    PointerEx intPtr2 = ptr + num;
                    WriteProcessMemory.Write(Handle, intPtr2, Encoding.ASCII.GetBytes(s));
                    byte[] array2 = new byte[2] { 255, 53 };
                    int value = (int)((long)intPtr2 - (long)(ptr + array.Length) - 6);
                    array2 = array2.Add(BitConverter.GetBytes(value));
                    array = array.Add(array2);
                    num += ((string)parameters[num2]).Length + 2;
                }
                if (parameters[num2].GetType() == typeof(int))
                {
                    array = array.Add(new byte[1] { 104 }, BitConverter.GetBytes((int)parameters[num2]));
                }
            }
            array = array.Add(new byte[2] { 72, 184 });
            array = array.Add(BitConverter.GetBytes((long)Address));
            array = array.Add(new byte[2] { 255, 208 });
            byte[] callEpilogue = CallEpilogue64;
            long value2 = ptr + num;
            Buffer.BlockCopy(BitConverter.GetBytes(value2), 0, callEpilogue, 6, 8);
            array = array.Add(callEpilogue);
            WriteProcessMemory.Write(Handle, ptr, array);
        }
    }
}
