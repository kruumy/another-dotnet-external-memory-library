using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class GenericExtensions
    {
        public static byte[] ToByteArray<T>(this T s) //where T : unmanaged
        {
            int size = Marshal.SizeOf(s);
            byte[] data = new byte[size];
            IntPtrEx dwStruct = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(s, dwStruct, true);
            Marshal.Copy(dwStruct, data, 0, size);
            Marshal.FreeHGlobal(dwStruct);
            return data;
        }
        public static byte[] ToByteArray<T>(this T[] a_s) //where T : unmanaged
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] data = new byte[a_s.Length * size];
            for (int i = 0; i < a_s.Length; i++)
            {
                a_s[i].ToByteArray().CopyTo(data, i * size);
            }
            return data;
        }
        public static IEnumerable<T> GetRange<T>(this IEnumerable<T> source, int start, int end)
        {
            return source.Skip(start).Take(end);
        }

        public static IntPtrEx ToPointer<T>(this T value) where T : unmanaged
        {
            IntPtrEx size = Marshal.SizeOf<T>();
            IntPtrEx address = Marshal.AllocHGlobal((IntPtr)size);
            Marshal.StructureToPtr(value, address, true);
            return address;
        }
        public static ExternalPointer<T> ToPointer<T>(this T value, IntPtrEx pHandle) where T : unmanaged
        {
            ExternalPointer<T> externalPointer = new ExternalPointer<T>(pHandle);
            externalPointer.Value = value;
            return externalPointer;
        }

        public static ExternalPointerArray<T> ToPointer<T>(this T[] values, IntPtrEx pHandle) where T : unmanaged
        {
            ExternalPointerArray<T> externalPointerArray = new ExternalPointerArray<T>(pHandle, (UIntPtr)values.Length);
            externalPointerArray.Value = values;
            return externalPointerArray;
        }
    }
}
