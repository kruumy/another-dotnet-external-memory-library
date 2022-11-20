using AnotherExternalMemoryLibrary.Extensions;
using static AnotherExternalMemoryLibrary.ExternalCall.Shared;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary.ExternalCall
{
    public class UserCallx86
    {
        public PointerEx Handle { get; private set; }
        public PointerEx Address { get; private set; }
        public object? eax = null;
        public object? ecx = null;
        public object? edx = null;
        public object? ebx = null;
        public object? esp = null;
        public object? ebp = null;
        public object? esi = null;
        public object? edi = null;
        public UserCallx86(PointerEx _Handle, PointerEx _Address)
        {
            Handle = _Handle;
            Address = _Address;
        }
        public void Execute()
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
            WriteProcessMemory(Handle, ptr, array);
            CreateRemoteThread(Handle, 0x0, 0x0, ptr, 0x0, 0x0, 0x0);
        }
        public void CallFunction86(params object[] parameters)
        {
            byte[] array = CallPrologue86;
            PointerEx ptr = VirtualAllocEx(Handle, 0x0, 2048u, (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
            int num = 1024;
            int num2 = parameters.Length;
            while (num2-- > 0)
            {
                if (parameters[num2] is string s)
                {
                    byte[] array3 = new byte[1] { 104 };
                    int num3 = ptr + num;
                    WriteProcessMemory(Handle, num3, System.Text.Encoding.ASCII.GetBytes(s));
                    array3 = array3.Add(BitConverter.GetBytes(num3));
                    num += s.Length + 1;
                    array = array.Add(array3);
                }
                else
                {
                    byte[] array2 = new byte[1] { 104 };
                    array2 = array2.Add(parameters[num2].ToByteArray());
                    array = array.Add(array2);
                }
            }
            byte[] array4 = new byte[1] { 184 };
            array4 = array4.Add(Address);
            array = array.Add(array4);
            byte[] callEpilogue = CallEpilogue86;
            int num4 = ptr + num;
            Buffer.BlockCopy(BitConverter.GetBytes(num4), 0, callEpilogue, 6, 4);
            array = array.Add(callEpilogue);
            WriteProcessMemory(Handle, ptr, array);
            int num5 = -1;
            WriteProcessMemory(Handle, num4, BitConverter.GetBytes(num5));
            CreateRemoteThread(Handle, IntPtr.Zero, 0u, ptr, IntPtr.Zero, 0u, IntPtr.Zero);
            int num6 = 20;
            int num7 = 0;
            VirtualFreeEx(Handle, ptr, 2048, (uint)FreeType.Release);
        }
    }
}
