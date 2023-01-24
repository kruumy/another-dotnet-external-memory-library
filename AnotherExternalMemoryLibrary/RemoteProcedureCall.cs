using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class RemoteProcedureCall
    {
        public static void Callx64(IntPtrEx Handle, IntPtrEx Address, params ValueType[] parameters)
        {
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, GetParametersSize(parameters, 15) + 12u)) // TODO change later
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);
                byte stackAllocationAmount = (byte)((parameters.Length * sizeof(long)) + 40);
                main.AddRange(new byte[] { 0x48, 0x83, 0xEC, stackAllocationAmount }); // sub rsp, stackAllocationAmount

                Assemblex64RegisterParameters(ref main, parameters.Take(4).ToArray());
                Assemblex64StackParameters(ref main, parameters.Skip(4).ToArray());

                // TODO: read xmm0 & rax for return value

                main.AddRange(new byte[] { 0x48, 0xb8 }); // movabs rax,
                main.AddRange((byte[])Address);
                main.AddRange(new byte[] { 0xFF, 0xD0 }); // call rax
                main.AddRange(new byte[] { 0x48, 0x83, 0xC4, stackAllocationAmount }); // add rsp, stackAllocationAmount
                main.Add(0xc3); // ret

                WriteProcessMemory.Write(Handle, mainAlloc.Address, main.ToArray());
                CreateRemoteThread(Handle, 0, 0, mainAlloc.Address, 0, 0, out _);
            }
        }

        private static void Assemblex64RegisterParameters(ref List<byte> main, ValueType[] parameters)
        {
            if (parameters.Length > 4)
            {
                throw new ArgumentException($"{parameters.Length} is greater than the amount of parameter registers!", nameof(parameters));
            }
            int[] integerParameterRegistersValue = { 1, 2, 0, 1 };
            for (int i = 0; i < parameters.Length; i++)
            {
                byte regPrefix;
                if (i >= integerParameterRegistersValue.Length / 2)
                {
                    regPrefix = 0x49;
                }
                else
                {
                    regPrefix = 0x48;
                }
                main.Add(regPrefix);
                byte currentRegister = (byte)(0xB8 + integerParameterRegistersValue[i]);
                main.Add(currentRegister);
                main.AddRange(parameters[i].ToByteArrayUnsafe().EnforceLength(sizeof(long))); // for movabs
                if (parameters[i] is float || parameters[i] is double)
                {
                    byte floatRegister = (byte)(0xC0 + integerParameterRegistersValue[i]);
                    floatRegister += (byte)(i << 3);
                    main.AddRange(new byte[] { 0x66, regPrefix, 0x0F, 0x6E, floatRegister }); // movq floating-Point Register,currentRegister
                }
            }
        }

        private static void Assemblex64StackParameters(ref List<byte> main, ValueType[] parameters)
        {
            int regOffset = 0x20;
            foreach (ValueType param in parameters)
            {
                byte[] paramBytes = param.ToByteArrayUnsafe();
                if (paramBytes.Length > 4)
                {
                    main.AddRange(new byte[] { 0x48, 0xB8 }); // movabs rax,
                    main.AddRange(paramBytes.EnforceLength(sizeof(long)));
                    main.AddRange(new byte[] { 0x48, 0x89, 0x84, 0x24 }); // mov    QWORD PTR [rsp+ ... ],rax
                    main.AddRange(BitConverter.GetBytes(regOffset));
                }
                else
                {
                    main.AddRange(new byte[] { 0xC7, 0x84, 0x24 }); // mov DWORD PTR [rsp+
                    main.AddRange(BitConverter.GetBytes(regOffset));
                    main.AddRange(paramBytes.EnforceLength(sizeof(int)));
                }
                regOffset += sizeof(long);
            }
        }

        public static void Callx86(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            AllocateNonValueTypes(Handle, parameters, out ValueType[] newParameters, out IDisposable[] allocs);
            Callx86(Handle, Address, newParameters);
            allocs.Dispose();
        }

        public static Task<int> Callx86(IntPtrEx Handle, IntPtrEx Address, uint maxReturnAttempts, params object[] parameters)
        {
            AllocateNonValueTypes(Handle, parameters, out ValueType[] newParameters, out IDisposable[] allocs);
            Task<int> ret = Callx86(Handle, Address, maxReturnAttempts, newParameters);
            allocs.Dispose();
            return ret;
        }

        public static Task<int> Callx86(IntPtrEx Handle, IntPtrEx Address, uint maxReturnAttempts, params ValueType[] stack)
        {
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, GetParametersSize(stack, 1) + 20u))
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);
                main.Add(0x55); // push ebp
                main.AddRange(new byte[] { 0x89, 0xE5 }); //  mov ebp,esp
                for (int i = stack.Length - 1; i >= 0; i--)
                {
                    main.Add(0x68); // push
                    main.AddRange(stack[i].ToByteArrayUnsafe().EnforceLength(sizeof(int)));
                }
                main.Add(0xb8); // mov eax,
                main.AddRange((byte[])Address);
                main.AddRange(new byte[] { 0xFF, 0xD0 }); // call eax

                Task<int> returnTask = null;
                if (maxReturnAttempts > 0)
                {
                    ExternalPointer<int> returnPtr = new ExternalPointer<int>(Handle);
                    main.Add(0xA3); // mov ds:  ,eax
                    main.AddRange((byte[])returnPtr.Address);
                    int previousResult = returnPtr.Value;
                    returnTask = new Task<int>(() =>
                    {
                        for (uint i = 0; i < maxReturnAttempts; i++)
                        {
                            if (returnPtr.Value != previousResult)
                            {
                                break;
                            }
                            System.Threading.Thread.Sleep((int)(i * 10));
                        }
                        int ret = returnPtr.Value;
                        returnPtr.Dispose();
                        return ret;
                    });
                }

                main.AddRange(new byte[] { 0x89, 0xEC }); //  mov esp,ebp
                main.Add(0x5d); // pop ebp
                main.Add(0xC3); // ret
                WriteProcessMemory.Write(Handle, mainAlloc.Address, main.ToArray());
                CreateRemoteThread(Handle, 0, 0, mainAlloc.Address, 0, 0, out _);
                returnTask?.Start();
                return returnTask;
            }
        }

        public static void Callx86(IntPtrEx Handle, IntPtrEx Address, params ValueType[] parameters)
        {
            _ = Callx86(Handle, Address, 0u, parameters);
        }

        public static void UserCallx86(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            AllocateNonValueTypes(Handle, parameters, out ValueType[] newParameters, out IDisposable[] allocs);
            UserCallx86(Handle, Address, newParameters);
            allocs.Dispose();
        }

        public static void UserCallx86(IntPtrEx Handle, IntPtrEx Address, params ValueType[] parameters)
        {
            if (parameters.Length > 8)
            {
                throw new ArgumentException($"{parameters.Length} is greater than the amount of registers!", nameof(parameters));
            }
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, GetParametersSize(parameters, 1) + 12u))
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);

                for (int i = 0; i < parameters.Length; i++)
                {
                    main.Add((byte)(0xb8 + i)); // mov register,
                    main.AddRange(parameters[i].ToByteArrayUnsafe().EnforceLength(sizeof(int)));
                }

                main.AddRange(new byte[] { 0xC7, 0x45, 0xFC }); // mov DWORD PTR [ebp-0x4],
                main.AddRange((byte[])Address);
                main.AddRange(new byte[] { 0xFF, 0x55, 0xFC }); // call DWORD PTR[ebp-0x4]
                main.Add(0xC3); // ret
                WriteProcessMemory.Write(Handle, mainAlloc.Address, main.ToArray());
                CreateRemoteThread(Handle, 0, 0, mainAlloc.Address, 0, 0, out _);
            }
        }

        private static uint GetParametersSize(ValueType[] parameters, uint additionalPer = 0)
        {
            uint ret = 0;
            foreach (ValueType item in parameters)
            {
                ret += (uint)Marshal.SizeOf(item) + additionalPer;
            }
            return ret;
        }

        private static void AllocateNonValueTypes(IntPtrEx pHandle, IEnumerable<object> oldParameters, out ValueType[] newParameters, out IDisposable[] allocation)
        {
            List<ValueType> newParameters_l = new List<ValueType>();
            List<IDisposable> allocation_l = new List<IDisposable>();
            foreach (object oParam in oldParameters)
            {
                if (oParam is ValueType v)
                {
                    newParameters_l.Add(v);
                }
                else
                {
                    byte[] data;
                    if (oParam is string sParam)
                    {
                        data = sParam.ToByteArray(true);
                    }
                    else
                    {
                        data = oParam.ToByteArrayUnsafe();
                    }
                    ExternalPointerArray<byte> alloc = new ExternalPointerArray<byte>(pHandle, data);
                    allocation_l.Add(alloc);
                    newParameters_l.Add(alloc.Address);
                }
            }
            newParameters = newParameters_l.ToArray();
            allocation = allocation_l.ToArray();
        }
    }
}
