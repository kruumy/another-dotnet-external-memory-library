﻿using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Text;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    // Most code from
    // https://github.com/Airyzz
    // I just reworked it
    public static class CallProcessFunction
    {
        private static readonly byte[] CallEpilogue64 = new byte[19]
        {
            0x48, 0x83, 0xC4, 0x8, 0x48, 0xA3, 0x0, 0x0, 0x0, 0x0,
            0x0, 0x0, 0x0, 0x0, 0x48, 0x8B, 0xE5, 0x5D, 0xC3
        };

        private static readonly byte[] CallEpilogue86 = new byte[14]
        {
            0xFF, 0xD0, 0x83, 0xC4, 0x8, 0xA3, 0x0, 0x0, 0x0, 0x0,
            0x8B, 0xE5, 0x5D, 0xC3
        };

        private static readonly byte[] CallPrologue64 = new byte[8]
                        {
            0x55, 0x48, 0x8B, 0xEC, 0x48, 0x83, 0xEC, 0x8
        };

        private static readonly byte[] CallPrologue86 = new byte[6]
        {
            0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x8
        };

        private static readonly byte[] UserCallEpilogue86 = new byte[22]
        {
            0xC7, 0x45, 0xFC, 0x0, 0x0, 0x0, 0x0, 0xFF, 0x55, 0xFC,
            0xA3, 0x0, 0x0, 0x0, 0x0, 0x83, 0xC4, 0x8, 0x8B, 0xE5,
            0x5D, 0xC3
        };

        public static void Callx64(IntPtrEx handle, IntPtrEx targetAddress, params object[] parameters)
        {
            // have not tested
            uint allocationSize = (uint)(CalculateAmountToAllocate(parameters) + CallPrologue64.Length + CallEpilogue64.Length);
            IntPtrEx memoryPointer = VirtualAllocEx(handle, 0x0, new UIntPtr(allocationSize), (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
            int currentIndex = 1024;
            int parameterIndex = parameters.Length;
            byte[] prologue = CallPrologue64;
            while (parameterIndex-- > 0)
            {
                if (parameters[parameterIndex] is string s)
                {
                    IntPtr stringMemoryPointer = memoryPointer + currentIndex;
                    WriteProcessMemory.Write(handle, stringMemoryPointer, Encoding.ASCII.GetBytes(s));
                    byte[] offset = new byte[2] { 255, 53 };
                    int value = (int)((long)stringMemoryPointer - (long)(memoryPointer + prologue.Length) - 6);
                    offset = offset.Add(BitConverter.GetBytes(value));
                    prologue = prologue.Add(offset);
                    currentIndex += ((string)parameters[parameterIndex]).Length + 2;
                }
                if (parameters[parameterIndex].GetType() == typeof(int))
                {
                    prologue = prologue.Add(new byte[1] { 104 }, BitConverter.GetBytes((int)parameters[parameterIndex]));
                }
            }
            prologue = prologue.Add(new byte[2] { 72, 184 });
            prologue = prologue.Add(targetAddress);
            prologue = prologue.Add(new byte[2] { 255, 208 });
            byte[] epilogue = CallEpilogue64;
            long finalIndex = memoryPointer + currentIndex;
            Buffer.BlockCopy(BitConverter.GetBytes(finalIndex), 0, epilogue, 6, 8);
            prologue = prologue.Add(epilogue);
            WriteProcessMemory.Write(handle, memoryPointer, prologue);
        }

        public static void Callx86(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            uint totalSize = (uint)(CalculateAmountToAllocate(parameters) + CallPrologue86.Length + CallEpilogue86.Length);
            IntPtrEx memoryAddress = VirtualAllocEx(Handle, 0x0, new UIntPtr(totalSize), (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
            int currentPointer = 1024;
            byte[] callPrologue = CallPrologue86;
            for (int i = parameters.Length - 1; i >= 0; i--)
            {
                if (parameters[i] == null)
                    continue;
                if (parameters[i] is string s)
                {
                    byte[] pushStringAddress = { 0x68 };
                    int stringMemoryAddress = memoryAddress + currentPointer;
                    WriteProcessMemory.Write(Handle, stringMemoryAddress, Encoding.ASCII.GetBytes(s));
                    pushStringAddress = pushStringAddress.Add(BitConverter.GetBytes(stringMemoryAddress));
                    currentPointer += s.Length + 1;
                    callPrologue = callPrologue.Add(pushStringAddress);
                }
                else
                {
                    callPrologue = callPrologue.Add(new byte[] { 0x68 }, parameters[i].ToByteArray());
                }
            }
            byte[] callInstruction = new byte[1] { 0xB8 };
            callInstruction = callInstruction.Add(Address);
            callPrologue = callPrologue.Add(callInstruction);
            byte[] callEpilogue = CallEpilogue86;
            int returnAddress = memoryAddress + currentPointer;
            Buffer.BlockCopy(BitConverter.GetBytes(returnAddress), 0, callEpilogue, 6, 4);
            callPrologue = callPrologue.Add(callEpilogue);
            WriteProcessMemory.Write(Handle, memoryAddress, callPrologue);
            int returnValue = -1;
            WriteProcessMemory.Write(Handle, returnAddress, BitConverter.GetBytes(returnValue));
            CreateRemoteThread(Handle, IntPtr.Zero, 0u, memoryAddress, IntPtr.Zero, 0u, out _);
            VirtualFreeEx(Handle, memoryAddress, 2048, AllocationType.Release);
        }

        public static void UserCallx86(IntPtrEx Handle, IntPtrEx Address, params object[] parameters)
        {
            uint totalSize = (uint)(CalculateAmountToAllocate(parameters) + CallPrologue86.Length + UserCallEpilogue86.Length);
            IntPtrEx ptr = VirtualAllocEx(Handle, 0x0, new UIntPtr(totalSize), (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
            byte[] array = CallPrologue86;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null)
                    continue;
                array = array.Add((byte)(0xB8 + i));
                if (parameters[i] is string s)
                {
                    IntPtrEx intPtr = VirtualAllocEx(Handle, IntPtr.Zero, new UIntPtr((uint)(s.Length + 1)), (AllocationType)0x3000, MemoryProtection.ExecuteReadWrite);
                    WriteProcessMemory.Write(Handle, intPtr, Encoding.ASCII.GetBytes(s));
                    array = array.Add(intPtr);
                }
                else
                {
                    array = array.Add(parameters[i].ToByteArray());
                }
            }

            Buffer.BlockCopy(Address, 0, UserCallEpilogue86, 3, 4);
            Buffer.BlockCopy(ptr + (totalSize / 2), 0, UserCallEpilogue86, 11, 4);
            array = array.Add(UserCallEpilogue86);
            WriteProcessMemory.Write(Handle, ptr, array);
            CreateRemoteThread(Handle, 0x0, 0x0, ptr, 0x0, 0x0, out _);
        }

        private static uint CalculateAmountToAllocate(params object[] parameters)
        {
            uint totalSize = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != null)
                {
                    if (parameters[i] is string s)
                    {
                        totalSize += (uint)(s.Length + 1);
                    }
                    else
                    {
                        totalSize += (uint)parameters[i].ToByteArray().Length;
                    }
                }
            }
            return totalSize;
        }
    }
}
