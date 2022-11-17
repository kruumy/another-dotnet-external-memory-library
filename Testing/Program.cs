using AnotherExternalMemoryLibrary;
using System.Diagnostics;
using System.Text;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ProcessEx mem = new ProcessEx(Process.GetProcessesByName("BloonsTD6").FirstOrDefault());

            //Console.WriteLine(mem.Read<byte>(mem.BaseAddress + 0x04563, 4).GetHexString());
            mem.Write(0x0, new byte[] { 0x90, 0x90, 0x90 });

        }

    }
}