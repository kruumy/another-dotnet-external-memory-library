using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherExternalMemoryLibrary
{
    public static class Utils
    {
        public static PointerEx OffsetCalculator(ProcessEx mem, PointerEx baseAddr, PointerEx baseOffset, PointerEx[] offsets)
        {
            PointerEx result = baseAddr + baseOffset;
            foreach (PointerEx offset in offsets)
                result = offset + mem.Read<PointerEx>(result);

            return result;
        }
        public static byte[] NOP(PointerEx NumOfBytes)
        {
            return Enumerable.Repeat((byte)0x90, NumOfBytes).ToArray();
        }
    }
}
