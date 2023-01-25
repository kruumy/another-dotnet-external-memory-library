﻿using AnotherExternalMemoryLibrary.Extensions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public static class ScanProcessMemory
    {
        // TODO: make this acually good, implement a multithreaded aob scan
        public static UIntPtrEx[] Scan(IntPtrEx pHandle, UIntPtrEx start, UIntPtrEx end, bool nullAsWildCard, params byte[] pattern)
        {
            List<UIntPtrEx> ret = new List<UIntPtrEx>();
            MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();
            UIntPtrEx memInfoSize = Marshal.SizeOf(memInfo);
            UIntPtrEx lpAddress = start;
            while (VirtualQueryEx(pHandle, lpAddress, out memInfo, memInfoSize) != 0 && lpAddress < end)
            {
                lpAddress = memInfo.BaseAddress + memInfo.RegionSize;

                int SplitNum = (int)((long)memInfo.RegionSize / int.MaxValue) + 1;
                UIntPtrEx readStart = lpAddress;
                int readLength = (int)((long)memInfo.RegionSize / SplitNum);
                for (int i = 0; i < SplitNum; i++)
                {
                    byte[] data = ReadProcessMemory.Read<byte>(pHandle, readStart, readLength);
                    int[] searchResults = data.IndexOf(pattern, nullAsWildCard: nullAsWildCard);
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
