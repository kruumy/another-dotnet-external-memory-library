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

            byte[] bytes = { 0x90, 0x90, 0x90 };
            mem.Write<byte>(mem.BaseAddress, bytes);
            Console.WriteLine(mem.Read<byte>(mem.BaseAddress, 4).GetHexString());
            Console.WriteLine(mem.BaseAddress.ToString());
            Console.ReadLine();

        }

    }
}