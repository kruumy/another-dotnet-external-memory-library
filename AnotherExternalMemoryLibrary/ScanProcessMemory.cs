using AnotherExternalMemoryLibrary.Extensions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class ScanProcessMemory
    {
        public static IntPtrEx[] Scan(IntPtrEx pHandle, IntPtrEx start, IntPtrEx end, params byte[] pattern)
        {
            List<IntPtrEx> ret = new List<IntPtrEx>();
            MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();
            IntPtrEx memInfoSize = Marshal.SizeOf(memInfo);
            IntPtrEx lpAddress = start;
            while (VirtualQueryEx(pHandle, lpAddress, out memInfo, memInfoSize) != 0 && lpAddress < end)
            {
                lpAddress = memInfo.BaseAddress + memInfo.RegionSize;

                int SplitNum = (int)((long)memInfo.RegionSize / int.MaxValue) + 1;
                IntPtrEx readStart = lpAddress;
                int readLength = (int)((long)memInfo.RegionSize / SplitNum);
                for (int i = 0; i < SplitNum; i++)
                {
                    byte[] data = ReadProcessMemory.Read<byte>(pHandle, readStart, readLength);
                    int[] searchResults = data.IndexOf(pattern);
                    if (searchResults.Length > 0)
                    {
                        foreach (int num in searchResults)
                        {
                            ret.Add(readStart + num);
                        }
                    }
                    readStart += readLength;
                }
            }
            return ret.ToArray();
        }
    }
}
