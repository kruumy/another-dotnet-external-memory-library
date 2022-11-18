using AnotherExternalMemoryLibrary;
using AnotherExternalMemoryLibrary.Extensions;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ProcessEx mem = ProcessEx.GetProcessByName("BloonsTD6");
            PointerEx pointerEx = 0xFFFFFF;
            pointerEx.print();
            byte[] bytes = { 0x90, 0x90, 0x90, 0x90, 0x45, 0x65, 0x25, 0x90, 0x90 };
            bytes.Subtract(0x90, 0x90, 0x90, 0x90).print();
        }

    }
}