using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AnotherExternalMemoryLibrary.Core.Extensions
{
    public static class MiscExtenstions
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
        public static IEnumerable<T> GetRange<T>(this IEnumerable<T> source, int start, int end)
        {
            return source.Skip(start).Take(end);
        }
    }
}
