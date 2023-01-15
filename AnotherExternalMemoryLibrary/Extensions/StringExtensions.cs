using System.Linq;
using System.Text;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ToByteArray(this string s)
        {
            return Encoding.ASCII.GetBytes(s).Append<byte>(0).ToArray();
        }

    }
}
