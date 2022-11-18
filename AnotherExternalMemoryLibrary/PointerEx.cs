namespace AnotherExternalMemoryLibrary
{
    public struct PointerEx
    {
        public static bool Is32Bit => IntPtr.Size == sizeof(int);
        public static bool Is64Bit => IntPtr.Size == sizeof(long);
        public static int Size => IntPtr.Size;

        public IntPtr IntPtr { get; set; }
        public PointerEx(IntPtr value)
        {
            IntPtr = value;
        }
        public PointerEx(UIntPtr value)
        {
            if (Is64Bit)
                IntPtr = (IntPtr)(long)(ulong)value;
            else
                IntPtr = (IntPtr)(int)(uint)value;
        }
        public override string ToString()
        {
            if (Is64Bit)
                return IntPtr.ToInt64().ToString($"X{IntPtr.Size * 2}");
            else
                return IntPtr.ToInt32().ToString($"X{IntPtr.Size}");
        }
        #region Operators
        public static PointerEx operator +(PointerEx px, PointerEx pxo)
        {
            if (Is64Bit)
                return px.IntPtr.ToInt64() + pxo.IntPtr.ToInt64();
            else
                return px.IntPtr.ToInt32() + pxo.IntPtr.ToInt32();
        }
        public static PointerEx operator -(PointerEx px, PointerEx pxo)
        {
            if (Is64Bit)
                return px.IntPtr.ToInt64() - pxo.IntPtr.ToInt64();
            else
                return px.IntPtr.ToInt32() - pxo.IntPtr.ToInt32();
        }
        public static bool operator ==(PointerEx px, PointerEx pxo)
        {
            return px.IntPtr == pxo.IntPtr;
        }
        public static bool operator !=(PointerEx px, PointerEx pxo)
        {
            return px.IntPtr != pxo.IntPtr;
        }
        public static bool operator >(PointerEx px, PointerEx pxo)
        {
            if (Is64Bit)
                return px.IntPtr.ToInt64() > pxo.IntPtr.ToInt64();
            else
                return px.IntPtr.ToInt32() > pxo.IntPtr.ToInt32();
        }
        public static bool operator <(PointerEx px, PointerEx pxo)
        {
            if (Is64Bit)
                return px.IntPtr.ToInt64() < pxo.IntPtr.ToInt64();
            else
                return px.IntPtr.ToInt32() < pxo.IntPtr.ToInt32();
        }
        #endregion
        #region Type Conversion
        public static implicit operator IntPtr(PointerEx px)
        {
            return px.IntPtr;
        }
        public static implicit operator UIntPtr(PointerEx px)
        {
            if (Is64Bit)
                return new UIntPtr((ulong)px.IntPtr.ToInt64());
            else
                return new UIntPtr((uint)px.IntPtr.ToInt32());
        }
        public static implicit operator PointerEx(IntPtr ip)
        {
            return new PointerEx(ip);
        }
        public static implicit operator bool(PointerEx px)
        {
            return (long)px != 0;
        }

        public static implicit operator byte(PointerEx px)
        {
            return (byte)px.IntPtr;
        }

        public static implicit operator sbyte(PointerEx px)
        {
            return (sbyte)px.IntPtr;
        }

        public static implicit operator int(PointerEx px)
        {
            return px.IntPtr.ToInt32();
        }

        public static implicit operator uint(PointerEx px)
        {
            return (uint)px.IntPtr.ToInt32();
        }

        public static implicit operator long(PointerEx px)
        {
            return px.IntPtr.ToInt64();
        }

        public static implicit operator ulong(PointerEx px)
        {
            return (ulong)px.IntPtr.ToInt64();
        }


        public static implicit operator PointerEx(int i)
        {
            return new IntPtr(i);
        }

        public static implicit operator PointerEx(uint ui)
        {
            return new IntPtr((int)ui);
        }

        public static implicit operator PointerEx(long l)
        {
            return new IntPtr(l);
        }

        public static implicit operator PointerEx(ulong ul)
        {
            return new IntPtr((long)ul);
        }

        public static bool operator true(PointerEx p)
        {
            return p;
        }

        public static bool operator false(PointerEx p)
        {
            return !p;
        }
        #endregion
    }
}
