using System;
using System.Collections.Generic;
using System.Text;

namespace BubbleMind.Extensions
{
    public static class ObjectExtensions
    {
        public static T Coalesce<T>(this T @object)
            where T : new()
        {
            return @object ?? new T();
        }

        public static T With<T>(this T @object, Action<T> update)
            where T : struct
        {
            T copy = @object;
            update(copy);
            return copy;
        }
    }
}
