using AnotherExternalMemoryLibrary.Core.Extensions;
using System.Collections.Generic;

namespace AnotherExternalMemoryLibrary.Core
{
    public class Pointer
    {
        public PointerEx pHandle { get; set; }
        public List<PointerEx> Offsets = new List<PointerEx>();
        public PointerEx BaseOffset { get; set; }
        public PointerEx BaseAddress { get; set; }
        public PointerEx AbsoluteAddress
        {
            get
            {
                PointerEx result = BaseAddress + BaseOffset;
                foreach (PointerEx offset in Offsets)
                    result = offset + ReadProcessMemory.Read<byte>(pHandle, result, PointerEx.Size).ToStruct<PointerEx>();

                return result;
            }
        }
        public Pointer(PointerEx _pHandle, PointerEx _BaseAddress)
        {
            pHandle = _pHandle;
            BaseAddress = _BaseAddress;
        }
        public Pointer(PointerEx _pHandle, PointerEx _BaseAddress, PointerEx _BaseOffset)
        {
            pHandle = _pHandle;
            BaseAddress = _BaseAddress;
            BaseOffset = _BaseOffset;
        }
        public Pointer(PointerEx _pHandle, PointerEx _BaseAddress, PointerEx _BaseOffset, params PointerEx[] _Offsets)
        {
            pHandle = _pHandle;
            BaseAddress = _BaseAddress;
            BaseOffset = _BaseOffset;
            Offsets.AddRange(_Offsets);
        }
    }
}
