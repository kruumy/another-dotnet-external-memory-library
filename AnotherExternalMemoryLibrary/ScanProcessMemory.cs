using System.Collections.Generic;
using System.Diagnostics;

namespace AnotherExternalMemoryLibrary
{
    public static class ScanProcessMemory
    {
        public static IEnumerable<IntPtrEx> Scan( IntPtrEx pHandle, ProcessModule processModule, bool nullAsWildCard, params byte[] pattern )
        {
            IntPtrEx address = processModule.BaseAddress;
            IntPtrEx endAddress = address + processModule.ModuleMemorySize;
            while ( address < endAddress )
            {
                byte[] buffer = ReadProcessMemory.Read<byte>(pHandle, address, pattern.Length);
                if ( ByteArrayCompare(buffer, pattern, nullAsWildCard) )
                {
                    yield return address;
                }
                address += 1;
            }
            yield return IntPtrEx.Zero;
        }

        private static bool ByteArrayCompare( byte[] a1, byte[] a2, bool nullAsWildCard )
        {
            if ( a1.Length != a2.Length )
            {
                return false;
            }

            for ( int i = 0; i < a1.Length; i++ )
            {
                if ( a1[ i ] != a2[ i ] )
                {
                    if ( nullAsWildCard )
                    {
                        if ( a2[ i ] != 0x00 )
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
            }

            return true;
        }
    }
}
