using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class ProcessExtensions
    {
        /// <summary>
        /// Calculates a pointer to an address in the process memory.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="BaseOffset">Offset of the main module</param>
        /// <param name="Offsets">Offsets to read from that point to the absolute address</param>
        /// <returns>Absolute address</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtrEx CalculatePointer(this Process process, IntPtrEx BaseAddress, IntPtrEx BaseOffset, params IntPtrEx[] Offsets)
        {
            IntPtrEx result = BaseAddress + BaseOffset;
            foreach (IntPtrEx offset in Offsets)
            {
                result = offset + ReadProcessMemory.Read<IntPtrEx>(process.Handle, result);
            }
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtrEx CalculatePointer(this Process process, string ModuleName, IntPtrEx ModuleOffset, params IntPtrEx[] Offsets)
        {
            return process.CalculatePointer(process.Modules.GetByName(ModuleName).BaseAddress, ModuleOffset, Offsets);
        }

        /// <summary>
        /// Determine if the process is x64 or x86.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <returns>enum</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Architecture GetArchitecture(this Process process)
        {
            Win32.IsWow64Process(process.Handle, out bool IsWow64);
            return (Architecture)Convert.ToInt32(!IsWow64);
        }

        /// <summary>
        /// Get a WindowController object to control the main window of the process.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <returns>WindowController object</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static WindowController GetMainWindow(this Process process)
        {
            return new WindowController(process.MainWindowHandle);
        }

        /// <summary>
        /// Load a dll into the process.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="dllPath">Path of dll</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LoadLibraryA(this Process process, string dllPath)
        {
            LoadLibrary.LoadLibraryA(process.Handle, dllPath);
        }

        /// <summary>
        /// Read from the process memory.
        /// </summary>
        /// <typeparam name="T">Type of value to read</typeparam>
        /// <param name="process">Target process</param>
        /// <param name="address">Address to read from</param>
        /// <returns>value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(this Process process, IntPtrEx address) where T : unmanaged
        {
            return ReadProcessMemory.Read<T>(process.Handle, address);
        }

        /// <summary>
        /// Read an array from the process memory.
        /// </summary>
        /// <typeparam name="T">Type of value to read</typeparam>
        /// <param name="process">Target process</param>
        /// <param name="address">Address to read from</param>
        /// <param name="NumOfItems">Number of items to read</param>
        /// <returns>values</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Read<T>(this Process process, IntPtrEx address, int NumOfItems) where T : unmanaged
        {
            return ReadProcessMemory.Read<T>(process.Handle, address, NumOfItems);
        }

        /// <summary>
        /// Reads characters until hitting a '\0'
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="address">Address to write to</param>
        /// <param name="maxLength">Max length to read</param>
        /// <param name="buffSize">Amount of characters to read each loop</param>
        /// <returns>A managed string of characters. Does not contain the null terminate character.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(this Process process, IntPtrEx address, int maxLength = 1023, int buffSize = 64)
        {
            return ReadProcessMemory.ReadString(process.Handle, address, maxLength, buffSize);
        }

        /// <summary>
        /// Scan the process memory.
        /// </summary>
        /// <param name="process">Target process</param>
        /// <param name="start">Starting point</param>
        /// <param name="end">Ending point</param>
        /// <param name="pattern">Pattern to search for</param>
        /// <returns>All matches</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UIntPtrEx[] Scan(this Process process, UIntPtrEx start, UIntPtrEx end, params byte[] pattern)
        {
            return ScanProcessMemory.Scan(process.Handle, start, end, pattern);
        }

        /// <summary>
        /// Write a value to the process memory.
        /// </summary>
        /// <typeparam name="T">Type of value to write</typeparam>
        /// <param name="process">Target process</param>
        /// <param name="address">Address to write to</param>
        /// <param name="value">What to write</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(this Process process, IntPtrEx address, T value) where T : unmanaged
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(this Process process, IntPtrEx address, params T[] values) where T : unmanaged
        {
            WriteProcessMemory.Write<T>(process.Handle, address, values);
        }
    }
}
