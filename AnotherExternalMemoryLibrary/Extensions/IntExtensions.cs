using System;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class IntExtensions
    {
        public static bool CanBeByte(this int val)
        {
            try
            {
                _ = (byte)val;
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
        }
    }
}
