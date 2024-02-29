using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class ByteExtensions
    {
        public static unsafe string GetString(this byte[] buffer, int index = 0)
        {
            fixed (byte* bytes = &buffer[index])
            {
                return new string((sbyte*)bytes);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ToStruct( this byte[] data, Type type )
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            object val = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
            handle.Free();
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ToStruct<T>(this byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            T val = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ToStructArray<T>(this byte[] data, int? size = null) where T : unmanaged
        {
            int sizeOfResultType = size == null ? Marshal.SizeOf<T>() : (int)size;
            T[] result = new T[data.Length / sizeOfResultType];
            for (int i = 0; i < data.Length; i += sizeOfResultType)
            {
                result[i / sizeOfResultType] = (data.GetRange(i, sizeOfResultType).ToArray().ToStruct<T>());
            }
            return result;
        }
        public static int[] IndexOf(this byte[] bytes, byte[] searchBytes, int maxResults = int.MaxValue, int start = 0, int? end = null, bool nullAsWildCard = false)
        {
            List<int> result = new List<int>();
            if (end == null)
                end = bytes.Length - 2;
            for (int i = start; i < end; i++)
            {
                if (bytes[i] != searchBytes[0])
                    continue;
                bool isFullPattern = true;
                for (int j = 0; j < searchBytes.Length; j++)
                {
                    if (nullAsWildCard && searchBytes[j] == 0x0)
                        continue;
                    if (searchBytes[j] != bytes[i + j])
                    {
                        isFullPattern = false;
                        break;
                    }
                }
                if (isFullPattern)
                {
                    result.Add(i);
                    if (result.Count >= maxResults)
                        break;
                }
            }
            return result.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this byte[] bytes, params byte[] checkBytes)
        {
            return bytes.IndexOf(checkBytes, 1, nullAsWildCard: false).Length > 0;
        }

        public static byte[] EnforceLength(this byte[] bytes, int newLength)
        {
            if (bytes.Length == newLength)
            {
                return bytes;
            }
            else
            {
                byte[] ret = new byte[newLength];
                int amountToCopy = ret.Length > bytes.Length ? bytes.Length : ret.Length;
                Buffer.BlockCopy(bytes, 0, ret, 0, amountToCopy);
                return ret;
            }

        }
    }
}
