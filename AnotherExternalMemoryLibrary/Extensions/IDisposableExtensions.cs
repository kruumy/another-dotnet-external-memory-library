using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class IDisposableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dispose(this IEnumerable<IDisposable> disposables)
        {
            foreach (IDisposable item in disposables)
            {
                item.Dispose();
            }
        }
    }
}
