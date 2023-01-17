using System;

namespace AnotherExternalMemoryLibrary
{
    public readonly struct IntPtrEx
    {
        private readonly IntPtr value;
        public static int Size => IntPtr.Size;
        public static bool Is64Bit => Size == sizeof(long);
        public static IntPtrEx MaxValue => Is64Bit ? (IntPtrEx)long.MaxValue : (IntPtrEx)int.MaxValue;
        public static IntPtrEx MinValue => Is64Bit ? (IntPtrEx)long.MinValue : (IntPtrEx)int.MinValue;
        public static IntPtrEx Zero => IntPtr.Zero;
        public IntPtrEx(IntPtr ip)
        {
            value = ip;
        }
        public object ToArchitectureInteger()
        {
            return Is64Bit ? (long)this : (int)this;
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
            return obj is IntPtrEx px && px == this;
        }
        #region Operators
        public static IntPtrEx operator +(IntPtrEx px, IntPtrEx pxo)
        {
            if (Is64Bit) return px.value.ToInt64() + pxo.value.ToInt64();
            else return px.value.ToInt32() + pxo.value.ToInt32();
        }
        public static IntPtrEx operator -(IntPtrEx px, IntPtrEx pxo)
        {
            if (Is64Bit) return px.value.ToInt64() - pxo.value.ToInt64();
            else return px.value.ToInt32() - pxo.value.ToInt32();
        }
        public static IntPtrEx operator *(IntPtrEx px, IntPtrEx pxo)
        {
            if (Is64Bit) return px.value.ToInt64() * pxo.value.ToInt64();
            else return px.value.ToInt32() * pxo.value.ToInt32();
        }
        public static IntPtrEx operator /(IntPtrEx px, IntPtrEx pxo)
        {
            if (Is64Bit) return px.value.ToInt64() / pxo.value.ToInt64();
            else return px.value.ToInt32() / pxo.value.ToInt32();
        }
        public static IntPtrEx operator %(IntPtrEx px, IntPtrEx pxo)
        {
            if (Is64Bit) return px.value.ToInt64() % pxo.value.ToInt64();
            else return px.value.ToInt32() & pxo.value.ToInt32();
        }
        public static bool operator ==(IntPtrEx px, IntPtrEx pxo)
        {
            return px.value == pxo.value;
        }
        public static bool operator !=(IntPtrEx px, IntPtrEx pxo)
        {
            return px.value != pxo.value;
        }
        public static bool operator >(IntPtrEx px, IntPtrEx pxo)
        {
            if (Is64Bit) return px.value.ToInt64() > pxo.value.ToInt64();
            else return px.value.ToInt32() > pxo.value.ToInt32();
        }
        public static bool operator <(IntPtrEx px, IntPtrEx pxo)
        {
            if (Is64Bit) return px.value.ToInt64() < pxo.value.ToInt64();
            else return px.value.ToInt32() < pxo.value.ToInt32();
        }
        #endregion
        #region Type Conversion
        public static implicit operator IntPtr(IntPtrEx px)
        {
            return px.value;
        }
        public static implicit operator UIntPtr(IntPtrEx px)
        {
            if (Is64Bit) return new UIntPtr((ulong)px.value.ToInt64());
            else return new UIntPtr((uint)px.value.ToInt32());
        }
        public static implicit operator IntPtrEx(IntPtr ip)
        {
            return new IntPtrEx(ip);
        }
        public static implicit operator IntPtrEx(UIntPtr uip)
        {
            if (Is64Bit) return new IntPtrEx(new IntPtr((long)(ulong)uip));
            else return new IntPtrEx(new IntPtr((int)(uint)uip));
        }
        public static implicit operator byte(IntPtrEx px)
        {
            return (byte)px.value;
        }
        public static implicit operator sbyte(IntPtrEx px)
        {
            return (sbyte)px.value;
        }
        public static implicit operator short(IntPtrEx px)
        {
            return (short)px.value.ToInt32();
        }
        public static implicit operator ushort(IntPtrEx px)
        {
            return (ushort)(short)px.value.ToInt32();
        }
        public static implicit operator int(IntPtrEx px)
        {
            return px.value.ToInt32();
        }
        public static implicit operator uint(IntPtrEx px)
        {
            return (uint)px.value.ToInt32();
        }
        public static implicit operator long(IntPtrEx px)
        {
            return px.value.ToInt64();
        }
        public static implicit operator ulong(IntPtrEx px)
        {
            return (ulong)px.value.ToInt64();
        }
        public static implicit operator IntPtrEx(int i)
        {
            return new IntPtrEx(new IntPtr(i));
        }
        public static implicit operator IntPtrEx(uint ui)
        {
            return new IntPtrEx(new IntPtr((int)ui));
        }
        public static implicit operator IntPtrEx(long l)
        {
            return new IntPtrEx(new IntPtr(l));
        }
        public static implicit operator IntPtrEx(ulong ul)
        {
            return new IntPtrEx(new IntPtr((long)ul));
        }
        public static unsafe implicit operator IntPtrEx(void* pointer)
        {
            return new IntPtrEx(new IntPtr(pointer));
        }
        public static unsafe implicit operator void*(IntPtrEx px)
        {
            return px.value.ToPointer();
        }
        public static implicit operator byte[](IntPtrEx px)
        {
            if (Is64Bit) return BitConverter.GetBytes((long)px);
            else return BitConverter.GetBytes((int)px);
        }
        #endregion
    }
}

