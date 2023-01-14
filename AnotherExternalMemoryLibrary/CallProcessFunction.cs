using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class CallProcessFunction
    {
        public static void Callx86(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            UIntPtr mainAllocSize = new UIntPtr(GetParametersSize(parameters) + 30u);
            IntPtrEx mainAllocAddress = VirtualAllocEx(Handle, 0x0, mainAllocSize, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
            Dictionary<IntPtrEx, UIntPtr> largeParameterAllocInfo = new Dictionary<IntPtrEx, UIntPtr>();

            List<byte> main = new List<byte>((int)mainAllocSize);
            main.AddRange(Assemblerx86.PUSH(Assemblerx86.Register.EBP));
            main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.EBP, Assemblerx86.Register.ESP));
            for (int i = parameters.Length - 1; i >= 0; i--)
            {
                if (parameters[i] is string s)
                {
                    byte[] strbytes = Encoding.Default.GetBytes(s);
                    IntPtrEx strAddr = VirtualAllocEx(Handle, 0x0, (UIntPtr)strbytes.Length, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
                    WriteProcessMemory.Write(Handle, strAddr, strbytes);
                    largeParameterAllocInfo.Add(strAddr, (UIntPtr)strbytes.Length);
                    main.AddRange(Assemblerx86.PUSH(strAddr));
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

            WriteProcessMemory.Write(Handle, mainAllocAddress, main.ToArray());
            CreateRemoteThread(Handle, 0, 0, mainAllocAddress, 0, 0, out _);

            foreach (KeyValuePair<IntPtrEx, UIntPtr> addr in largeParameterAllocInfo)
            {
                VirtualFreeEx(Handle, addr.Key, addr.Value, AllocationType.Release);
            }
            VirtualFreeEx(Handle, mainAllocAddress, mainAllocSize, AllocationType.Release);
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
            UIntPtr mainAllocSize = new UIntPtr(GetParametersSize(parameters) + 25u);
            IntPtrEx mainAllocAddress = VirtualAllocEx(Handle, 0x0, mainAllocSize, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
            Dictionary<IntPtrEx, UIntPtr> largeParameterAllocInfo = new Dictionary<IntPtrEx, UIntPtr>();

            List<byte> main = new List<byte>((int)mainAllocSize);
            foreach (KeyValuePair<Assemblerx86.Register, object> item in parameters)
            {
                if (item.Value is string s)
                {
                    byte[] strbytes = Encoding.Default.GetBytes(s);
                    IntPtrEx strAddr = VirtualAllocEx(Handle, 0x0, (UIntPtr)strbytes.Length, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
                    WriteProcessMemory.Write(Handle, strAddr, strbytes);
                    largeParameterAllocInfo.Add(strAddr, (UIntPtr)strbytes.Length);
                    main.AddRange(Assemblerx86.MOV(item.Key, strAddr));
                }
                else
                {
                    main.AddRange(Assemblerx86.MOV(item.Key, item.Value.ToByteArray().ToStruct<int>()));
                }
            }

            main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.EBP, -0x4, Address));
            main.AddRange(Assemblerx86.CALL(Assemblerx86.Register.EBP, -0x4));
            main.AddRange(Assemblerx86.RET());

            WriteProcessMemory.Write(Handle, mainAllocAddress, main.ToArray());
            CreateRemoteThread(Handle, 0, 0, mainAllocAddress, 0, 0, out _);

            foreach (KeyValuePair<IntPtrEx, UIntPtr> addr in largeParameterAllocInfo)
            {
                VirtualFreeEx(Handle, addr.Key, addr.Value, AllocationType.Release);
            }
            VirtualFreeEx(Handle, mainAllocAddress, mainAllocSize, AllocationType.Release);
        }

        private static uint GetParametersSize(params object[] parameters)
        {
            return (uint)(parameters.Length * 4);
        }
    }
}
