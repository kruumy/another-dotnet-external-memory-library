using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AnotherExternalMemoryLibrary
{
    public static class ScanProcessMemory
    {
        public static IEnumerable<IntPtrEx> Scan( IntPtrEx pHandle, ProcessModule processModule, bool nullAsWildCard, params byte[] pattern )
        {
            return Scan( pHandle, processModule, nullAsWildCard, progressCallBack: null, pattern );
        }
        public static IEnumerable<IntPtrEx> Scan( IntPtrEx pHandle, ProcessModule processModule, bool nullAsWildCard, Action<float> progressCallBack, params byte[] pattern )
        {
            int bufferSize = CalculateBufferSize(processModule.ModuleMemorySize);
            IntPtrEx address = processModule.BaseAddress;
            IntPtrEx endAddress = address + processModule.ModuleMemorySize;

            float totalMemory = processModule.ModuleMemorySize;
            float oldProgress = 0;

            while ( address < endAddress )
            {
                if ( progressCallBack != null )
                {
                    float progress = (int)(address - processModule.BaseAddress) / totalMemory;
                    if ( progress - oldProgress > 0.01f )
                    {
                        oldProgress = progress;
                        progressCallBack(progress);
                    }
                }

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

            if ( progressCallBack != null )
            {
                progressCallBack(1f);
            }
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
