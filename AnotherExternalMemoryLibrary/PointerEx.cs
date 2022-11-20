using AnotherExternalMemoryLibrary.Core.Extensions;

namespace AnotherExternalMemoryLibrary
{
    public readonly struct PointerEx
    {
        private readonly IntPtr IntPtr { get; }
        public static int Size => IntPtr.Size;
        public static bool Is64Bit => Size == sizeof(long);
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
                return IntPtr.ToInt32().ToString($"X{IntPtr.Size * 2}");
        }
        public override int GetHashCode()
        {
            return this;
        }
#pragma warning disable CS8765
        public override bool Equals(object obj)
#pragma warning restore CS8765
        {
            return obj is PointerEx px && px == this;
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
        public static implicit operator PointerEx(UIntPtr uip)
        {
            return new PointerEx(uip);
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
        public static unsafe implicit operator PointerEx(void* pointer)
        {
            return new IntPtr(pointer);
        }
        public static unsafe implicit operator void*(PointerEx px)
        {
            return px.IntPtr.ToPointer();
        }
        public static implicit operator PointerEx(byte[] bytes)
        {
            if (bytes.Length <= Size)
            {
                while (bytes.Length < Size)
                    bytes = bytes.Add(0x0);

                if (Is64Bit)
                    return BitConverter.ToInt64(bytes);
                else
                    return BitConverter.ToInt32(bytes);
            }
            else
                throw new Exception($"Cannot convert byte[] length of {bytes.Length} to PointerEx size {Size}");
        }
        public static implicit operator byte[](PointerEx px)
        {
            return Is64Bit ? BitConverter.GetBytes((long)px) : BitConverter.GetBytes((int)px);
        }
    }
    #endregion
}

