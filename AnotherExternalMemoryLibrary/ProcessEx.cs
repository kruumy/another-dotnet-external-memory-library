using AnotherExternalMemoryLibrary.Core;
using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static AnotherExternalMemoryLibrary.Core.Win32;

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
            if (Misc.IsAdministrator)
                Process.EnterDebugMode();

            BaseProcess = baseProcess ?? throw new ArgumentNullException(nameof(baseProcess));

            Handle = OpenProcess(dwDesiredAccess, false, BaseProcess.Id);

            IsWow64Process(Handle, out bool IsWow64);
            Architecture = (Architecture)Convert.ToInt32(!IsWow64);

            Window = new Core.Window(BaseProcess.MainWindowHandle);
        }
        public static implicit operator ProcessEx(Process p) => new ProcessEx(p);
        public Core.Window Window { get; private set; }
        public T Read<T>(PointerEx addr) where T : struct => Core.ReadProcessMemory.Read<T>(Handle, addr);
        public T[] Read<T>(PointerEx addr, int NumOfItems) where T : struct => Core.ReadProcessMemory.Read<T>(Handle, addr, NumOfItems);
        public void Write<T>(PointerEx addr, T value) where T : struct => Core.WriteProcessMemory.Write<T>(Handle, addr, value);
        public void Write<T>(PointerEx addr, params T[] array) where T : struct => Core.WriteProcessMemory.Write<T>(Handle, addr, array);
        public PointerEx[] Scan(byte[] arrPattern, ProcessModule[] targetModules) => Core.SigSharpScan.Scan(Handle, arrPattern, targetModules);
        public PointerEx[] Scan(string query, ProcessModule[] targetModules) => Core.SigSharpScan.Scan(Handle, query, targetModules);
        public PointerEx[] Scan<T>(T value, ProcessModule[] targetModules) where T : struct => Core.SigSharpScan.Scan<T>(Handle, value, targetModules);
        public void LoadLibraryA(string dllPath) => Core.DLLInjection.LoadLibraryA(Handle, dllPath);
        public void UserCallx86(PointerEx addr, object? eax = null, object? ecx = null, object? edx = null, object? ebx = null, object? esp = null, object? ebp = null, object? esi = null, object? edi = null) => Core.ExternalCall.UserCallx86(Handle, addr, eax, ecx, edx, ebx, esp, ebp, esi, edi);
        public void Callx86(PointerEx addr, params object[] paramaters) => Core.ExternalCall.Callx86(Handle, addr, paramaters);
        public void Callx64(PointerEx addr, params object[] paramaters) => Core.ExternalCall.Callx64(Handle, addr, paramaters);
        public PointerEx this[PointerEx BaseOffset] => BaseAddress + BaseOffset;
        public PointerEx this[PointerEx BaseOffset, params PointerEx[] Offsets] => Misc.OffsetCalculator(Handle, BaseAddress, BaseOffset, Offsets);
        public PointerEx this[string ModuleName, PointerEx ModuleOffset, params PointerEx[] Offsets] => Misc.OffsetCalculator(Handle, this[ModuleName], ModuleOffset, Offsets);
        public PointerEx this[string ModuleName] => BaseProcess.Modules.GetByName(ModuleName).BaseAddress;
        public override string ToString() => @$"{BaseProcess.ProcessName} - {Architecture}";
        void IDisposable.Dispose()
        {
            CloseHandle(Handle);
            if (Misc.IsAdministrator)
                Process.LeaveDebugMode();
            BaseProcess.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}