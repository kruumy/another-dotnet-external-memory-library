using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Collections.Generic;
using System.Diagnostics;

namespace AnotherExternalMemoryLibrary.Core
{
    public static class MemoryScan
    {
        public static IEnumerable<PointerEx> Scan(PointerEx pHandle,IEnumerable<ProcessModule> modules, params byte[] pattern)
        {
            List<PointerEx> result = new();
            foreach (ProcessModule item in modules)
            {
                PointerEx g_lpModuleBase = item.BaseAddress;
                byte[] g_arrModuleBuffer = Core.ReadProcessMemory.Read<byte>(pHandle, g_lpModuleBase, item.ModuleMemorySize);
                IEnumerable<int> scanResult = g_arrModuleBuffer.IndexOf(pattern, int.MaxValue, nullAsBlank: true);
                foreach (var ptr in scanResult)
                    result.Add(g_lpModuleBase + ptr);
            }
            return result;
        }
    }
}
