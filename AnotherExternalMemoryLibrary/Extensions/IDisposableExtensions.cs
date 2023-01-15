using System;
using System.Collections.Generic;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class IDisposableExtensions
    {
        public static void Dispose(this IEnumerable<IDisposable> disposables)
        {
            foreach (IDisposable item in disposables)
            {
                item.Dispose();
            }
        }
    }
}
