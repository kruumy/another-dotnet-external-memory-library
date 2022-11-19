using AnotherExternalMemoryLibrary.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static AnotherExternalMemoryLibrary.Win32;

namespace AnotherExternalMemoryLibrary
{
    public class ProcessEx : IDisposable
    {
        #region Properties
        public Process BaseProcess { get; private set; }
        public PointerEx BaseAddress => BaseProcess.MainModule?.BaseAddress ?? IntPtr.Zero;
        public PointerEx Handle { get; private set; }
        public Architecture Architecture { get; private set; }
        #endregion
        public ProcessEx(Process baseProcess, ProcessAccess dwDesiredAccess = ProcessAccess.PROCESS_ALL_ACCESS)
        {
            if (Utils.IsAdministrator())
                Process.EnterDebugMode();

            BaseProcess = baseProcess ?? throw new ArgumentNullException(nameof(baseProcess));

            Handle = OpenProcess(dwDesiredAccess, false, BaseProcess.Id);

            IsWow64Process(Handle, out bool IsWow64);
            Architecture = (Architecture)Convert.ToInt32(!IsWow64);
        }
        #region Read&Write

        /// <summary>
        /// Reads Process Memory (Value)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="addr">Absolute address</param>
        /// <returns>Value of Type(</returns>
        public T Read<T>(PointerEx addr) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] data = Read<byte>(addr, size);

