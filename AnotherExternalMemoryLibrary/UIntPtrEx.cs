using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

#if PLATFORM_X64
using nuint_t = System.UInt64;
#else
using nuint_t = System.UInt32;
#endif

namespace AnotherExternalMemoryLibrary
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct UIntPtrEx : ISerializable
    {
        private readonly nuint_t value;
        public readonly static nuint_t Size = sizeof(nuint_t);
        public readonly static UIntPtrEx MaxValue = nuint_t.MaxValue;
        public readonly static UIntPtrEx MinValue = nuint_t.MinValue;
        public readonly static UIntPtrEx Zero = 0x0;

        public UIntPtrEx(nuint_t value)
        {
            this.value = value;
        }
        public unsafe UIntPtrEx(void* value)
        {
            this.value = (nuint_t)value;
        }
        public override string ToString()
        {
            return value.ToString($"X{Size * 2}");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info?.AddValue(nameof(value), value);
        }

        public override bool Equals(object obj)
        {
            return obj is UIntPtrEx ex && value == ex.value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static unsafe implicit operator UIntPtrEx(void* ptr)
        {
            return new UIntPtrEx((nuint_t)ptr);
        }
        public static implicit operator UIntPtrEx(int @int)
        {
            return new UIntPtrEx((nuint_t)@int);
        }
        public static implicit operator UIntPtrEx(uint @uint)
        {
            return new UIntPtrEx((nuint_t)@uint);
        }
        public static implicit operator UIntPtrEx(long @long)
        {
            return new UIntPtrEx((nuint_t)@long);
        }
        public static implicit operator UIntPtrEx(ulong @ulong)
        {
            return new UIntPtrEx((nuint_t)@ulong);
        }
        public static implicit operator UIntPtrEx(IntPtr @IntPtr)
        {
            return new UIntPtrEx((nuint_t)@IntPtr);
        }
        public static implicit operator UIntPtrEx(UIntPtr @UIntPtr)
        {
            return new UIntPtrEx((nuint_t)@UIntPtr);
        }
        public static implicit operator UIntPtrEx(IntPtrEx @IntPtrEx)
        {
            return new UIntPtrEx((nuint_t)@IntPtrEx);
        }

        public static unsafe implicit operator void*(UIntPtrEx IntPtrEx)
        {
            return (void*)IntPtrEx.value;
        }
        public static implicit operator int(UIntPtrEx UIntPtrEx)
        {
            return (int)UIntPtrEx.value;
        }
        public static implicit operator uint(UIntPtrEx UIntPtrEx)
        {
            return (uint)UIntPtrEx.value;
        }
        public static implicit operator long(UIntPtrEx UIntPtrEx)
        {
            return (long)UIntPtrEx.value;
        }
        public static implicit operator ulong(UIntPtrEx UIntPtrEx)
        {
            return (ulong)UIntPtrEx.value;
        }
        public static unsafe implicit operator IntPtr(UIntPtrEx UIntPtrEx)
        {
            return new IntPtr((void*)UIntPtrEx.value);
        }
        public static unsafe implicit operator UIntPtr(UIntPtrEx UIntPtrEx)
        {
            return new UIntPtr((void*)UIntPtrEx.value);
        }
        public static unsafe implicit operator IntPtrEx(UIntPtrEx UIntPtrEx)
        {
            return new IntPtrEx((void*)UIntPtrEx.value);
        }
        public static implicit operator byte[](UIntPtrEx UIntPtrEx)
        {
            return BitConverter.GetBytes(UIntPtrEx.value);
        }

        public static UIntPtrEx operator +(UIntPtrEx left, UIntPtrEx right)
        {
            return left.value + right.value;
        }
        public static UIntPtrEx operator -(UIntPtrEx left, UIntPtrEx right)
        {
            return left.value - right.value;
        }
        public static UIntPtrEx operator *(UIntPtrEx left, UIntPtrEx right)
        {
            return left.value * right.value;
        }
        public static UIntPtrEx operator /(UIntPtrEx left, UIntPtrEx right)
        {
            return left.value / right.value;
        }
        public static UIntPtrEx operator %(UIntPtrEx left, UIntPtrEx right)
        {
            return left.value % right.value;
        }
        public static bool operator ==(UIntPtrEx left, UIntPtrEx right)
        {
            return left.value == right.value;
        }
        public static bool operator !=(UIntPtrEx left, UIntPtrEx right)
        {
            return left.value != right.value;
        }
        public static bool operator >(UIntPtrEx left, UIntPtrEx right)
        {
            return left.value > right.value;
        }
        public static bool operator <(UIntPtrEx left, UIntPtrEx right)
        {
            return left.value < right.value;
        }
    }
}
