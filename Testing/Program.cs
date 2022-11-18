using AnotherExternalMemoryLibrary;
using AnotherExternalMemoryLibrary.Extensions;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ProcessEx mem = new ProcessEx(Process.GetProcessesByName("BloonsTD6").FirstOrDefault());
            PointerEx pointerEx = 0xFFFFFF;
            pointerEx.print();

        }

    }
}