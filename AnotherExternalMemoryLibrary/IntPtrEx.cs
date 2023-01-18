using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

#if PLATFORM_X64
using nint_t = System.Int64;
#else
using nint_t = System.Int32;
#endif

namespace AnotherExternalMemoryLibrary
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct IntPtrEx : ISerializable
    {
        private readonly nint_t value;
        public static nint_t Size = sizeof(nint_t);
        public static IntPtrEx MaxValue = nint_t.MaxValue;
        public static IntPtrEx MinValue = nint_t.MinValue;
        public static IntPtrEx Zero = 0x0;

        public IntPtrEx(nint_t value)
        {
            this.value = value;
        }
        public unsafe IntPtrEx(void* value)
        {
            this.value = (nint_t)value;
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
            return obj is IntPtrEx ex && value == ex.value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static unsafe implicit operator IntPtrEx(void* ptr)
        {
            return new IntPtrEx((nint_t)ptr);
        }
        public static implicit operator IntPtrEx(int @int)
        {
            return new IntPtrEx((nint_t)@int);
        }
        public static implicit operator IntPtrEx(uint @uint)
        {
            return new IntPtrEx((nint_t)@uint);
        }
        public static implicit operator IntPtrEx(long @long)
        {
            return new IntPtrEx((nint_t)@long);
        }
        public static implicit operator IntPtrEx(ulong @ulong)
        {
            return new IntPtrEx((nint_t)@ulong);
        }
        public static implicit operator IntPtrEx(IntPtr @IntPtr)
        {
            return new IntPtrEx((nint_t)@IntPtr);
        }
        public static implicit operator IntPtrEx(UIntPtr @UIntPtr)
        {
            return new IntPtrEx((nint_t)@UIntPtr);
        }

        public static unsafe implicit operator void*(IntPtrEx IntPtrEx)
        {
            return (void*)IntPtrEx.value;
        }
        public static implicit operator int(IntPtrEx IntPtrEx)
        {
            return (int)IntPtrEx.value;
        }
        public static implicit operator uint(IntPtrEx IntPtrEx)
        {
            return (uint)IntPtrEx.value;
        }
        public static implicit operator long(IntPtrEx IntPtrEx)
        {
            return (long)IntPtrEx.value;
        }
        public static implicit operator ulong(IntPtrEx IntPtrEx)
        {
            return (ulong)IntPtrEx.value;
        }
        public static unsafe implicit operator IntPtr(IntPtrEx IntPtrEx)
        {
            return new IntPtr((void*)IntPtrEx.value);
        }
        public static unsafe implicit operator UIntPtr(IntPtrEx IntPtrEx)
        {
            return new UIntPtr((void*)IntPtrEx.value);
        }
        public static implicit operator byte[](IntPtrEx IntPtrEx)
        {
            return BitConverter.GetBytes(IntPtrEx.value);
        }

        public static IntPtrEx operator +(IntPtrEx left, IntPtrEx right)
        {
            return left.value + right.value;
        }
        public static IntPtrEx operator -(IntPtrEx left, IntPtrEx right)
        {
            return left.value - right.value;
        }
        public static IntPtrEx operator *(IntPtrEx left, IntPtrEx right)
        {
            return left.value * right.value;
        }
        public static IntPtrEx operator /(IntPtrEx left, IntPtrEx right)
        {
            return left.value / right.value;
        }
        public static IntPtrEx operator %(IntPtrEx left, IntPtrEx right)
        {
            return left.value % right.value;
        }
        public static bool operator ==(IntPtrEx left, IntPtrEx right)
        {
            return left.value == right.value;
        }
        public static bool operator !=(IntPtrEx left, IntPtrEx right)
        {
            return left.value != right.value;
        }
        public static bool operator >(IntPtrEx left, IntPtrEx right)
        {
            return left.value > right.value;
        }
        public static bool operator <(IntPtrEx left, IntPtrEx right)
        {
            return left.value < right.value;
        }
    }
}
