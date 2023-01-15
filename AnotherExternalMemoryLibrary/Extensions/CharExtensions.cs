namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class CharExtensions
    {
        public static string GetString(this char[] chars)
        {
            return new string(chars);
        }
    }
}
