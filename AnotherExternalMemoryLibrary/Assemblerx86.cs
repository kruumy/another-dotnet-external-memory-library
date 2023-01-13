using System;

namespace AnotherExternalMemoryLibrary
{
    public static class Assemblerx86
    {
        public enum Register : byte
        {
            EAX = 0b000,
            ECX = 0b001,
            EDX = 0b010,
            EBX = 0b011,
            ESP = 0b100,
            EBP = 0b101,
            ESI = 0b110,
            EDI = 0b111
        }

        public static byte[] ADD(Register reg, int val)
        {
            return Instruction(0x81, 0xC0, reg, val);
        }

        public static byte[] ADD(Register reg1, Register reg2)
        {
            return Instruction(0x01, reg1, reg2);
        }

        public static byte[] AND(Register reg, int val)
        {
            return Instruction(0x81, 0xE0, reg, val);
        }

        public static byte[] AND(Register reg1, Register reg2)
        {
            return Instruction(0x21, reg1, reg2);
        }

        public static byte[] CALL(int offset)
        {
            return Instruction(0xE8, offset);
        }

        public static byte[] CMP(Register reg, int val)
        {
            return Instruction(0x81, 0xF8, reg, val);
        }

        public static byte[] CMP(Register reg1, Register reg2)
        {
            return Instruction(0x39, reg1, reg2);
        }

        public static byte[] HLT()
        {
            return Instruction(0xF4);
        }

        public static byte[] MOV(Register reg1, Register reg2)
        {
            return Instruction(0x89, reg1, reg2);
        }

        public static byte[] MOV(Register reg, int val)
        {
            return Instruction(0xB8, reg, val);
        }

        public static byte[] NOP()
        {
            return Instruction(0x90);
        }

        public static byte[] OR(Register reg, int val)
        {
            return Instruction(0x81, 0xC8, reg, val);
        }

        public static byte[] OR(Register reg1, Register reg2)
        {
            return Instruction(0x09, reg1, reg2);
        }

        public static byte[] POP(Register reg)
        {
            return Instruction(0x58, reg);
        }

        public static byte[] PUSH(Register reg)
        {
            return Instruction(0x50, reg);
        }

        public static byte[] RET()
        {
            return Instruction(0xC3);
        }

        public static byte[] SUB(Register reg, int val)
        {
            return Instruction(0x81, 0xE8, reg, val);
        }

        public static byte[] SUB(Register reg1, Register reg2)
        {
            return Instruction(0x29, reg1, reg2);
        }

        public static byte[] TEST(Register reg1, Register reg2)
        {
            return Instruction(0x85, reg1, reg2);
        }

        public static byte[] XOR(Register reg, int val)
        {
            return Instruction(0x81, 0xF0, reg, val);
        }

        public static byte[] XOR(Register reg1, Register reg2)
        {
            return Instruction(0x31, reg1, reg2);
        }

        private static byte[] Instruction(byte opCode, Register reg1, Register reg2)
        {
            byte[] ret = new byte[2];
            ret[0] = opCode;
            byte r = 0xC0;
            r += (byte)reg1;
            r += (byte)((byte)reg2 << 3);
            ret[1] = r;
            return ret;
        }

        private static byte[] Instruction(byte opCode)
        {
            byte[] ret = new byte[1];
            ret[0] = opCode;
            return ret;
        }

        private static byte[] Instruction(byte opCode, Register reg)
        {
            byte[] ret = new byte[1];
            ret[0] = opCode;
            return ret;
        }

        private static byte[] Instruction(byte opCode, Register reg, int val)
        {
            byte[] ret = new byte[5];
            ret[0] = (byte)(opCode + reg);
            BitConverter.GetBytes(val).CopyTo(ret, 1);
            return ret;
        }

        private static byte[] Instruction(byte opCode, byte regOffset, Register reg, int val)
        {
            byte[] ret = new byte[6];
            ret[0] = opCode;
            ret[1] = (byte)(regOffset + reg);
            BitConverter.GetBytes(val).CopyTo(ret, 2);
            return ret;
        }

        private static byte[] Instruction(byte opCode, int val)
        {
            byte[] ret = new byte[5];
            ret[0] = opCode;
            BitConverter.GetBytes(val).CopyTo(ret, 1);
            return ret;
        }
    }
}
