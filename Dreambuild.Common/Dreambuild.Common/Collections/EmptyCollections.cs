using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Collections
{
    /// <summary>
    /// Empty list.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public static class EmptyList<T>
    {
        /// <summary>
        /// The empty list instance.
        /// </summary>
        public static readonly List<T> Instance = new List<T>();
    }
}
