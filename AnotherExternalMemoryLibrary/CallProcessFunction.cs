using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class CallProcessFunction
    {
        public static void Callx64(IntPtrEx Handle, IntPtrEx Address, int[] intParameters, float[] floatParameters)
        {
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(GetParametersSize(intParameters) + 60u)))
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);

                main.AddRange(Assemblerx64.PUSH(Assemblerx64.StandardRegister.RBP));
                main.AddRange(Assemblerx64.MOV(Assemblerx64.StandardRegister.RBP, Assemblerx64.StandardRegister.RSP));

                // https://learn.microsoft.com/en-us/windows-hardware/drivers/debugger/x64-architecture#calling-conventions
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
                foreach (float floatParameter in floatParameters)
                {
                    //TODO
                }
                // figure out why having more than 1 argument breaks the stack frame
                main.AddRange(Assemblerx64.MOV(Assemblerx64.StandardRegister.RAX, (long)Address));
                main.AddRange(Assemblerx64.CALL(Assemblerx64.StandardRegister.RAX));
                main.AddRange(Assemblerx64.POP(Assemblerx64.StandardRegister.RBP));
                main.AddRange(Assemblerx64.RET());
                main.ForEach(v => Console.Write($"\\x{v.ToString("X")}"););
                WriteAndCreateThread(Handle, Address, main.ToArray());
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
            throw new NotImplementedException();
        }

        public static void Callx86(IntPtrEx Handle, IntPtrEx Address, params int[] parameters)
        {
            using (ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(GetParametersSize(parameters) + 30u)))
            {
                List<byte> main = new List<byte>((int)mainAlloc.Size);
                main.AddRange(Assemblerx86.SetupStackFrame());
                for (int i = parameters.Length - 1; i >= 0; i--)
                {
                    main.AddRange(Assemblerx86.PUSH(parameters[i]));
                }
                main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.EAX, Address));
                main.AddRange(Assemblerx86.CALL(Assemblerx86.Register.EAX));
                main.AddRange(Assemblerx86.CleanStackFrame());
                main.Add(Assemblerx86.RET());
                WriteAndCreateThread(Handle, Address, main.ToArray());
            }
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
                WriteAndCreateThread(Handle, Address, main.ToArray());
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
                else if (Marshal.SizeOf(parameters[i]) <= sizeof(int))
                {
                    intParameters[i] = parameters[i].ToByteArrayUnsafe().ToStruct<int>();
                }
                else if (parameters[i] is string sp)
                {
                    ExternalPointerArray<char> ep = sp.ToCharArray().ToPointer<char>(Handle);
                    largeParameterAllocs_l.Add(ep);
                    intParameters[i] = ep.Address;
                }
                else
                {
                    ExternalPointerArray<byte> ep = parameters[i].ToByteArrayUnsafe().ToPointer<byte>(Handle);
                    largeParameterAllocs_l.Add(ep);
                    intParameters[i] = ep.Address;
                }
                largeParameterAllocs = largeParameterAllocs_l.ToArray();
            }
        }

        private static IntPtrEx WriteAndCreateThread(IntPtrEx Handle, IntPtr Address, byte[] data)
        {
            WriteProcessMemory.Write<byte>(Handle, Address, data);
            return CreateRemoteThread(Handle, 0, 0, Address, 0, 0, out _);
        }
    }
}
