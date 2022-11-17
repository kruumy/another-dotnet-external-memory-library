using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary
{
    public class ProcessEx
    {
        public Process BaseProcess { get; private set; }
        public PointerEx BaseAddress => BaseProcess.MainModule?.BaseAddress ?? IntPtr.Zero;
        public PointerEx Handle { get; private set; }
        public ProcessModuleCollection Modules => BaseProcess.Modules;
        public ProcessEx(Process targetProcess)
        {
            BaseProcess = targetProcess;

            if (Utils.IsAdministrator())
                Process.EnterDebugMode();

            Handle = Win32.OpenProcess(Win32.ProcessAccess.PROCESS_ACCESS, false, BaseProcess.Id);
        }
        #region Read&Write
        public byte[] Read(PointerEx addr, int NumOfBytes)
        {
            byte[] data = new byte[NumOfBytes];
            PointerEx bytesRead = IntPtr.Zero;
            Win32.ReadProcessMemory(BaseProcess.Handle, addr, data, NumOfBytes, ref bytesRead);
            return data;
        }
        public T Read<T>(PointerEx addr)
        {
            PointerEx size = IntPtr.Zero;
            if (typeof(T) == typeof(string) || typeof(T) == typeof(char[]))
                size = 1023;
            else
                size = Marshal.SizeOf(typeof(T));
            byte[] data = Read(addr, size);

            if (typeof(T) == typeof(PointerEx) || typeof(T) == typeof(IntPtr))
                if (PointerEx.Is64Bit)
                    return (dynamic)(PointerEx)BitConverter.ToInt64(data);
                else
                    return (dynamic)(PointerEx)BitConverter.ToInt32(data);
            if (typeof(T) == typeof(float)) return (dynamic)BitConverter.ToSingle(data);
            if (typeof(T) == typeof(double)) return (dynamic)BitConverter.ToDouble(data);
            if (typeof(T) == typeof(long)) return (dynamic)BitConverter.ToInt64(data);
            if (typeof(T) == typeof(ulong)) return (dynamic)BitConverter.ToUInt64(data);
            if (typeof(T) == typeof(int)) return (dynamic)BitConverter.ToInt32(data);
            if (typeof(T) == typeof(uint)) return (dynamic)BitConverter.ToUInt32(data);
            if (typeof(T) == typeof(short)) return (dynamic)BitConverter.ToInt16(data);
            if (typeof(T) == typeof(ushort)) return (dynamic)BitConverter.ToUInt16(data);
            if (typeof(T) == typeof(bool)) return (dynamic)BitConverter.ToBoolean(data);
            if (typeof(T) == typeof(string)) return (dynamic)data.GetString();
            if (typeof(T) == typeof(char)) return (dynamic)Convert.ToChar(data[0]);
            if (typeof(T) == typeof(byte)) return (dynamic)data[0];
            if (typeof(T) == typeof(sbyte)) return (dynamic)data[0];
            throw new Exception($"Invalid Type {typeof(T)}");
        }
        public T[] Read<T>(PointerEx addr, int NumOfItems)
        {
            T[] arr = new T[NumOfItems];
            int size = Marshal.SizeOf(typeof(T));
            IEnumerable<byte> data = Read(addr, arr.Length * size);
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = data.Skip(i * size).Take(size).ToArray().ToStruct<T>();
            }
            return arr;
        }
        public void Write(PointerEx addr, byte[] bytes)
        {
            PointerEx bytesWritten = IntPtr.Zero;
            Win32.VirtualProtectEx(BaseProcess.Handle, addr, bytes.Length, Win32.MemoryProtection.ExecuteReadWrite, out int oldProtection);
            Win32.WriteProcessMemory(BaseProcess.Handle, addr, bytes, bytes.Length, ref bytesWritten);
            Win32.VirtualProtectEx(BaseProcess.Handle, addr, bytes.Length, (Win32.MemoryProtection)oldProtection, out int _);
            Console.WriteLine(oldProtection);
        }
        public void Write<T>(PointerEx addr, T value)
        {
            byte[] data = Array.Empty<byte>();
            if (value is IntPtr ip) data = !PointerEx.Is64Bit ? BitConverter.GetBytes(ip.ToInt32()) : BitConverter.GetBytes(ip.ToInt64());
            else if (value is PointerEx ipx) data = !PointerEx.Is64Bit ? BitConverter.GetBytes(ipx.IntPtr.ToInt32()) : BitConverter.GetBytes(ipx.IntPtr.ToInt64());
            else if (value is float f) data = BitConverter.GetBytes(f);
            else if (value is double d) data = BitConverter.GetBytes(d);
            else if (value is long l) data = BitConverter.GetBytes(l);
            else if (value is ulong ul) data = BitConverter.GetBytes(ul);
            else if (value is int i) data = BitConverter.GetBytes(i);
            else if (value is uint ui) data = BitConverter.GetBytes(ui);
            else if (value is short s) data = BitConverter.GetBytes(s);
            else if (value is ushort u) data = BitConverter.GetBytes(u);
            else if (value is bool bo) data = BitConverter.GetBytes(bo);
            else if (value is char c) data = BitConverter.GetBytes(c);
            else if (value is string str) data = str.ToByteArray();
            else if (value is byte b) data = new byte[] { b };
            else if (value is sbyte sb) data = new byte[] { (byte)sb };
            else if (value is byte[] ba) data = ba;
            else throw new Exception($"Invalid Type {typeof(T)}");
            Write(addr, data);
        }
        public void Write<T>(PointerEx addr, T[] array)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] writeData = new byte[size * array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i].ToByteArray().CopyTo(writeData, i * size);
            }
            Write(addr, writeData);
        }
        #endregion
        #region Scanning
        public PointerEx[] Scan(byte[] arrPattern, ProcessModule? targetModule = null)
        {
            List<ProcessModule> processModules = new List<ProcessModule>();
            if (targetModule == null)
                foreach (ProcessModule pm in Modules)
                    processModules.Add(pm);
            else
                processModules.Add(targetModule);

            List<PointerEx> result = new List<PointerEx>();
            foreach (ProcessModule item in processModules)
            {
                PointerEx g_lpModuleBase = item.BaseAddress;
                byte[] g_arrModuleBuffer = Read(g_lpModuleBase, item.ModuleMemorySize);

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
        public PointerEx[] Scan(string query, ProcessModule? targetModule = null)
        {
            List<byte> patternbytes = new List<byte>();
            foreach (var szByte in query.Split(' '))
                patternbytes.Add(szByte == "?" ? (byte)0x0 : Convert.ToByte(szByte, 16));
            return Scan(patternbytes.ToArray(), targetModule);
        }
        public PointerEx[] Scan<T>(T value, ProcessModule? targetModule = null)
        {
            if (value is IntPtr ip) return !PointerEx.Is64Bit ? Scan(BitConverter.GetBytes(ip.ToInt32()), targetModule) : Scan(BitConverter.GetBytes(ip.ToInt64()), targetModule);
            else if (value is PointerEx ipx) return !PointerEx.Is64Bit ? Scan(BitConverter.GetBytes(ipx.IntPtr.ToInt32()), targetModule) : Scan(BitConverter.GetBytes(ipx.IntPtr.ToInt64()), targetModule);
            else if (value is string str) return Scan(str.ToByteArray(), targetModule);
            else if (value is int i) return Scan(BitConverter.GetBytes(i), targetModule);
            else if (value is uint ui) return Scan(BitConverter.GetBytes(ui), targetModule);
            else if (value is long l) return Scan(BitConverter.GetBytes(l), targetModule);
            else if (value is ulong ul) return Scan(BitConverter.GetBytes(ul), targetModule);
            else if (value is float f) return Scan(BitConverter.GetBytes(f), targetModule);
            else if (value is short s) return Scan(BitConverter.GetBytes(s), targetModule);
            else if (value is ushort us) return Scan(BitConverter.GetBytes(us), targetModule);
            else if (value is bool bo) return Scan(BitConverter.GetBytes(bo), targetModule);
            else if (value is char c) return Scan(BitConverter.GetBytes(c), targetModule);
            else if (value is double d) return Scan(BitConverter.GetBytes(d), targetModule);
            else if (value is byte b) return Scan(new byte[] { b }, targetModule);
            else if (value is sbyte sb) return Scan(new byte[] { (byte)sb }, targetModule);
            else if (value is byte[] ba) return Scan(ba, targetModule);
            else throw new Exception($"Invalid Type {typeof(T)}");
        }
        #endregion
        #region Misc
        public PointerEx this[PointerEx BaseOffset]
        {
            get
            {
                return BaseAddress + BaseOffset;
            }
        }
        public ProcessModule this[string name]
        {
            get
            {
                return Modules.GetByName(name);
            }
        }
        #endregion
    }
}