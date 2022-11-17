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
            ProcessEx mem = new ProcessEx(Process.GetProcessesByName("BloonsTD6").FirstOrDefault());


        }

    }
}