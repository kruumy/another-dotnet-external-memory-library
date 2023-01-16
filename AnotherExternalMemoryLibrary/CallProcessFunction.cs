﻿using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class CallProcessFunction
    {
        public static void Callx64(IntPtrEx Handle, IntPtrEx Address, int[] intParameters)
        {
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(GetParametersSize(intParameters) + 60u)))
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);

                main.Add(Assemblerx64.PUSH(Assemblerx64.StandardRegister.RBP));
                main.Add(Assemblerx64.PUSH(Assemblerx64.StandardRegister.RDI));
                main.AddRange(Assemblerx64.SUB(Assemblerx64.StandardRegister.RSP, 0xE8));
                main.AddRange(Assemblerx64.LEA(Assemblerx64.StandardRegister.RBP, Assemblerx64.StandardRegister.RSP, 0x20));

                int integerRegsCount = 0;
                foreach (int intParameter in intParameters)
                {
                    if (integerRegsCount <= 3)
                    {
                        object intRegister = Assemblerx64.IntegerParameterRegisters[integerRegsCount];
                        if (intRegister is Assemblerx64.StandardRegister sr)
                        {
                            main.AddRange(Assemblerx64.MOV(sr, intParameter));
                        }
                        else if (intRegister is Assemblerx64.ExtraRegister er)
                        {
                            main.AddRange(Assemblerx64.MOV(er, intParameter));
                        }
                        integerRegsCount++;
                    }
                    else
                    {
                        main.AddRange(Assemblerx86.PUSH(intParameter));
                    }
                }
                main.AddRange(Assemblerx64.MOV(Assemblerx64.StandardRegister.RAX, (long)Address));
                main.AddRange(Assemblerx64.CALL(Assemblerx64.StandardRegister.RAX));

                main.AddRange(Assemblerx64.LEA(Assemblerx64.StandardRegister.RSP, Assemblerx64.StandardRegister.RBP, 0xC8));
                main.Add(Assemblerx64.POP(Assemblerx64.StandardRegister.RDI));
                main.Add(Assemblerx64.POP(Assemblerx64.StandardRegister.RBP));
                main.Add(Assemblerx64.RET());

                main.ForEach(v => Console.Write($"\\x{v.ToString("X")}"));
                WriteProcessMemory.Write(Handle, mainAlloc.Address, main.ToArray());
                CreateRemoteThread(Handle, 0, 0, mainAlloc.Address, 0, 0, out _);
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

        public static Task<int> Callx86(IntPtrEx Handle, IntPtrEx Address, uint maxReturnAttempts, params int[] parameters)
        {
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(GetParametersSize(parameters) + 40u)))
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);
                main.AddRange(Assemblerx86.SetupStackFrame());
                for (int i = parameters.Length - 1; i >= 0; i--)
                {
                    main.AddRange(Assemblerx86.PUSH(parameters[i]));
                }
                main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.EAX, Address));
                main.AddRange(Assemblerx86.CALL(Assemblerx86.Register.EAX));

                Task<int> returnTask = null;
                if (maxReturnAttempts > 0)
                {
                    ExternalPointer<int> returnPtr = new ExternalPointer<int>(Handle);
                    main.AddRange(Assemblerx86.MOV(returnPtr.Address));
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

                main.AddRange(Assemblerx86.CleanStackFrame());
                main.Add(Assemblerx86.RET());
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
            KeyValuePair<Assemblerx86.Register, int>[] newParameters = new KeyValuePair<Assemblerx86.Register, int>[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                newParameters[i] = new KeyValuePair<Assemblerx86.Register, int>((Assemblerx86.Register)i, parameters[i]);
            }
            UserCallx86(Handle, Address, newParameters);
        }

        public static void UserCallx86(IntPtrEx Handle, IntPtrEx Address, params KeyValuePair<Assemblerx86.Register, int>[] parameters)
        {
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(GetParametersSize(parameters) + 25u)))
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);
                foreach (KeyValuePair<Assemblerx86.Register, int> item in parameters)
                {
                    main.AddRange(Assemblerx86.MOV(item.Key, item.Value));
                }
                main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.EBP, -0x4, Address));
                main.AddRange(Assemblerx86.CALL(Assemblerx86.Register.EBP, -0x4));
                main.Add(Assemblerx86.RET());
                WriteProcessMemory.Write(Handle, mainAlloc.Address, main.ToArray());
                CreateRemoteThread(Handle, 0, 0, mainAlloc.Address, 0, 0, out _);
            }
        }

        private static uint GetParametersSize(params object[] parameters)
        {
            return (uint)(parameters.Length * sizeof(int));
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
