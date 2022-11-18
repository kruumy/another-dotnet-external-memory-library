namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class Chars
    {
        public static string GetString(this char[] chars)
        {
            return new string(chars);
        }
    }
}
