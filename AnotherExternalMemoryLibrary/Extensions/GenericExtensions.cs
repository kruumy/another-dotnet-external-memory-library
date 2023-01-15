using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class GenericExtensions
    {
        public static IEnumerable<T> GetRange<T>(this IEnumerable<T> source, int start, int end)
        {
            return source.Skip(start).Take(end);
        }

        public static byte[] ToByteArray<T>(this T value) where T : unmanaged
        {
            return value.ToByteArrayUnsafe();
        }

        public static byte[] ToByteArray<T>(this T[] values) where T : unmanaged
        {
            return values.ToByteArrayUnsafe();
        }

        public static byte[] ToByteArrayUnsafe<T>(this T value)
        {
            int size = Marshal.SizeOf<T>();
            byte[] data = new byte[size];
            IntPtrEx dwStruct = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(value, dwStruct, true);
            Marshal.Copy(dwStruct, data, 0, size);
            Marshal.FreeHGlobal(dwStruct);
            return data;
        }

        public static byte[] ToByteArrayUnsafe<T>(this T[] values)
        {
            int size = Marshal.SizeOf<T>();
            byte[] data = new byte[values.Length * size];
            for (int i = 0; i < values.Length; i++)
            {
                values[i].ToByteArrayUnsafe().CopyTo(data, i * size);
            }
            return data;
        }
    }
}
