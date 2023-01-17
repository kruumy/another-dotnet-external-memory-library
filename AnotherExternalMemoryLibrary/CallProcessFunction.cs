﻿using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class CallProcessFunction
    {
        public static void Callx64(IntPtrEx Handle, IntPtrEx Address, object[] parameters)
        {
            throw new NotImplementedException();
        }
        public static void Callx64(IntPtrEx Handle, IntPtrEx Address, object param0 = null, object param1 = null, object param2 = null, object param3 = null, int[] stack = null)
        {
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(256u))) // TODO change later
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);
                main.Add(0x55); // push rbp
                main.Add(0x57); // push rdi
                main.AddRange(new byte[] { 0x48, 0x81, 0xEC, 0xE8, 0x00, 0x00, 0x00 }); // sub rsp,0xe8 
                main.AddRange(new byte[] { 0x48, 0x8D, 0x6C, 0x24, 0x20 }); // lea rbp,[rsp+0x20]


                Assemblex64RegisterParameters(ref main, param0, param1, param2, param3);
                if (stack != null)
                {
                    for (int i = 0; i < stack.Length; i++)
                    {
                        main.AddRange(new byte[] { 0xC7, 0x44, 0x24 }); //mov DWORD PTR [rsp+
                        main.Add((byte)(0x20 + (i * 8)));
                        main.AddRange(BitConverter.GetBytes(stack[i]));
                    }
                }

                main.AddRange(new byte[] { 0x48, 0xb8 }); // movabs rax,
                main.AddRange((byte[])Address);
                main.AddRange(new byte[] { 0xFF, 0xD0 }); // call rax
                main.AddRange(new byte[] { 0x48, 0x8D, 0xA5, 0xC8, 0x00, 0x00, 0x00 }); // lea rsp,[rbp+0xc8] 
                main.Add(0x5f); // pop rdi 
                main.Add(0x5d); // pop rbp
                main.Add(0xc3); // ret

                main.ForEach(v => Console.Write($"\\x{v.ToString("X")}"));

                WriteProcessMemory.Write(Handle, mainAlloc.Address, main.ToArray());
                CreateRemoteThread(Handle, 0, 0, mainAlloc.Address, 0, 0, out _);
            }
        }

        private static void Assemblex64RegisterParameters(ref List<byte> main, params object[] parameters)
        {
            if (parameters.Length > 4)
            {
                throw new ArgumentException($"{parameters.Length} is greater than the amount of parameter registers!", nameof(parameters));
            }
            int[] integerParameterRegistersValue = { 1, 2, 0, 1 };
            for (int i = 0; i < parameters.Length; i++)
            {
                byte regPrefix = 0x0;
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
                main.AddRange(parameters[i].ToByteArrayUnsafe().ToStruct<long>().ToByteArray());
                if (parameters[i] is float || parameters[i] is double)
                {
                    byte floatRegister = (byte)(0xC0 + integerParameterRegistersValue[i]);
                    floatRegister += (byte)(i << 3);
                    main.AddRange(new byte[] { 0x66, regPrefix, 0x0F, 0x6E, floatRegister }); // movq floating-Point Register,currentRegister
                }
            }
        }

        public static void Callx86(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            ObjectToIntParameters(Handle, parameters, out int[] intParameters, out ExternalAlloc[] largeParameterAllocs);
            Callx86(Handle, Address, intParameters);
            largeParameterAllocs.Dispose();
        }

        public static Task<int> Callx86(IntPtrEx Handle, IntPtrEx Address, uint maxReturnAttempts, params object[] parameters)
        {
            ObjectToIntParameters(Handle, parameters, out int[] intParameters, out ExternalAlloc[] largeParameterAllocs);
            Task<int> ret = Callx86(Handle, Address, maxReturnAttempts, intParameters);
            largeParameterAllocs.Dispose();
            return ret;
        }

        public static Task<int> Callx86(IntPtrEx Handle, IntPtrEx Address, uint maxReturnAttempts, params int[] stack)
        {
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(GetParametersSize(stack) + 20u)))
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);
                main.Add(0x55); // push ebp
                main.AddRange(new byte[] { 0x89, 0xE5 }); //  mov ebp,esp
                for (int i = stack.Length - 1; i >= 0; i--)
                {
                    main.Add(0x68); // push
                    main.AddRange(BitConverter.GetBytes(stack[i]));
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

        public static void Callx86(IntPtrEx Handle, IntPtrEx Address, params int[] parameters)
        {
            _ = Callx86(Handle, Address, 0u, parameters);
        }

        public static void UserCallx86(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            ObjectToIntParameters(Handle, parameters, out int[] intParameters, out ExternalAlloc[] largeParameterAllocs);
            UserCallx86(Handle, Address, intParameters);
            largeParameterAllocs.Dispose();
        }

        public static void UserCallx86(IntPtrEx Handle, IntPtrEx Address, params int[] parameters)
        {
            if (parameters.Length > 8)
            {
                throw new ArgumentException($"{parameters.Length} is greater than the amount of registers!", nameof(parameters));
            }
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(GetParametersSize(parameters) + 12u)))
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);

                for (int i = 0; i < parameters.Length; i++)
                {
                    main.Add((byte)(0xb8 + i)); // mov register,
                    main.AddRange(BitConverter.GetBytes(parameters[i]));
                }

                main.AddRange(new byte[] { 0xC7, 0x45, 0xFC }); // mov DWORD PTR [ebp-0x4],
                main.AddRange((byte[])Address);
                main.AddRange(new byte[] { 0xFF, 0x55, 0xFC }); // call DWORD PTR[ebp-0x4]
                main.Add(0xC3); // ret
                WriteProcessMemory.Write(Handle, mainAlloc.Address, main.ToArray());
                CreateRemoteThread(Handle, 0, 0, mainAlloc.Address, 0, 0, out _);
            }
        }

        private static uint GetParametersSize(params int[] parameters)
        {
            return (uint)(parameters.Length * (sizeof(int) + 1));
        }

        private static uint GetParametersSize(params int?[] parameters)
        {
            uint size = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != null)
                {
                    size += sizeof(int) + 1;
                }
            }
            return size;
        }

        private static void ObjectToIntParameters(IntPtrEx Handle, object[] parameters, out int[] intParameters, out ExternalAlloc[] largeParameterAllocs)
        {
            intParameters = new int[parameters.Length];
            largeParameterAllocs = Array.Empty<ExternalAlloc>();
            List<ExternalAlloc> largeParameterAllocs_l = new List<ExternalAlloc>();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is int ip)
                {
                    intParameters[i] = ip;
                }
                else if (parameters[i] is ExternalAlloc ea)
                {
                    largeParameterAllocs_l.Add(ea);
                    intParameters[i] = ea.Address;
                }
                else if (parameters[i] is string sp)
                {
                    ExternalPointerArray<byte> ep = new ExternalPointerArray<byte>(Handle, sp.ToByteArray());
                    largeParameterAllocs_l.Add(ep);
                    intParameters[i] = ep.Address;
                }
                else if (Marshal.SizeOf(parameters[i]) <= sizeof(int))
                {
                    intParameters[i] = parameters[i].ToByteArrayUnsafe().ToStruct<int>();
                }
                else
                {
                    ExternalPointerArray<byte> ep = new ExternalPointerArray<byte>(Handle, parameters[i].ToByteArrayUnsafe());
                    largeParameterAllocs_l.Add(ep);
                    intParameters[i] = ep.Address;
                }
                largeParameterAllocs = largeParameterAllocs_l.ToArray();
            }
        }
    }
}
