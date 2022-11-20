using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Core
{
    public static class WriteProcessMemory
    {
        public static void Write<T>(PointerEx pHandle, PointerEx addr, T value) where T : struct
        {
            byte[] data = Array.Empty<byte>();
            if (value is IntPtr ip) data = !PointerEx.Is64Bit ? BitConverter.GetBytes(ip.ToInt32()) : BitConverter.GetBytes(ip.ToInt64());
            else if (value is PointerEx ipx) data = !PointerEx.Is64Bit ? BitConverter.GetBytes((int)ipx) : BitConverter.GetBytes((long)ipx);
            else if (value is float f) data = BitConverter.GetBytes(f);
            else if (value is double d) data = BitConverter.GetBytes(d);
            else if (value is long l) data = BitConverter.GetBytes(l);
            else if (value is ulong ul) data = BitConverter.GetBytes(ul);
            else if (value is int i) data = BitConverter.GetBytes(i);
            else if (value is uint ui) data = BitConverter.GetBytes(ui);
            else if (value is short s) data = BitConverter.GetBytes(s);
            else if (value is ushort u) data = BitConverter.GetBytes(u);
            else if (value is bool bo) data = BitConverter.GetBytes(bo);
            else if (value is char c) data = BitConverter.GetBytes(c);
            else if (value is byte b) data = new byte[] { b };
            else if (value is sbyte sb) data = new byte[] { (byte)sb };
            else data = value.ToByteArray<T>();
            Write<byte>(pHandle, addr, data);
        }
        public static void Write<T>(PointerEx pHandle, PointerEx addr, T[] array) where T : struct
        {
            if (array is byte[] ba) { Win32.WriteProcessMemory(pHandle, addr, ba); return; }

            int size = Marshal.SizeOf(typeof(T));
            byte[] writeData = new byte[size * array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i].ToByteArray().CopyTo(writeData, i * size);
            }
            Write<byte>(pHandle, addr, writeData);
        }
    }
}
