using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
