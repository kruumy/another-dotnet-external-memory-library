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


            //mem.Read<float>(mem.BaseAddress + 0x5FFCBA0);
            //Console.WriteLine(mem.Read<string>(mem.BaseAddress + 0x5EE27A0));

            int ttt = 3456;
            Console.WriteLine(BitConverter.ToString(BitConverter.GetBytes(ttt)));
            PointerEx[] result = mem.Scan<int>(3456, mem.BaseProcess.MainModule);
            foreach (var item in result)
            {
                Console.WriteLine(item.ToString());
            }
        }

    }
}