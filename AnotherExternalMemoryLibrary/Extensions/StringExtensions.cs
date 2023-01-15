using System.Text;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ToByteArray(this string s, bool nullTerminate = true)
        {
            return nullTerminate ? Encoding.ASCII.GetBytes(s + "\0") : Encoding.ASCII.GetBytes(s);
        }

        public static char[] ToCharArray(this string s, bool nullTerminate = true)
        {
            return nullTerminate ? s.ToByteArray(true).ToStructArray<char>() : s.ToCharArray();
        }

    }
}
