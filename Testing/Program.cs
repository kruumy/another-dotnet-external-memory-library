using AnotherExternalMemoryLibrary;
using System.Diagnostics;
using System.Text;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Memory mem = new Memory(Process.GetProcessesByName("iw4m").FirstOrDefault());


            mem.Write<float>(mem.BaseAddress + 0x5FFCBA0, 90);
        }

    }
}