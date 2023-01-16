using System;

namespace AnotherExternalMemoryLibrary
{
    /// <summary>
    /// WIP, not all instructions complete.
    /// </summary>
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

        /// <summary>
        /// push ebp
        /// mov ebp,esp
        /// </summary>
        public static byte[] SetupStackFrame()
        {
            byte[] ret = new byte[3];
            ret[0] = PUSH(Register.EBP);
            Buffer.BlockCopy(MOV(Register.EBP, Register.ESP), 0, ret, 1, 2);
            return ret;
        }

        /// <summary>
        /// mov esp,ebp
        /// pop ebp
        /// </summary>
        public static byte[] CleanStackFrame()
        {
            byte[] ret = new byte[3];
            Buffer.BlockCopy(MOV(Register.ESP, Register.EBP), 0, ret, 0, 2);
            ret[2] = POP(Register.EBP);
            return ret;
        }

        /// <summary>
        /// add <paramref name="reg"/>,<paramref name="val"/>
        /// </summary>
        public static byte[] ADD(Register reg, int val)
        {
            return Instruction(0x81, 0xC0, reg, val);
        }

        /// <summary>
        /// add <paramref name="reg1"/>,<paramref name="reg2"/>
        /// </summary>
        public static byte[] ADD(Register reg1, Register reg2)
        {
            return Instruction(0x01, reg1, reg2);
        }

        /// <summary>
        /// and <paramref name="reg"/>,<paramref name="val"/>
        /// </summary>
        public static byte[] AND(Register reg, int val)
        {
            return Instruction(0x81, 0xE0, reg, val);
        }

        /// <summary>
        /// and <paramref name="reg1"/>,<paramref name="reg2"/>
        /// </summary>
        public static byte[] AND(Register reg1, Register reg2)
        {
            return Instruction(0x21, reg1, reg2);
        }

        /// <summary>
        /// call <paramref name="offset"/>
        /// </summary>
        public static byte[] CALL(int offset)
        {
            return Instruction(0xE8, offset);
        }

        /// <summary>
        /// call <paramref name="reg"/>
        /// </summary>
        public static byte[] CALL(Register reg)
        {
            return Instruction(0xFF, 0xD0, reg);
        }

        /// <summary>
        /// call [<paramref name="reg"/>+/-<paramref name="regPtrOffset"/>]
        /// </summary>
        public static byte[] CALL(Register reg, int regPtrOffset)
        {
            return Instruction(0xFF, 0x90, reg, regPtrOffset);
        }

        /// <summary>
        /// CMP <paramref name="reg"/>,<paramref name="val"/>
        /// </summary>
        public static byte[] CMP(Register reg, int val)
        {
            return Instruction(0x81, 0xF8, reg, val);
        }

        /// <summary>
        /// CMP <paramref name="reg1"/>,<paramref name="reg2"/>
        /// </summary>
        public static byte[] CMP(Register reg1, Register reg2)
        {
            return Instruction(0x39, reg1, reg2);
        }

        /// <summary>
        /// hlt
        /// </summary>
        public static byte HLT()
        {
            return Instruction(0xF4);
        }

        /// <summary>
        /// jmp <paramref name="offset"/>
        /// </summary>
        public static byte[] JMP(int offset)
        {
            return Instruction(0xE9, offset);
        }

        /// <summary>
        /// mov <paramref name="reg1"/>,<paramref name="reg2"/>
        /// </summary>
        public static byte[] MOV(Register reg1, Register reg2)
        {
            return Instruction(0x89, reg1, reg2);
        }

        /// <summary>
        /// mov <paramref name="reg1"/>,[<paramref name="reg2"/>+/-<paramref name="reg2PtrOffset"/>]
        /// </summary>
        public static byte[] MOV(Register reg1, Register reg2, int reg2PtrOffset)
        {
            return Instruction(0x8B, reg1, reg2, reg2PtrOffset);
        }

        /// <summary>
        /// mov [<paramref name="reg1"/>+/-<paramref name="reg1PtrOffset"/>],<paramref name="reg2"/>
        /// </summary>
        public static byte[] MOV(Register reg1, int reg1PtrOffset, Register reg2)
        {
            return Instruction(0x89, reg1, reg1PtrOffset, reg2);
        }

        /// <summary>
        /// mov [<paramref name="reg1"/>+/-<paramref name="reg1PtrOffset"/>],<paramref name="val"/>
        /// </summary>
        public static byte[] MOV(Register reg1, int reg1PtrOffset, int val)
        {
            return Instruction(0xC7, reg1, reg1PtrOffset, val);
        }

        /// <summary>
        /// mov <paramref name="reg"/>,<paramref name="val"/>
        /// </summary>
        public static byte[] MOV(Register reg, int val)
        {
            return Instruction(0xB8, reg, val);
        }

        /// <summary>
        /// mov <paramref name="ds"/>,eax
        /// </summary>
        public static byte[] MOV(int ds)
        {
            return Instruction(0xA3, ds);
        }

        /// <summary>
        /// nop
        /// </summary>
        public static byte NOP()
        {
            return Instruction(0x90);
        }

        /// <summary>
        /// or <paramref name="reg"/>,<paramref name="val"/>
        /// </summary>
        public static byte[] OR(Register reg, int val)
        {
            return Instruction(0x81, 0xC8, reg, val);
        }

        /// <summary>
        /// or <paramref name="reg1"/>,<paramref name="reg2"/>
        /// </summary>
        public static byte[] OR(Register reg1, Register reg2)
        {
            return Instruction(0x09, reg1, reg2);
        }

        /// <summary>
        /// pop <paramref name="reg"/>
        /// </summary>
        public static byte POP(Register reg)
        {
            return Instruction(0x58, reg);
        }

