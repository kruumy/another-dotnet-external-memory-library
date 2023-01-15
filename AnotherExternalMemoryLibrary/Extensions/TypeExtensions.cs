using System;
using System.Linq;
using System.Reflection;

namespace AnotherExternalMemoryLibrary.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsUnmanaged(this Type type)
        {
            if (type.IsPrimitive || type.IsPointer || type.IsEnum)
            {
                return true;
            }
            if (!type.IsValueType)
            {
                return false;
            }
            return type
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .All(f => IsUnmanaged(f.FieldType));
        }
    }
}
