using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static AnotherExternalMemoryLibrary.Core.Win32;

namespace AnotherExternalMemoryLibrary.Core
{
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

                int SplitNum = (int)((long)memInfo.RegionSize / int.MaxValue) + 1;
                PointerEx readStart = lpAddress;
                int readLength = (int)((long)memInfo.RegionSize / SplitNum);
                for (int i = 0; i < SplitNum; i++)
                {
                    byte[] data = new byte[readLength];
                    ReadProcessMemory(pHandle, readStart, data, data.Length, out PointerEx bytesRead);
                    Debug.WriteLine($"started, {readStart}--{(PointerEx)readLength}/{SplitNum}, {memInfo.RegionSize}");
                    int[] searchResults = data.IndexOf(pattern);
                    Debug.WriteLine($"stopped, {searchResults.Length}");
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
        public static PointerEx[] Scan(PointerEx pHandle, params byte[] pattern)
        {
            return Scan(pHandle, 0x0, PointerEx.MaxValue, pattern);
        }
        public static PointerEx[] Scan(PointerEx pHandle, int NumOfThreads, params byte[] pattern)
        {
            // TODO: Recieves duplicate values cause VirtualQueryEx rounds to base address, fix it.
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
