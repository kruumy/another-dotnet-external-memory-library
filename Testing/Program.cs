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
            Console.WriteLine(mem.BaseAddress.ToString());
            Console.WriteLine(mem.Modules.GetByName("UnityPlayer.dll").BaseAddress.ToPointerEx().ToString());
            Console.WriteLine(mem["UnityPlayer.dll"].BaseAddress.ToPointerEx().ToString());
            Console.WriteLine(mem[0xF].ToString());
            Console.WriteLine(mem[0xF].ToString());
        }

    }
}