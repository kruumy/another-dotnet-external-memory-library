using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AnotherExternalMemoryLibrary
{
    public static class ScanProcessMemory
    {
        public static IEnumerable<IntPtrEx> Scan( IntPtrEx pHandle, ProcessModule processModule, bool nullAsWildCard, params byte[] pattern )
        {
            int bufferSize = CalculateBufferSize(processModule.ModuleMemorySize);
            IntPtrEx address = processModule.BaseAddress;
            IntPtrEx endAddress = address + processModule.ModuleMemorySize;


            while ( address < endAddress )
            {
                int remainingBytes = (int)(endAddress - address);
                int bytesToRead = Math.Min(bufferSize, remainingBytes);

                byte[] buffer = ReadProcessMemory.Read<byte>(pHandle, address, bytesToRead);
                if ( buffer.Length == 0 )
                    yield break;

                for ( int offset = 0; offset < buffer.Length - pattern.Length + 1; offset++ )
                {
                    if ( ByteArrayCompare(buffer, offset, pattern, nullAsWildCard) )
                    {
                        yield return address + offset;
                    }
                }

                address += buffer.Length;
            }

            yield return IntPtrEx.Zero;
        }

        private static int CalculateBufferSize( int moduleSize )
        {
            return Math.Min(4096, moduleSize);
        }

        private static bool ByteArrayCompare( byte[] buffer, int offset, byte[] pattern, bool nullAsWildCard )
        {
            for ( int i = 0; i < pattern.Length; i++ )
            {
                if ( pattern[ i ] != buffer[ offset + i ] )
                {
                    if ( nullAsWildCard )
                    {
                        if ( pattern[ i ] != 0x00 )
                            return false;
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
