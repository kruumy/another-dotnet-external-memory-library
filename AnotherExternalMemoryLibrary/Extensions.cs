using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace AnotherExternalMemoryLibrary
{
    public static class Extensions
    {
        public static byte[] ToByteArray(this char[] chars)
        {
            byte[] result = new byte[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                result[i] = Convert.ToByte(chars[i]);
            }
            return result;
        }
        public static byte[] ToByteArray(this string s)
        {
            return Encoding.Default.GetBytes(s);
        }
        public static T ToStruct<T>(this byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            T val = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return val;
        }
        public static byte[] ToByteArray<T>(this T s)
        {
            PointerEx size = Marshal.SizeOf(s);
            byte[] data = new byte[size];
            PointerEx dwStruct = Marshal.AllocHGlobal((int)size);
            Marshal.StructureToPtr(s, dwStruct, true);
            Marshal.Copy(dwStruct, data, 0, size);
            Marshal.FreeHGlobal(dwStruct);
            return data;
        }
        public static byte[] ToByteArray<T>(this T[] a_s)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] data = new byte[a_s.Length * size];
            for (int i = 0; i < a_s.Length; i++)
            {
                a_s[i].ToByteArray().CopyTo(data, i * size);
            }
            return data;
        }

        public static string GetString(this byte[] bytes)
        {
            int length = bytes.Length;
            for (int i = 0; i < length; i++)
            {
                if (bytes[i] == 0)
                {
                    length = i;
                    break;
                }
            }
            return Encoding.Default.GetString(bytes, 0, length);
        }
        public static string GetHexString(this byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2} ", b);
            }
            return hex.ToString();
        }
        public static ProcessModule GetByName(this ProcessModuleCollection modules, string name)
        {
            foreach (ProcessModule item in modules)
                if (item.ModuleName == name)
                    return item;
            throw new Exception("Name Did Not Match Any ModuleNames");
        }
        public static PointerEx ToPointerEx(this IntPtr ip)
        {
            return new PointerEx(ip);
        }
    }
}