        /// <summary>
        /// push <paramref name="reg"/>
        /// </summary>
        public static byte PUSH(Register reg)
        {
            return Instruction(0x50, reg);
        }

        /// <summary>
        /// push <paramref name="val"/>
        /// </summary>
        public static byte[] PUSH(int val)
        {
            return Instruction(0x68, val);
        }

        /// <summary>
        /// dec <paramref name="reg"/>
        /// </summary>
        public static byte DEC(Register reg)
        {
            return Instruction(0x48, reg);
        }

        /// <summary>
        /// ret
        /// </summary>
        public static byte RET()
        {
            return Instruction(0xC3);
        }

        /// <summary>
        /// sub <paramref name="reg"/>,<paramref name="val"/>
        /// </summary>
        public static byte[] SUB(Register reg, int val)
        {
            return Instruction(0x81, 0xE8, reg, val);
        }

        /// <summary>
        /// sub <paramref name="reg1"/>,<paramref name="reg2"/>
        /// </summary>
        public static byte[] SUB(Register reg1, Register reg2)
        {
            return Instruction(0x29, reg1, reg2);
        }

        /// <summary>
        /// test <paramref name="reg1"/>,<paramref name="reg2"/>
        /// </summary>
        public static byte[] TEST(Register reg1, Register reg2)
        {
            return Instruction(0x85, reg1, reg2);
        }

        /// <summary>
        /// xor <paramref name="reg"/>,<paramref name="val"/>
        /// </summary>
        public static byte[] XOR(Register reg, int val)
        {
            return Instruction(0x81, 0xF0, reg, val);
        }

        /// <summary>
        /// xor <paramref name="reg1"/>,<paramref name="reg2"/>
        /// </summary>
        public static byte[] XOR(Register reg1, Register reg2)
        {
            return Instruction(0x31, reg1, reg2);
        }

        private static byte[] Instruction(byte opCode, Register reg1, int reg1PtrOffset, int val)
        {
            byte[] ret;
            if (!reg1PtrOffset.ShouldBe4Bytes())
            {
                ret = new byte[2 + (sizeof(byte) + sizeof(int))];
                ret[1] = 0x40;
            }
            else
            {
                ret = new byte[2 + (sizeof(int) * 2)];
                ret[1] = 0x80;
            }
            ret[0] = opCode;
            ret[1] += (byte)reg1;
            if (!reg1PtrOffset.ShouldBe4Bytes())
            {
                Buffer.BlockCopy(BitConverter.GetBytes(reg1PtrOffset), 0, ret, 2, sizeof(byte));
                Buffer.BlockCopy(BitConverter.GetBytes(val), 0, ret, 2 + sizeof(byte), sizeof(int));
            }
            else
            {
                Buffer.BlockCopy(BitConverter.GetBytes(reg1PtrOffset), 0, ret, 2, sizeof(int));
                Buffer.BlockCopy(BitConverter.GetBytes(val), 0, ret, 2 + sizeof(int), sizeof(int));
            }
            return ret;
        }

        private static byte[] Instruction(byte opCode, Register reg1, Register reg2, int reg2PtrOffset)
        {
            byte[] ret;
            byte r;
            if (!reg2PtrOffset.ShouldBe4Bytes())
            {
                ret = new byte[2 + sizeof(byte)];
                r = 0x40;
            }
            else
            {
                ret = new byte[2 + sizeof(int)];
                r = 0x80;
            }
            ret[0] = opCode;
            r += (byte)reg2;
            r += (byte)((byte)reg1 << 3);
            ret[1] = r;
            Buffer.BlockCopy(BitConverter.GetBytes(reg2PtrOffset), 0, ret, 2, ret.Length - 2);
            return ret;
        }

        private static byte[] Instruction(byte opCode, Register reg1, int reg1PtrOffset, Register reg2)
        {
            byte[] ret;
            byte r;
            if (!reg1PtrOffset.ShouldBe4Bytes())
            {
                ret = new byte[2 + sizeof(byte)];
                r = 0x40;
            }
            else
            {
                ret = new byte[2 + sizeof(int)];
                r = 0x80;
            }
            ret[0] = opCode;
            r += (byte)reg1;
            r += (byte)((byte)reg2 << 3);
            ret[1] = r;
            Buffer.BlockCopy(BitConverter.GetBytes(reg1PtrOffset), 0, ret, 2, ret.Length - 2);
            return ret;
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

        private static byte Instruction(byte opCode)
        {
            return opCode;
        }

        private static byte Instruction(byte opCode, Register reg)
        {
            return (byte)(opCode + reg);
        }

        private static byte[] Instruction(byte opCode, Register reg, int val)
        {
            byte[] ret = new byte[5];
            ret[0] = (byte)(opCode + reg);
            BitConverter.GetBytes(val).CopyTo(ret, 1);
            return ret;
        }

        private static byte[] Instruction(byte opCode, byte regBaseOffset, Register reg, int val)
        {
            byte[] ret = new byte[6];
            ret[0] = opCode;
            ret[1] = (byte)(regBaseOffset + reg);
            BitConverter.GetBytes(val).CopyTo(ret, 2);
            return ret;
        }

        private static byte[] Instruction(byte opCode, byte regBaseOffset, Register reg)
        {
            byte[] ret = new byte[2];
            ret[0] = opCode;
            ret[1] = (byte)(regBaseOffset + reg);
            return ret;
        }

        private static byte[] Instruction(byte opCode, int val)
        {
            byte[] ret = new byte[5];
            ret[0] = opCode;
            BitConverter.GetBytes(val).CopyTo(ret, 1);
            return ret;
        }

        internal static bool ShouldBe4Bytes(this int val)
        {
            return val >= 0x80;
        }
    }
}
