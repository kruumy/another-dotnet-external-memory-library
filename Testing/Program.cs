using AnotherExternalMemoryLibrary;
using System.Diagnostics;
using System.Text;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ProcessEx mem = new ProcessEx(Process.GetProcessesByName("iw4m").FirstOrDefault());


            //mem.Write<float>(mem.BaseAddress + 0x5FFCBA0, 90);
            Console.WriteLine(mem.Read<string>(mem.BaseAddress + 0x5EE27A0));
            mem.Write<string>(mem.BaseAddress + 0x5EE27A0, "mp_highrise");
            Console.WriteLine(mem.Read<string>(mem.BaseAddress + 0x5EE27A0));
        }

    }
}