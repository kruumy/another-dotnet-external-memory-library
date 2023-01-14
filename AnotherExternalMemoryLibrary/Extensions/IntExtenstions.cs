using System;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class IntExtenstions
    {
        public static bool CanBeByte(this int val)
        {
            try
            {
                byte _ = (byte)val;
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
        }
    }
}
