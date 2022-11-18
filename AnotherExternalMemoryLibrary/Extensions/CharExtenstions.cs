namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class CharExtenstions
    {
        public static string GetString(this char[] chars)
        {
            return new string(chars);
        }
    }
}
