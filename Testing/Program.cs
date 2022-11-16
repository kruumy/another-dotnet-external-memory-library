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
            Console.WriteLine(mem.BaseAddress.ToString());
            foreach (var i in mem.Read<byte>(mem.BaseAddress, 10))
            {
                Console.WriteLine(i.ToString());
            }
        }

    }
}