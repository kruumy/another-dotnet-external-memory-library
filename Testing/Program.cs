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
            Utils.GoDebugPriv();
            Console.WriteLine(mem.Scan(new byte[] { 0x90 })[0].ToString());
        }

    }
}