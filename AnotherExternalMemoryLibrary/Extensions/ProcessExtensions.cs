using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class ProcessExtensions
    {
        public static Architecture GetArchitecture(this Process process)
        {
            Win32.IsWow64Process(process.Handle, out bool IsWow64);
            return (Architecture)Convert.ToInt32(!IsWow64);
        }
        public static T Read<T>(this Process process, PointerEx address) where T : struct
        {
            return AnotherExternalMemoryLibrary.ReadProcessMemory.Read<T>(process.Handle, address);
        }
        public static T[] Read<T>(this Process process, PointerEx address, int NumOfItems) where T : struct
        {
            return AnotherExternalMemoryLibrary.ReadProcessMemory.Read<T>(process.Handle, address, NumOfItems);
        }
        public static void Write<T>(this Process process, PointerEx address, T value) where T : struct
        {
            WriteProcessMemory.Write<T>(process.Handle, address, value);
        }
        public static void Write<T>(this Process process, PointerEx address, params T[] values) where T : struct
        {
            WriteProcessMemory.Write<T>(process.Handle, address, values);
        }
        public static void Call(this Process process, PointerEx address, params object[] parameters)
        {
            switch (process.GetArchitecture())
            {
                case Architecture.X86:
                    CallProcessFunction.Callx86(process.Handle, address, parameters);
                    break;
                case Architecture.X64:
                    CallProcessFunction.Callx86(process.Handle, address, parameters);
                    break;
                default:
                    throw new Exception($"Invalid process architecture {process.GetArchitecture()}");
            }
        }
        public static void UserCall(this Process process, PointerEx address, object eax = null, object ecx = null, object edx = null, object ebx = null, object esp = null, object ebp = null, object esi = null, object edi = null)
        {
            CallProcessFunction.UserCallx86(process.Handle, address, eax, ecx, edx, ebx, esp, ebp, esi, edi);
        }
        public static void LoadLibraryA(this Process process, string dllPath)
        {
            LoadLibrary.LoadLibraryA(process.Handle, dllPath);
        }
        public static WindowController GetMainWindow(this Process process)
        {
            return new WindowController(process.MainWindowHandle);
        }
        public static PointerEx CalculateOffsets(this Process process, PointerEx BaseOffset, params PointerEx[] Offsets)
        {
            return process.CalculateOffsets(process.MainModule.BaseAddress, BaseOffset, Offsets);
        }
        public static PointerEx CalculateOffsets(this Process process, PointerEx BaseAddress, PointerEx BaseOffset, params PointerEx[] Offsets)
        {
            PointerEx result = BaseAddress + BaseOffset;
            foreach (PointerEx offset in Offsets)
                result = offset + ReadProcessMemory.Read<byte>(process.Handle, result, PointerEx.Size).ToStruct<PointerEx>();
            return result;
        }
    }
}
