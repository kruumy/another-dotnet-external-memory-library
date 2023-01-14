using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    // Most code from
    // https://github.com/Airyzz
    // I just reworked it
    public static class CallProcessFunction
    {
        /// <summary>
        /// dec    eax
        /// add    esp,0x8
        /// dec    eax
        /// mov    6..9,eax
        /// ??
        /// ??
        /// dec    eax
        /// mov    esp,ebp
        /// pop    ebp
        /// ret
        /// </summary>
        private static readonly byte[] CallEpilogue64 = new byte[19]
        {
            0x48, 0x83, 0xC4, 0x8, 0x48, 0xA3, 0x0, 0x0, 0x0, 0x0,
            0x0, 0x0, 0x0, 0x0, 0x48, 0x8B, 0xE5, 0x5D, 0xC3
        };

        /// <summary>
        /// push   ebp
        /// dec    eax
        /// mov    ebp,esp
        /// dec    eax
        /// sub    esp,0x8 
        /// </summary>
        private static readonly byte[] CallPrologue64 = new byte[8]
        {
            0x55, 0x48, 0x8B, 0xEC, 0x48, 0x83, 0xEC, 0x8
        };

        public static void Callx64(IntPtrEx Handle, IntPtrEx targetAddress, params object[] parameters)
        {
            // not finished
            uint allocationSize = (uint)(GetParametersSize(parameters) + CallPrologue64.Length + CallEpilogue64.Length);
            IntPtrEx memoryPointer = VirtualAllocEx(Handle, 0x0, new UIntPtr(allocationSize), (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
            int currentIndex = 1024;
            int parameterIndex = parameters.Length;
            byte[] prologue = CallPrologue64;
            while (parameterIndex-- > 0)
            {
                if (parameters[parameterIndex] is string s)
                {
                    IntPtr stringMemoryPointer = memoryPointer + currentIndex;
                    WriteProcessMemory.Write(Handle, stringMemoryPointer, Encoding.ASCII.GetBytes(s));
                    byte[] offset = new byte[2] { 255, 53 };
                    int value = (int)((long)stringMemoryPointer - (long)(memoryPointer + prologue.Length) - 6);
                    offset = offset.Add(BitConverter.GetBytes(value));
                    prologue = prologue.Add(offset);
                    currentIndex += ((string)parameters[parameterIndex]).Length + 2;
                }
                else
                {
                    prologue = prologue.Add(new byte[1] { 104 }, BitConverter.GetBytes((int)parameters[parameterIndex]));
                }
            }
            prologue = prologue.Add(new byte[2] { 0x48, 0xB8 }); // dec eax
            prologue = prologue.Add(targetAddress);
            prologue = prologue.Add(new byte[2] { 0xFF, 0xD0 }); //call   eax 
            byte[] epilogue = CallEpilogue64;
            long finalIndex = memoryPointer + currentIndex;
            Buffer.BlockCopy(BitConverter.GetBytes(finalIndex), 0, epilogue, 6, 8);
            prologue = prologue.Add(epilogue);
            WriteProcessMemory.Write(Handle, memoryPointer, prologue);
            VirtualFreeEx(Handle, memoryPointer, new UIntPtr(allocationSize), AllocationType.Release);
        }

        public static void Callx86(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            UIntPtr mainAllocSize = new UIntPtr(GetParametersSize(parameters) + 30u);
            IntPtrEx mainAllocAddress = VirtualAllocEx(Handle, 0x0, mainAllocSize, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
            Dictionary<IntPtrEx, UIntPtr> largeParameterAllocInfo = new Dictionary<IntPtrEx, UIntPtr>();

            List<byte> main = new List<byte>((int)mainAllocSize);
            main.AddRange(Assemblerx86.PUSH(Assemblerx86.Register.EBP));
            main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.EBP, Assemblerx86.Register.ESP));
            main.AddRange(Assemblerx86.SUB(Assemblerx86.Register.ESP, 0x8));
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
            main.AddRange(Assemblerx86.ADD(Assemblerx86.Register.ESP, 0x8));
            main.AddRange(Assemblerx86.MOV(mainAllocAddress)); // havnt added reading return value yet.
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
            UIntPtr mainAllocSize = new UIntPtr(GetParametersSize(parameters) + 50u);
            IntPtrEx mainAllocAddress = VirtualAllocEx(Handle, 0x0, mainAllocSize, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
            Dictionary<IntPtrEx, UIntPtr> largeParameterAllocInfo = new Dictionary<IntPtrEx, UIntPtr>();

            List<byte> main = new List<byte>((int)mainAllocSize);
            main.AddRange(Assemblerx86.PUSH(Assemblerx86.Register.EBP));
            main.AddRange(Assemblerx86.MOV(Assemblerx86.Register.EBP, Assemblerx86.Register.ESP));
            main.AddRange(Assemblerx86.SUB(Assemblerx86.Register.ESP, 0x8));

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
            main.AddRange(Assemblerx86.MOV(mainAllocAddress + (mainAllocSize.ToUInt32() / 2)));
            main.AddRange(Assemblerx86.ADD(Assemblerx86.Register.ESP, 0x8));
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

        private static uint GetParametersSize(params object[] parameters)
        {
            return (uint)(parameters.Length * 4);
        }
    }
}
