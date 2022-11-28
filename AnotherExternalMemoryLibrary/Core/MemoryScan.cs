using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Runtime.InteropServices;
using static AnotherExternalMemoryLibrary.Core.Win32;

namespace AnotherExternalMemoryLibrary.Core
{
    // Does not support x64 yet
    public static class MemoryScan
    {
        public static PointerEx[] Scan(PointerEx pHandle, PointerEx start, PointerEx end, params byte[] pattern)
        {
            List<PointerEx> ret = new();
            MEMORY_BASIC_INFORMATION memInfo = new();
            PointerEx memInfoSize = Marshal.SizeOf(memInfo);
            PointerEx lpAddress = start;
            while (VirtualQueryEx(pHandle, lpAddress, out memInfo, memInfoSize) != 0 || lpAddress >= end)
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
                        //ReadProcessMemory.Read<byte>(pHandle,ret[^1],5).print();
                    }
                }
            }
            return ret.ToArray();
        }
        public static PointerEx[] Scan(PointerEx pHandle, params byte[] pattern)
        {
            return Scan(pHandle, 0x0, PointerEx.MaxValue, pattern);
        }
        public static PointerEx[] Scan(PointerEx pHandle, int NumOfThreads, params byte[] pattern)
        {
            List<PointerEx> ret = new();
            Thread[] threads = new Thread[NumOfThreads];
            PointerEx start = 0x0;
            PointerEx end = PointerEx.MaxValue / NumOfThreads;

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    ret.AddRange(Scan(pHandle, start, end, pattern));
                });
                threads[i].Start();
                start += PointerEx.MaxValue / NumOfThreads;
                end += PointerEx.MaxValue / NumOfThreads;
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            return ret.ToArray();
        }
    }
}
