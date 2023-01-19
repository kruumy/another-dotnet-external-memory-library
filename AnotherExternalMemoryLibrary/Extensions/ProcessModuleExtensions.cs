using System;
using System.Diagnostics;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class ProcessModuleExtensions
    {
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
