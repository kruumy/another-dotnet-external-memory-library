using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Core
{
    public static class ReadProcessMemory
    {
        public static T Read<T>(PointerEx pHandle, PointerEx addr) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] data = Read<byte>(pHandle, addr, size);

            if (typeof(T) == typeof(PointerEx) || typeof(T) == typeof(IntPtr))
                if (PointerEx.Is64Bit)
                    return (dynamic)(PointerEx)BitConverter.ToInt64(data);
                else
                    return (dynamic)(PointerEx)BitConverter.ToInt32(data);
            else if (typeof(T) == typeof(float)) return (dynamic)BitConverter.ToSingle(data);
            else if (typeof(T) == typeof(double)) return (dynamic)BitConverter.ToDouble(data);
            else if (typeof(T) == typeof(long)) return (dynamic)BitConverter.ToInt64(data);
            else if (typeof(T) == typeof(ulong)) return (dynamic)BitConverter.ToUInt64(data);
            else if (typeof(T) == typeof(int)) return (dynamic)BitConverter.ToInt32(data);
            else if (typeof(T) == typeof(uint)) return (dynamic)BitConverter.ToUInt32(data);
            else if (typeof(T) == typeof(short)) return (dynamic)BitConverter.ToInt16(data);
            else if (typeof(T) == typeof(ushort)) return (dynamic)BitConverter.ToUInt16(data);
            else if (typeof(T) == typeof(bool)) return (dynamic)BitConverter.ToBoolean(data);
            else if (typeof(T) == typeof(char)) return (dynamic)Convert.ToChar(data[0]);
            else if (typeof(T) == typeof(byte)) return (dynamic)data[0];
            else if (typeof(T) == typeof(sbyte)) return (dynamic)data[0];
            else return data.ToStruct<T>();
        }

        public static T[] Read<T>(PointerEx pHandle, PointerEx addr, int NumOfItems) where T : struct
        {
            if (typeof(T) == typeof(byte)) { return (dynamic)Win32.ReadProcessMemory(pHandle, addr, NumOfItems); }

            T[] arr = new T[NumOfItems];
            int size = Marshal.SizeOf(typeof(T));
            IEnumerable<byte> data = Read<byte>(pHandle, addr, arr.Length * size);
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = data.Skip(i * size).Take(size).ToArray().ToStruct<T>();
            }
            return arr;
        }
    }
}
