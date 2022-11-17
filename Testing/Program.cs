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


            Console.WriteLine(Utils.IsAdministrator());
            Console.WriteLine(mem.BaseAddress.ToString());
            Console.ReadLine();

        }

    }
}