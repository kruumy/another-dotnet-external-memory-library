using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class StructExtenstions
    {
        public static byte[] ToByteArray<T>(this T s) where T : struct
        {
            int size = Marshal.SizeOf(s);
            byte[] data = new byte[size];
            PointerEx dwStruct = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(s, dwStruct, true);
            Marshal.Copy(dwStruct, data, 0, size);
            Marshal.FreeHGlobal(dwStruct);
            return data;
        }
        public static byte[] ToByteArray<T>(this T[] a_s) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] data = new byte[a_s.Length * size];
            for (int i = 0; i < a_s.Length; i++)
            {
                a_s[i].ToByteArray().CopyTo(data, i * size);
            }
            return data;
        }
    }
}
