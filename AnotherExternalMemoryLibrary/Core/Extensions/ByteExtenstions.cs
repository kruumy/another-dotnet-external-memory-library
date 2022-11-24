using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace AnotherExternalMemoryLibrary.Core.Extensions
{
    public static class ByteExtenstions
    {
        public static string GetString(this byte[] bytes, bool trimToNull = true)
        {
            if (trimToNull)
            {
                int length = bytes.IndexOf(new byte[] { 0x00 }, 1, false).FirstOrDefault();
                return Encoding.Default.GetString(bytes, 0, length);
            }
            else
            {
                return Encoding.Default.GetString(bytes);
            }
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
        public static string GetHexString(this byte _byte)
        {
            return new byte[] { _byte }.GetHexString();
        }
        public static T ToStruct<T>(this byte[] data) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
#pragma warning disable CS8605 // Unboxing a possibly null value.
            T val = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
#pragma warning restore CS8605 // Unboxing a possibly null value.
            handle.Free();
            return val;
        }
        public static byte[] Add(this byte[] bytes, params byte[] addBytes)
        {
            byte[] result = new byte[addBytes.Length + bytes.Length];
            bytes.CopyTo(result, 0);
            addBytes.CopyTo(result, bytes.Length);
            return result;
        }
        public static byte[] Add(this byte[] bytes, params byte[][] arrays)
        {
            byte[] result = new byte[arrays.Sum((byte[] a) => a.Length) + bytes.Length];
            bytes.CopyTo(result, 0);
            int num = bytes.Length;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, num, array.Length);
                num += array.Length;
            }
            return result;
        }
        public static int[] IndexOf(this byte[] bytes, byte[] searchBytes, int maxResults = int.MaxValue, bool nullAsBlank = false)
        {
            List<int> result = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != searchBytes[0])
                    continue;

                bool PatternCheck(int nOffset, byte[] arrPattern)
                {
                    for (int j = 0; j < arrPattern.Length; j++)
                    {
                        if (nullAsBlank && arrPattern[j] == 0x0)
                            continue;

                        if (arrPattern[j] != bytes[nOffset + j])
                            return false;
                    }
                    return true;
                }
                if (PatternCheck(i, searchBytes))
                {
                    result.Add(i);
                    if (result.Count >= maxResults)
                        break;
                }
            }
            return result.ToArray();
        }
        public static bool Contains(this byte[] bytes, params byte[] checkBytes)
        {
            return bytes.IndexOf(checkBytes, 1, false).Length > 0;
        }
        public static byte[] Compress(this byte[] bytes)
        {
            byte[] compressedBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                {
                    deflateStream.Write(bytes, 0, bytes.Length);
                }
                compressedBytes = memoryStream.ToArray();
            }
            return compressedBytes;
        }
        public static byte[] Decompress(this byte[] compressedBytes)
        {
            using MemoryStream inputStream = new MemoryStream(compressedBytes.Length);
            inputStream.Write(compressedBytes, 0, compressedBytes.Length);
            inputStream.Seek(0, SeekOrigin.Begin);
            using MemoryStream outputStream = new MemoryStream();
            using (DeflateStream deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = deflateStream.Read(buffer, 0, buffer.Length)) != 0)
                    outputStream.Write(buffer, 0, bytesRead);
            }
            return outputStream.ToArray();
        }

    }
}
