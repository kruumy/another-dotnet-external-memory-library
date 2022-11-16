using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

namespace AnotherExternalMemoryLibrary
{
    public class Memory
    {
        public Process BaseProcess { get; private set; }
        public PointerEx BaseAddress { get; private set; }
        public bool Is64Bit { get; private set; }
        public Memory(Process targetProcess)
        {
            BaseProcess = targetProcess;
            BaseAddress = BaseProcess.MainModule?.BaseAddress ?? IntPtr.Zero;
            Imports.IsWow64Process(BaseProcess.Handle, out bool _Is32Bit);
            Is64Bit = !_Is32Bit;
            if (Is64Bit != PointerEx.Is64Bit)
                Console.Error.WriteLine("Warning: Target Process Architecture != Process Architecture");

        }
        public byte[] Read(PointerEx addr, PointerEx NumOfBytes)
        {
            byte[] data = new byte[NumOfBytes];
            PointerEx bytesRead = IntPtr.Zero;
            Imports.ReadProcessMemory(BaseProcess.Handle, addr, data, NumOfBytes, ref bytesRead);
            return data;
        }
        public T Read<T>(PointerEx addr)
        {
            PointerEx size = Marshal.SizeOf(typeof(T));
            byte[] data = Read(addr, size);

            if (typeof(T) == typeof(PointerEx) || typeof(T) == typeof(IntPtr))
                if (PointerEx.Is64Bit)
                    return (dynamic)(PointerEx)BitConverter.ToInt64(data);
                else
                    return (dynamic)(PointerEx)BitConverter.ToInt32(data);
            if (typeof(T) == typeof(float)) return (dynamic)BitConverter.ToSingle(data);
            if (typeof(T) == typeof(long)) return (dynamic)BitConverter.ToInt64(data);
            if (typeof(T) == typeof(ulong)) return (dynamic)BitConverter.ToUInt64(data);
            if (typeof(T) == typeof(int)) return (dynamic)BitConverter.ToInt32(data);
            if (typeof(T) == typeof(uint)) return (dynamic)BitConverter.ToUInt32(data);
            if (typeof(T) == typeof(short)) return (dynamic)BitConverter.ToInt16(data);
            if (typeof(T) == typeof(ushort)) return (dynamic)BitConverter.ToUInt16(data);
            if (typeof(T) == typeof(byte)) return (dynamic)data[0];
            if (typeof(T) == typeof(sbyte)) return (dynamic)data[0];
            throw new Exception($"Invalid Type {typeof(T)}");
        }
        public void Write(PointerEx addr, byte[] bytes)
        {
            PointerEx bytesWritten = IntPtr.Zero;
            Imports.VirtualProtectEx(BaseProcess.Handle, addr, bytes.Length, Imports.MemoryProtection.ExecuteReadWrite, out int oldProtection);
            Imports.WriteProcessMemory(BaseProcess.Handle, addr, bytes, bytes.Length, ref bytesWritten);
            Imports.VirtualProtectEx(BaseProcess.Handle, addr, bytes.Length, (Imports.MemoryProtection)oldProtection, out int _);
        }
        public void Write<T>(PointerEx addr, T value)
        {
            byte[] data = Array.Empty<byte>();
            if (value is IntPtr ip) data = !Is64Bit ? BitConverter.GetBytes(ip.ToInt32()) : BitConverter.GetBytes(ip.ToInt64());
            else if (value is PointerEx ipx) data = !Is64Bit ? BitConverter.GetBytes(ipx.IntPtr.ToInt32()) : BitConverter.GetBytes(ipx.IntPtr.ToInt64());
            else if (value is float f) data = BitConverter.GetBytes(f);
            else if (value is long l) data = BitConverter.GetBytes(l);
            else if (value is ulong ul) data = BitConverter.GetBytes(ul);
            else if (value is int i) data = BitConverter.GetBytes(i);
            else if (value is uint ui) data = BitConverter.GetBytes(ui);
            else if (value is short s) data = BitConverter.GetBytes(s);
            else if (value is ushort u) data = BitConverter.GetBytes(u);
            else if (value is byte b) data = new byte[] { b };
            else if (value is sbyte sb) data = new byte[] { (byte)sb };
            else if (value is byte[] ba) data = ba;
            else throw new Exception($"Invalid Type {typeof(T)}");
            Write(addr, data);
        }

    }
}