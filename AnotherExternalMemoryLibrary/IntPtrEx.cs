using System;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ptr
    {
        private readonly IntPtr value;
        public static int Size => IntPtr.Size;
        public static bool Is64Bit => Size == sizeof(long);
        public static ptr MaxValue => Is64Bit ? (ptr)long.MaxValue : (ptr)int.MaxValue;
        public static ptr MinValue => Is64Bit ? (ptr)long.MinValue : (ptr)int.MinValue;
        public ptr(IntPtr ip)
        {
            value = ip;
        }
        public override string ToString()
        {
            if (Is64Bit) return value.ToInt64().ToString($"X{IntPtr.Size * 2}");
            else return value.ToInt32().ToString($"X{IntPtr.Size * 2}");
        }
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj is ptr px && px == this;
        }
        #region Operators
        public static ptr operator +(ptr px, ptr pxo)
        {
            if (Is64Bit) return px.value.ToInt64() + pxo.value.ToInt64();
            else return px.value.ToInt32() + pxo.value.ToInt32();
        }
        public static ptr operator -(ptr px, ptr pxo)
        {
            if (Is64Bit) return px.value.ToInt64() - pxo.value.ToInt64();
            else return px.value.ToInt32() - pxo.value.ToInt32();
        }
        public static ptr operator *(ptr px, ptr pxo)
        {
            if (Is64Bit) return px.value.ToInt64() * pxo.value.ToInt64();
            else return px.value.ToInt32() * pxo.value.ToInt32();
        }
        public static ptr operator /(ptr px, ptr pxo)
        {
            if (Is64Bit) return px.value.ToInt64() / pxo.value.ToInt64();
            else return px.value.ToInt32() / pxo.value.ToInt32();
        }
        public static ptr operator %(ptr px, ptr pxo)
        {
            if (Is64Bit) return px.value.ToInt64() % pxo.value.ToInt64();
            else return px.value.ToInt32() & pxo.value.ToInt32();
        }
        public static bool operator ==(ptr px, ptr pxo)
        {
            return px.value == pxo.value;
        }
        public static bool operator !=(ptr px, ptr pxo)
        {
            return px.value != pxo.value;
        }
        public static bool operator >(ptr px, ptr pxo)
        {
            if (Is64Bit) return px.value.ToInt64() > pxo.value.ToInt64();
            else return px.value.ToInt32() > pxo.value.ToInt32();
        }
        public static bool operator <(ptr px, ptr pxo)
        {
            if (Is64Bit) return px.value.ToInt64() < pxo.value.ToInt64();
            else return px.value.ToInt32() < pxo.value.ToInt32();
        }
        #endregion
        #region Type Conversion
        public static implicit operator IntPtr(ptr px)
        {
            return px.value;
        }
        public static implicit operator UIntPtr(ptr px)
        {
            if (Is64Bit) return new UIntPtr((ulong)px.value.ToInt64());
            else return new UIntPtr((uint)px.value.ToInt32());
        }
        public static implicit operator ptr(IntPtr ip)
        {
            return new ptr(ip);
        }
        public static implicit operator ptr(UIntPtr uip)
        {
            if (Is64Bit) return new ptr(new IntPtr((long)(ulong)uip));
            else return new ptr(new IntPtr((int)(uint)uip));
        }
        public static implicit operator byte(ptr px)
        {
            return (byte)px.value;
        }
        public static implicit operator sbyte(ptr px)
        {
            return (sbyte)px.value;
        }
        public static implicit operator short(ptr px)
        {
            return (short)px.value.ToInt32();
        }
        public static implicit operator ushort(ptr px)
        {
            return (ushort)(short)px.value.ToInt32();
        }
        public static implicit operator int(ptr px)
        {
            return px.value.ToInt32();
        }
        public static implicit operator uint(ptr px)
        {
            return (uint)px.value.ToInt32();
        }
        public static implicit operator long(ptr px)
        {
            return px.value.ToInt64();
        }
        public static implicit operator ulong(ptr px)
        {
            return (ulong)px.value.ToInt64();
        }
        public static implicit operator ptr(int i)
        {
            return new ptr(new IntPtr(i));
        }
        public static implicit operator ptr(uint ui)
        {
            return new ptr(new IntPtr((int)ui));
        }
        public static implicit operator ptr(long l)
        {
            return new ptr(new IntPtr(l));
        }
        public static implicit operator ptr(ulong ul)
        {
            return new ptr(new IntPtr((long)ul));
        }
        public static unsafe implicit operator ptr(void* pointer)
        {
            return new ptr(new IntPtr(pointer));
        }
        public static unsafe implicit operator void*(ptr px)
        {
            return px.value.ToPointer();
        }
        public static implicit operator byte[](ptr px)
        {
            if (Is64Bit) return BitConverter.GetBytes((long)px);
            else return BitConverter.GetBytes((int)px);
        }
        #endregion
    }
}

