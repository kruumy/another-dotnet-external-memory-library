using AnotherExternalMemoryLibrary.Extensions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class ScanProcessMemory
    {
        public static PointerEx[] Scan(PointerEx pHandle, PointerEx start, PointerEx end, params byte[] pattern)
        {
            List<PointerEx> ret = new List<PointerEx>();
            MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();
            PointerEx memInfoSize = Marshal.SizeOf(memInfo);
            PointerEx lpAddress = start;
            while (VirtualQueryEx(pHandle, lpAddress, out memInfo, memInfoSize) != 0 && lpAddress < end)
            {
                lpAddress = memInfo.BaseAddress + memInfo.RegionSize;

                int SplitNum = (int)((long)memInfo.RegionSize / int.MaxValue) + 1;
                PointerEx readStart = lpAddress;
                int readLength = (int)((long)memInfo.RegionSize / SplitNum);
                for (int i = 0; i < SplitNum; i++)
                {
                    byte[] data = new byte[readLength];
                    ReadProcessMemory(pHandle, readStart, data, data.Length, out PointerEx bytesRead);
                    int[] searchResults = data.IndexOf(pattern);
                    if (searchResults.Length > 0)
                    {
                        foreach (int num in searchResults)
                        {
                            ret.Add(readStart + num);
                            //ReadProcessMemory.Read<byte>(pHandle,ret[^1],pattern.Length).print();
                        }
                    }
                    readStart += readLength;
                }
            }
            return ret.ToArray();
        }
    }
}
