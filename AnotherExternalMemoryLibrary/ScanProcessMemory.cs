using AnotherExternalMemoryLibrary.Extensions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class ScanProcessMemory
    {
        public static ptr[] Scan(ptr pHandle, ptr start, ptr end, params byte[] pattern)
        {
            List<ptr> ret = new List<ptr>();
            MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();
            ptr memInfoSize = Marshal.SizeOf(memInfo);
            ptr lpAddress = start;
            while (VirtualQueryEx(pHandle, lpAddress, out memInfo, memInfoSize) != 0 && lpAddress < end)
            {
                lpAddress = memInfo.BaseAddress + memInfo.RegionSize;

                int SplitNum = (int)((long)memInfo.RegionSize / int.MaxValue) + 1;
                ptr readStart = lpAddress;
                int readLength = (int)((long)memInfo.RegionSize / SplitNum);
                for (int i = 0; i < SplitNum; i++)
                {
                    byte[] data = new byte[readLength];
                    ReadProcessMemory(pHandle, readStart, data, data.Length, out ptr bytesRead);
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
