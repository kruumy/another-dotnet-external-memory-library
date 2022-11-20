using AnotherExternalMemoryLibrary.Extensions;
using System.Runtime.InteropServices;
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
        public int Execute()
        {
            uint num = 2048u;
            PointerEx ptr = VirtualAllocEx(Handle, IntPtr.Zero, num * 2, (AllocationType)12288, MemoryProtection.ExecuteReadWrite);
            PointerEx ptr2 = ptr + num;
            byte[] array = CallPrologue86;
            if (eax != null) array = array.Add(AssembleRegister(eax, Register.eax, Handle));
            if (ecx != null) array = array.Add(AssembleRegister(ecx, Register.ecx, Handle));
            if (edx != null) array = array.Add(AssembleRegister(edx, Register.edx, Handle));
            if (ebx != null) array = array.Add(AssembleRegister(ebx, Register.ebx, Handle));
            if (esp != null) array = array.Add(AssembleRegister(esp, Register.esp, Handle));
            if (ebp != null) array = array.Add(AssembleRegister(ebp, Register.ebp, Handle));
            if (esi != null) array = array.Add(AssembleRegister(esi, Register.esi, Handle));
            if (edi != null) array = array.Add(AssembleRegister(edi, Register.edi, Handle));
            Buffer.BlockCopy(BitConverter.GetBytes((uint)(int)Address), 0, UserCallEpilogue86, 3, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((uint)(int)ptr2), 0, UserCallEpilogue86, 11, 4);
            array = array.Add(UserCallEpilogue86);
            Win32.WriteProcessMemory(Handle, ptr, array);
            Random random = new Random(DateTime.Now.Millisecond);
            int num2 = random.Next(int.MinValue, int.MaxValue);
            Win32.WriteProcessMemory(Handle, ptr2, BitConverter.GetBytes(num2));
            CreateRemoteThread(Handle, IntPtr.Zero, 0u, ptr, IntPtr.Zero, 0u, IntPtr.Zero);
            while (BitConverter.ToInt32(Win32.ReadProcessMemory(Handle, ptr2, Marshal.SizeOf(ptr2))) == num2)
            {
                Thread.Sleep(5);
            }
            return BitConverter.ToInt32(Win32.ReadProcessMemory(Handle, ptr2, Marshal.SizeOf(ptr2)));
        }
    }
}
