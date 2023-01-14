using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class ProcessExtensions
    {
        /// <summary>
        /// Determine if the process is x64 or x86.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <returns>enum</returns>
        public static Architecture GetArchitecture(this Process process)
        {
            Win32.IsWow64Process(process.Handle, out bool IsWow64);
            return (Architecture)Convert.ToInt32(!IsWow64);
        }
        /// <summary>
        /// Read from the process memory.
        /// </summary>
        /// <typeparam name="T">Type of value to read</typeparam>
        /// <param name="process">Target process</param>
        /// <param name="address">Address to read from</param>
        /// <returns>value</returns>
        public static T Read<T>(this Process process, IntPtrEx address) where T : struct
        {
            return AnotherExternalMemoryLibrary.ReadProcessMemory.Read<T>(process.Handle, address);
        }
        /// <summary>
        /// Read an array from the process memory.
        /// </summary>
        /// <typeparam name="T">Type of value to read</typeparam>
        /// <param name="process">Target process</param>
        /// <param name="address">Address to read from</param>
        /// <param name="NumOfItems">Number of items to read</param>
        /// <returns>values</returns>
        public static T[] Read<T>(this Process process, IntPtrEx address, int NumOfItems) where T : struct
        {
            return AnotherExternalMemoryLibrary.ReadProcessMemory.Read<T>(process.Handle, address, NumOfItems);
        }
        /// <summary>
        /// Write a value to the process memory.
        /// </summary>
        /// <typeparam name="T">Type of value to write</typeparam>
        /// <param name="process">Target process</param>
        /// <param name="address">Address to write to</param>
        /// <param name="value">What to write</param>
        public static void Write<T>(this Process process, IntPtrEx address, T value) where T : struct
        {
            WriteProcessMemory.Write<T>(process.Handle, address, value);
        }
        /// <summary>
        /// Write an array to the process memory.
        /// </summary>
        /// <typeparam name="T">Type of value to write</typeparam>
        /// <param name="process">Target process</param>
        /// <param name="address">Address to write to</param>
        /// <param name="values">What to write</param>
        public static void Write<T>(this Process process, IntPtrEx address, params T[] values) where T : struct
        {
            WriteProcessMemory.Write<T>(process.Handle, address, values);
        }
        /// <summary>
        /// Scan the process memory.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="start">Starting point</param>
        /// <param name="end">Ending point</param>
        /// <param name="pattern">Pattern to search for</param>
        /// <returns>All matches</returns>
        public static IntPtrEx[] Scan(this Process process, IntPtrEx start, IntPtrEx end, params byte[] pattern)
        {
            return ScanProcessMemory.Scan(process.Handle, start, end, pattern);
        }
        /// <summary>
        /// Create a thread to call a function in the process memory.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="address">Address of function</param>
        /// <param name="parameters">All parameters to pass to the function if any</param>
        public static void Call(this Process process, IntPtrEx address, params object[] parameters)
        {
            switch (process.GetArchitecture())
            {
                case Architecture.X86:
                    CallProcessFunction.Callx86(process.Handle, address, parameters);
                    break;
                case Architecture.X64:
                    CallProcessFunction.Callx64(process.Handle, address, parameters);
                    break;
                default:
                    throw new Exception($"Invalid process architecture {process.GetArchitecture()}");
            }
        }
        /// <summary>
        /// Create a thread to call a function in the process memory.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="address">Address of function</param>
        /// <param name="parameters">All parameters to pass to the function if any</param>
        public static void UserCall(this Process process, IntPtrEx address, params object[] parameters)
        {
            CallProcessFunction.UserCallx86(process.Handle, address, parameters);
        }
        /// <summary>
        /// Load a dll into the process.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="dllPath">Path of dll</param>
        public static void LoadLibraryA(this Process process, string dllPath)
        {
            LoadLibrary.LoadLibraryA(process.Handle, dllPath);
        }
        /// <summary>
        /// Get a WindowController object to control the main window of the process.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <returns>WindowController object</returns>
        public static WindowController GetMainWindow(this Process process)
        {
            return new WindowController(process.MainWindowHandle);
        }
        /// <summary>
        /// Calculates a pointer to an address in the process memory.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="BaseOffset">Offset of the main module</param>
        /// <param name="Offsets">Offsets to read from that point to the absolute address</param>
        /// <returns>Absolute address</returns>
        public static IntPtrEx CalculatePointer(this Process process, IntPtrEx BaseOffset, params IntPtrEx[] Offsets)
        {
            return process.CalculatePointer(process.MainModule.BaseAddress, BaseOffset, Offsets);
        }
        /// <summary>
        /// Calculates a pointer to an address in the process memory.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="BaseAddress">Address to start from</param>
        /// <param name="BaseOffset">Offset of the BaseAddress</param>
        /// <param name="Offsets">Offsets to read from that point to the absolute address</param>
        /// <returns>Absolute address</returns>
        public static IntPtrEx CalculatePointer(this Process process, IntPtrEx BaseAddress, IntPtrEx BaseOffset, params IntPtrEx[] Offsets)
        {
            IntPtrEx result = BaseAddress + BaseOffset;
            foreach (IntPtrEx offset in Offsets)
                result = offset + ReadProcessMemory.Read<byte>(process.Handle, result, IntPtrEx.Size).ToStruct<IntPtrEx>();
            return result;
        }
        /// <summary>
        /// Calculates a pointer to an address in the process memory.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="ModuleName">Name of module to start from</param>
        /// <param name="ModuleOffset">Offset of the module</param>
        /// <param name="Offsets">Offsets to read from that point to the absolute address</param>
        /// <returns>Absolute address</returns>
        public static IntPtrEx CalculatePointer(this Process process, string ModuleName, IntPtrEx ModuleOffset, params IntPtrEx[] Offsets)
        {
            return process.CalculatePointer(process.Modules.GetByName(ModuleName).BaseAddress, ModuleOffset, Offsets);
        }
    }
}
