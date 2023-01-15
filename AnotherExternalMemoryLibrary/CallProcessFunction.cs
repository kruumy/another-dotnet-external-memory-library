using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Collections.Generic;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class CallProcessFunction
    {
        public static void Callx64(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(GetParametersSize(parameters) + 60u));
            List<ExternalAlloc> largeParameterAllocs = new List<ExternalAlloc>();
            List<byte> main = new List<byte>((int)mainAlloc.Size);

            main.AddRange(Assemblerx64.PUSH(Assemblerx64.StandardRegister.RBP));
            main.AddRange(Assemblerx64.MOV(Assemblerx64.StandardRegister.RBP, Assemblerx64.StandardRegister.RSP));

            // https://learn.microsoft.com/en-us/windows-hardware/drivers/debugger/x64-architecture#calling-conventions
            int integerRegsCount = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is string s)
                {
                    throw new NotImplementedException(s);
                }
                else if (parameters[i] is float || parameters[i] is double)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    if (integerRegsCount <= 3)
                    {
                        object intRegister = Assemblerx64.IntegerParameterRegisters[integerRegsCount];
                        if (intRegister is Assemblerx64.StandardRegister sr)
                        {
                            main.AddRange(Assemblerx64.MOV(sr, parameters[i].ToByteArray().ToStruct<int>()));
                        }
                        else if (intRegister is Assemblerx64.ExtraRegister er)
                        {
                            main.AddRange(Assemblerx64.MOV(er, parameters[i].ToByteArray().ToStruct<int>()));
                        }
                        integerRegsCount++;
                    }
                    else
                    {
                        main.AddRange(Assemblerx86.PUSH(parameters[i].ToByteArray().ToStruct<int>()));
                    }
                }
            }
            // figure out why having more than 1 argument breaks the stack frame
            main.AddRange(Assemblerx64.MOV(Assemblerx64.StandardRegister.RAX, (long)Address));
            main.AddRange(Assemblerx64.CALL(Assemblerx64.StandardRegister.RAX));
            main.AddRange(Assemblerx64.POP(Assemblerx64.StandardRegister.RBP));
            main.AddRange(Assemblerx64.RET());


            foreach (byte item in main)
            {
                Console.Write($"\\x{item.ToString("X")}");
            }


            largeParameterAllocs.ForEach(a => a.Dispose());
            mainAlloc.Dispose();
        }

        public static void Callx86(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(GetParametersSize(parameters) + 30u));
            List<ExternalAlloc> largeParameterAllocs = new List<ExternalAlloc>();

            List<byte> main = new List<byte>((int)mainAlloc.Size);
            main.AddRange(Assemblerx86.PUSH(Assemblerx86.Register.EBP));
            main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.EBP, Assemblerx86.Register.ESP));
            for (int i = parameters.Length - 1; i >= 0; i--)
            {
                if (parameters[i] is string s)
                {
                    ExternalPointerArray<char> strptr = new ExternalPointerArray<char>(Handle, (UIntPtr)s.Length)
                    {
                        Value = s.ToCharArray()
                    };
                    largeParameterAllocs.Add(strptr);
                    main.AddRange(Assemblerx86.PUSH(strptr.Address));
                }
                else
                {
                    main.AddRange(Assemblerx86.PUSH(parameters[i].ToByteArray().ToStruct<int>()));
                }
            }
            main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.EAX, Address));
            main.AddRange(Assemblerx86.CALL(Assemblerx86.Register.EAX));
            main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.ESP, Assemblerx86.Register.EBP));
            main.AddRange(Assemblerx86.POP(Assemblerx86.Register.EBP));
            main.AddRange(Assemblerx86.RET());

            WriteProcessMemory.Write(Handle, mainAlloc.Address, main.ToArray());
            CreateRemoteThread(Handle, 0, 0, mainAlloc.Address, 0, 0, out _);

            largeParameterAllocs.ForEach(a => a.Dispose());
            mainAlloc.Dispose();
        }

        public static void UserCallx86(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            KeyValuePair<Assemblerx86.Register, object>[] newParameters = new KeyValuePair<Assemblerx86.Register, object>[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                newParameters[i] = new KeyValuePair<Assemblerx86.Register, object>((Assemblerx86.Register)i, parameters[i]);
            }
            UserCallx86(Handle, Address, newParameters);
        }

        public static void UserCallx86(IntPtrEx Handle, IntPtrEx Address, params KeyValuePair<Assemblerx86.Register, object>[] parameters)
        {
            ExternalAlloc mainAlloc = new ExternalAlloc(Handle, new UIntPtr(GetParametersSize(parameters) + 25u));
            List<ExternalAlloc> largeParameterAllocs = new List<ExternalAlloc>();

            List<byte> main = new List<byte>((int)mainAlloc.Size);
            foreach (KeyValuePair<Assemblerx86.Register, object> item in parameters)
            {
                if (item.Value is string s)
                {
                    ExternalPointerArray<char> strptr = new ExternalPointerArray<char>(Handle, (UIntPtr)s.Length)
                    {
                        Value = s.ToCharArray()
                    };
                    largeParameterAllocs.Add(strptr);
                    main.AddRange(Assemblerx86.MOV(item.Key, strptr.Address));
                }
                else
                {
                    main.AddRange(Assemblerx86.MOV(item.Key, item.Value.ToByteArray().ToStruct<int>()));
                }
            }

            main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.EBP, -0x4, Address));
            main.AddRange(Assemblerx86.CALL(Assemblerx86.Register.EBP, -0x4));
            main.AddRange(Assemblerx86.RET());

            WriteProcessMemory.Write(Handle, mainAlloc.Address, main.ToArray());
            CreateRemoteThread(Handle, 0, 0, mainAlloc.Address, 0, 0, out _);

            largeParameterAllocs.ForEach(a => a.Dispose());
            mainAlloc.Dispose();
        }

        private static uint GetParametersSize(params object[] parameters)
        {
            return (uint)(parameters.Length * 4);
        }
    }
}
