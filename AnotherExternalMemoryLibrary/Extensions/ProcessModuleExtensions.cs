using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class ProcessModuleExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ProcessModule GetByName(this ProcessModuleCollection modules, string name)
        {
            foreach (ProcessModule item in modules)
            {
                if (item.ModuleName == name)
                {
                    return item;
                }
            }
            throw new ArgumentException(name, nameof(name));
        }
    }
}
