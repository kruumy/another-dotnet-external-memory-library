using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static AnotherExternalMemoryLibrary.Core.Win32;

namespace AnotherExternalMemoryLibrary.Core
{
    public static class MemoryScan
    {
        public static PointerEx[] Scan(PointerEx pHandle, params byte[] pattern)
        {
            List<PointerEx> ret = new();
            PointerEx lpAddress = 0x0;
            MEMORY_BASIC_INFORMATION memInfo = new();
            PointerEx memInfoSize = Marshal.SizeOf(memInfo);
            while (VirtualQueryEx(pHandle, lpAddress, out memInfo, memInfoSize) != 0)
            {
                lpAddress = memInfo.BaseAddress + memInfo.RegionSize;
                byte[] data = new byte[memInfo.RegionSize];
                ReadProcessMemory(pHandle, lpAddress, data, data.Length, out PointerEx bytesRead);
                int[] searchResults = data.IndexOf(pattern);
                if (searchResults.Length > 0)
                {
                    foreach (int num in searchResults)
                    {
                        ret.Add(lpAddress + num);
                        //ret[^1].print();
                        //ReadProcessMemory.Read<byte>(pHandle,ret[^1],5).print();
                    }
                }
            }
            return ret.ToArray();
        }
    }
}