            if (typeof(T) == typeof(PointerEx) || typeof(T) == typeof(IntPtr))
                if (PointerEx.Is64Bit)
                    return (dynamic)(PointerEx)BitConverter.ToInt64(data);
                else
                    return (dynamic)(PointerEx)BitConverter.ToInt32(data);
            else if (typeof(T) == typeof(float)) return (dynamic)BitConverter.ToSingle(data);
            else if (typeof(T) == typeof(double)) return (dynamic)BitConverter.ToDouble(data);
            else if (typeof(T) == typeof(long)) return (dynamic)BitConverter.ToInt64(data);
            else if (typeof(T) == typeof(ulong)) return (dynamic)BitConverter.ToUInt64(data);
            else if (typeof(T) == typeof(int)) return (dynamic)BitConverter.ToInt32(data);
            else if (typeof(T) == typeof(uint)) return (dynamic)BitConverter.ToUInt32(data);
            else if (typeof(T) == typeof(short)) return (dynamic)BitConverter.ToInt16(data);
            else if (typeof(T) == typeof(ushort)) return (dynamic)BitConverter.ToUInt16(data);
            else if (typeof(T) == typeof(bool)) return (dynamic)BitConverter.ToBoolean(data);
            else if (typeof(T) == typeof(char)) return (dynamic)Convert.ToChar(data[0]);
            else if (typeof(T) == typeof(byte)) return (dynamic)data[0];
            else if (typeof(T) == typeof(sbyte)) return (dynamic)data[0];
            else return data.ToStruct<T>();
        }
        /// <summary>
        /// Reads Process Memory (Array)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="addr">Absolute address</param>
        /// <param name="NumOfItems">Number of items to read</param>
        /// <returns>Array of Type</returns>
        public T[] Read<T>(PointerEx addr, int NumOfItems) where T : struct
        {
            if (typeof(T) == typeof(byte)) { return (dynamic)Win32.ReadProcessMemory(Handle, addr, NumOfItems); }

            T[] arr = new T[NumOfItems];
            int size = Marshal.SizeOf(typeof(T));
            IEnumerable<byte> data = Read<byte>(addr, arr.Length * size);
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = data.Skip(i * size).Take(size).ToArray().ToStruct<T>();
            }
            return arr;
        }
        /// <summary>
        /// Writes Process Memory (Value)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="addr">Absolute address</param>
        /// <param name="value">Value of type to write</param>
        public void Write<T>(PointerEx addr, T value) where T : struct
        {
            byte[] data = Array.Empty<byte>();
            if (value is IntPtr ip) data = !PointerEx.Is64Bit ? BitConverter.GetBytes(ip.ToInt32()) : BitConverter.GetBytes(ip.ToInt64());
            else if (value is PointerEx ipx) data = !PointerEx.Is64Bit ? BitConverter.GetBytes((int)ipx) : BitConverter.GetBytes((long)ipx);
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
            else if (value is byte b) data = new byte[] { b };
            else if (value is sbyte sb) data = new byte[] { (byte)sb };
            else data = value.ToByteArray<T>();
            Write<byte>(addr, data);
        }
        /// <summary>
        /// Writes Process Memory (Array)
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="addr">Absolute address</param>
        /// <param name="array">Array of type to write</param>
        public void Write<T>(PointerEx addr, T[] array) where T : struct
        {
            if (array is byte[] ba) { Win32.WriteProcessMemory(Handle, addr, ba); return; }

            int size = Marshal.SizeOf(typeof(T));
            byte[] writeData = new byte[size * array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i].ToByteArray().CopyTo(writeData, i * size);
            }
            Write<byte>(addr, writeData);
        }
        #endregion
        #region Scanning
        public PointerEx[] Scan(byte[] arrPattern, ProcessModule? targetModule = null)
        {
            List<ProcessModule> processModules = new List<ProcessModule>();
            if (targetModule == null)
                foreach (ProcessModule pm in BaseProcess.Modules)
                    processModules.Add(pm);
            else
                processModules.Add(targetModule);

            List<PointerEx> result = new List<PointerEx>();
            foreach (ProcessModule item in processModules)
            {
                PointerEx g_lpModuleBase = item.BaseAddress;
                byte[] g_arrModuleBuffer = Read<byte>(g_lpModuleBase, item.ModuleMemorySize);

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
            foreach (string szByte in query.Split(' '))
                patternbytes.Add(szByte == "?" ? (byte)0x0 : Convert.ToByte(szByte, 16));
            return Scan(patternbytes.ToArray(), targetModule);
        }
        public PointerEx[] Scan<T>(T value, ProcessModule? targetModule = null) where T : struct
        {
            if (value is IntPtr ip) return !PointerEx.Is64Bit ? Scan(BitConverter.GetBytes(ip.ToInt32()), targetModule) : Scan(BitConverter.GetBytes(ip.ToInt64()), targetModule);
            else if (value is PointerEx ipx) return !PointerEx.Is64Bit ? Scan(BitConverter.GetBytes((int)ipx), targetModule) : Scan(BitConverter.GetBytes((long)ipx), targetModule);
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
            else return Scan(value.ToByteArray<T>());
        }
        #endregion
        #region Indexers
        /// <summary>
        /// Adds Offset To BaseAddress
        /// </summary>
        /// <param name="BaseOffset">Offset for BaseAddress</param>
        /// <returns>Absolute Address</returns>
        public PointerEx this[PointerEx BaseOffset] => BaseAddress + BaseOffset;
        /// <summary>
        /// Adds Offsets to get Pointer
        /// Uses Utils.OffsetCalculator()
        /// </summary>
        /// <param name="BaseOffset">Offset for BaseAddress</param>
        /// <param name="Offsets">Offsets for Pointer</param>
        /// <returns>Absolute Address</returns>
        public PointerEx this[PointerEx BaseOffset, params PointerEx[] Offsets] => Utils.OffsetCalculator(Handle, BaseAddress, BaseOffset, Offsets);
        /// <summary>
        /// Adds Offsets to get Pointer
        /// Uses Utils.OffsetCalculator()
        /// </summary>
        /// <param name="ModuleName">Name of target module</param>
        /// <param name="ModuleOffset">Offset for ModuleAddress</param>
        /// <param name="Offsets">Offsets for Pointer</param>
        /// <returns>Absolute Address</returns>
        public PointerEx this[string ModuleName, PointerEx ModuleOffset, params PointerEx[] Offsets] => Utils.OffsetCalculator(Handle, this[ModuleName], ModuleOffset, Offsets);
        /// <summary>
        /// Gets Module Address in BaseProcess that matches name
        /// </summary>
        /// <param name="ModuleName">Module Name</param>
        /// <returns>Module BaseAddress</returns>
        public PointerEx this[string ModuleName] => BaseProcess.Modules.GetByName(ModuleName).BaseAddress;
        #endregion
        #region Misc
        public void Dump(string? path = null)
        {
            //TODO: make sure it works
            path ??= $"{BaseProcess.ProcessName}_{BaseProcess.UserProcessorTime.ToString().Replace(':', '_')}.dmp";
            if (File.Exists(path)) File.Delete(path);

            Win32.SYSTEM_INFO sys_info = new Win32.SYSTEM_INFO();
            Win32.GetSystemInfo(out sys_info);

            PointerEx proc_min_address = BaseAddress;
            PointerEx proc_max_address = BaseProcess.PrivateMemorySize64 + proc_min_address;

            PointerEx i = proc_min_address;
            MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();
            while (i < proc_max_address)
            {
                VirtualQueryEx(BaseProcess.Handle, sys_info.lpMinimumApplicationAddress, out memInfo, sys_info.dwPageSize);

                byte[] bytes = Read<byte>(i, memInfo.RegionSize);
                Utils.AppendAllBytes(path, bytes);

                i += memInfo.RegionSize;
            }
        }

        public void Dispose()
        {
            CloseHandle(Handle);
            if (Utils.IsAdministrator())
                Process.LeaveDebugMode();
            BaseProcess.Dispose();
            GC.SuppressFinalize(this);
        }

        public static implicit operator ProcessEx(Process p)
        {
            return new ProcessEx(p);
        }
        public override string ToString()
        {
            return @$"{BaseProcess.ProcessName} - {Architecture}";
        }
        #endregion
    }
}