using System;

namespace AnotherExternalMemoryLibrary
{
    /// <summary>
    /// WIP, not all instructions complete.
    /// </summary>
    public static class Assemblerx64
    {
        public enum ExtraRegister : byte
        {
            R8 = 0,
            R9 = 1,
            R10 = 2,
            R11 = 3,
            R12 = 4,
            R13 = 5,
            R14 = 6,
            R15 = 7
        }

        public enum StandardRegister : byte
        {
            RAX = 0,
            RCX = 1,
            RDX = 2,
            RBX = 3,
            RSP = 4,
            RBP = 5,
            RSI = 6,
            RDI = 7,
        }

        public static object[] IntegerParameterRegisters => new object[] { Assemblerx64.StandardRegister.RCX, Assemblerx64.StandardRegister.RDX, Assemblerx64.ExtraRegister.R8, Assemblerx64.ExtraRegister.R9 };

        public static byte[] ADD(StandardRegister reg, int val)
        {
            return Instruction(0x81, 0xC0, reg, val);
        }

        public static byte[] CALL(StandardRegister reg)
        {
            byte[] ret = new byte[2];
            ret[0] = 0xFF;
            ret[1] = (byte)(0xD0 + (byte)reg);
            return ret;
        }

        public static byte[] MOV(StandardRegister reg, int val)
        {
            return Instruction(0xC7, 0xC0, reg, val);
        }

        public static byte[] MOV(StandardRegister reg1, StandardRegister reg2)
        {
            byte[] ret = new byte[3];
            ret[0] = 0x48;
            ret[1] = 0x89;
            ret[2] = 0xC0;
            ret[2] += (byte)reg1;
            ret[2] += (byte)((byte)reg2 << 3);
            return ret;
        }

        public static byte[] MOV(StandardRegister reg, long val)
        {
            byte[] ret = new byte[2 + sizeof(long)];
            ret[0] = 0x48;
            ret[1] = (byte)(0xB8 + reg);
            byte[] value = BitConverter.GetBytes(val);
            Array.Copy(value, 0, ret, 2, value.Length);
            return ret;
        }

        public static byte[] MOV(ExtraRegister reg, int val)
        {
            return Instruction(0xC7, 0xC0, reg, val);
        }

        public static byte[] POP(StandardRegister reg)
        {
            return Instruction(0x58, reg);
        }

        public static byte[] PUSH(StandardRegister reg)
        {
            return Instruction(0x50, reg);
        }

        public static byte[] RET()
        {
            return Instruction(0xC3);
        }

        internal static byte[] SUB(StandardRegister reg, int val)
        {
            return Instruction(0x81, 0xE9, reg, val);
        }

        private static byte[] Instruction(byte opCode)
        {
            byte[] ret = new byte[1];
            ret[0] = opCode;
            return ret;
        }

        private static byte[] Instruction(byte opCode, StandardRegister reg)
        {
            byte[] ret = new byte[1];
            ret[0] = (byte)(opCode + (byte)reg);
            return ret;
        }

        private static byte[] Instruction(byte opCode, byte regBaseOffset, StandardRegister reg, int val)
        {
            byte[] ret = new byte[3 + sizeof(int)];
            ret[0] = 0x48;
            ret[1] = opCode;
            ret[2] = (byte)(regBaseOffset + reg);
            byte[] value = BitConverter.GetBytes(val);
            Array.Copy(value, 0, ret, 3, value.Length);
            return ret;
        }
        private static byte[] Instruction(byte opCode, byte regBaseOffset, ExtraRegister reg, int val)
        {
            byte[] ret = new byte[3 + sizeof(int)];
            ret[0] = 0x49;
            ret[1] = opCode;
            ret[2] = (byte)(regBaseOffset + reg);
            byte[] value = BitConverter.GetBytes(val);
            Array.Copy(value, 0, ret, 3, value.Length);
            return ret;
        }

    }
}
