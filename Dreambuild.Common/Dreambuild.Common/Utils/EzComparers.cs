using System;
using System.Collections.Generic;

namespace Dreambuild.Utils
{
    /// <summary>
    /// A handy <see cref="EqualityComparer{T}"/> base, so no need to subclass.
    /// </summary>
    /// <typeparam name="T">The argument type.</typeparam>
    public class EzEqualityComparer<T> : IEqualityComparer<T>
    {
        protected Func<T, T, bool> _func;

        public EzEqualityComparer(Func<T, T, bool> comparison)
        {
            _func = comparison;
        }

        public bool Equals(T x, T y)
        {
            return _func(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// A handy <see cref="Comparer{T}"/> base, so no need to subclass.
    /// </summary>
    /// <typeparam name="T">The argument type.</typeparam>
    public class EzComparer<T> : IComparer<T>
    {
        protected Func<T, T, int> _func;

        public EzComparer(Func<T, T, int> comparison)
        {
            _func = comparison;
        }

        public int Compare(T x, T y)
        {
            return _func(x, y);
        }
    }
}
