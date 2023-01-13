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

        public static byte[] MOV(Register reg, int val)
        {
            byte[] ret = new byte[5];
            ret[0] = (byte)(0xB8 + reg);
            BitConverter.GetBytes(val).CopyTo(ret, 1);
            return ret;
        }

        public static byte[] MOV(Register reg1, Register reg2)
        {
            byte[] ret = new byte[2];
            ret[0] = 0x89;
            byte r = 0xC0;
            r += (byte)reg1;
            r += (byte)((byte)reg2 << 3);
            ret[1] = r;
            return ret;
        }

        public static byte[] PUSH(Register reg)
        {
            byte[] ret = new byte[1];
            ret[0] = (byte)(0x50 + reg);
            return ret;
        }

        public static byte[] POP(Register reg)
        {
            byte[] ret = new byte[1];
            ret[0] = (byte)(0x58 + reg);
            return ret;
        }
    }
}
