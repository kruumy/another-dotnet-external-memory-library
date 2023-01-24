using System.Runtime.CompilerServices;
using System.Text;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToByteArray(this string s, bool nullTerminate = true)
        {
            return nullTerminate ? Encoding.ASCII.GetBytes(s + "\0") : Encoding.ASCII.GetBytes(s);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char[] ToCharArray(this string s, bool nullTerminate = true)
        {
            return nullTerminate ? s.ToByteArray(true).ToStructArray<char>() : s.ToCharArray();
        }

    }
}
