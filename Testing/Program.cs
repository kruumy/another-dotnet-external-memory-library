using AnotherExternalMemoryLibrary;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ProcessEx mem = new ProcessEx(Process.GetProcessesByName("BloonsTD6").FirstOrDefault());

            byte[] bytes = new byte[] { 0x90, 0x90, 0x90, 0x90 };
            byte[] bytes2 = new byte[] { 0x45, 0x45, 0x45, 0x45 };
            byte[] bytes3 = new byte[] { 0x32, 0x32, 0x32, 0x32 };
            bytes = bytes.Add(bytes2, bytes3);
            Console.WriteLine(bytes.GetHexString());
        }

    }
}