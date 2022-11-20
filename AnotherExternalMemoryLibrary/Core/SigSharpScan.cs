using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Diagnostics;

namespace AnotherExternalMemoryLibrary.Core
{
    public static class SigSharpScan
    {
        public static PointerEx[] Scan(PointerEx pHandle, byte[] arrPattern, ProcessModule[] processModules)
        {
            List<PointerEx> result = new List<PointerEx>();
            foreach (ProcessModule item in processModules)
            {
                PointerEx g_lpModuleBase = item.BaseAddress;
                byte[] g_arrModuleBuffer = Core.ReadProcessMemory.Read<byte>(pHandle, g_lpModuleBase, item.ModuleMemorySize);

                for (int nModuleIndex = 0; nModuleIndex < g_arrModuleBuffer.Length; nModuleIndex++)
                {
                    if (g_arrModuleBuffer[nModuleIndex] != arrPattern[0])
                        continue;

                    bool PatternCheck(int nOffset, byte[] arrPattern)
                    {
                        for (int i = 0; i < arrPattern.Length; i++)
                        {
                            if (arrPattern[i] == 0x0)
                                continue;

                            if (arrPattern[i] != g_arrModuleBuffer[nOffset + i])
                                return false;
                        }

                        return true;
                    }
                    if (PatternCheck(nModuleIndex, arrPattern))
                    {

                        result.Add(g_lpModuleBase + nModuleIndex);
                    }
                }
            }
            return result.ToArray();
        }

        public static PointerEx[] Scan(PointerEx pHandle, string query, ProcessModule[] processModules)
        {
            List<byte> patternbytes = new List<byte>();
            foreach (string szByte in query.Split(' '))
                patternbytes.Add(szByte == "?" ? (byte)0x0 : Convert.ToByte(szByte, 16));
            return Scan(pHandle, patternbytes.ToArray(), processModules);
        }
        public static PointerEx[] Scan<T>(PointerEx pHandle, T value, ProcessModule[] processModules) where T : struct
        {
            if (value is IntPtr ip) return !PointerEx.Is64Bit ? Scan(pHandle, BitConverter.GetBytes(ip.ToInt32()), processModules) : Scan(pHandle, BitConverter.GetBytes(ip.ToInt64()), processModules);
            else if (value is PointerEx ipx) return !PointerEx.Is64Bit ? Scan(pHandle, BitConverter.GetBytes((int)ipx), processModules) : Scan(pHandle, BitConverter.GetBytes((long)ipx), processModules);
            else if (value is int i) return Scan(pHandle, BitConverter.GetBytes(i), processModules);
            else if (value is uint ui) return Scan(pHandle, BitConverter.GetBytes(ui), processModules);
            else if (value is long l) return Scan(pHandle, BitConverter.GetBytes(l), processModules);
            else if (value is ulong ul) return Scan(pHandle, BitConverter.GetBytes(ul), processModules);
            else if (value is float f) return Scan(pHandle, BitConverter.GetBytes(f), processModules);
            else if (value is short s) return Scan(pHandle, BitConverter.GetBytes(s), processModules);
            else if (value is ushort us) return Scan(pHandle, BitConverter.GetBytes(us), processModules);
            else if (value is bool bo) return Scan(pHandle, BitConverter.GetBytes(bo), processModules);
            else if (value is char c) return Scan(pHandle, BitConverter.GetBytes(c), processModules);
            else if (value is double d) return Scan(pHandle, BitConverter.GetBytes(d), processModules);
            else if (value is byte b) return Scan(pHandle, new byte[] { b }, processModules);
            else if (value is sbyte sb) return Scan(pHandle, new byte[] { (byte)sb }, processModules);
            else return Scan(pHandle, value.ToByteArray<T>(), processModules);
        }
    }
}
