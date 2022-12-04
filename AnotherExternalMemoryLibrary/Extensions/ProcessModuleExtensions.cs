using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class ProcessModuleExtensions
    {
        public static ProcessModule GetByName(this ProcessModuleCollection modules, string name)
        {
            foreach (ProcessModule item in modules)
                if (item.ModuleName == name)
                    return item;
            throw new Exception("Name Did Not Match Any ModuleNames");
        }
        public static IEnumerable<ProcessModule> GetEnumerable(this ProcessModuleCollection modules)
        {
            return modules.Cast<ProcessModule>();
        }
    }
}
