using AnotherExternalMemoryLibrary;
using System.Diagnostics;

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