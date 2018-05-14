using System;
using System.Collections.Generic;

namespace Dreambuild.Collections
{
    /// <summary>
    /// Case-insensitive hash set.
    /// </summary>
    public class CIHashSet : HashSet<string>
    {
        /// <summary>
        /// An empty instance of the <see cref="CIHashSet"/> class.
        /// </summary>
        public static readonly CIHashSet Empty = new CIHashSet();

        /// <summary>
        /// Initializes a new instance of the <see cref="CIHashSet"/> class.
        /// </summary>
        public CIHashSet()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CIHashSet"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        public CIHashSet(IEnumerable<string> source)
            : base(source, StringComparer.InvariantCultureIgnoreCase)
        {
        }
    }

    /// <summary>
    /// Case-insensitive dictionary.
    /// </summary>
    /// <typeparam name="TValue">The type of value.</typeparam>
    public class CIDictionary<TValue> : Dictionary<string, TValue>
    {
        /// <summary>
        /// An empty instance of the <see cref="CIDictionary{TValue}"/> class.
        /// </summary>
        public static readonly CIDictionary<TValue> Empty = new CIDictionary<TValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CIDictionary{TValue}"/> class.
        /// </summary>
        public CIDictionary()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CIDictionary{TValue}"/> class.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the dictionary can contain.</param>
        public CIDictionary(int capacity)
            : base(capacity, StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CIDictionary{TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public CIDictionary(IDictionary<string, TValue> dictionary)
            : base(dictionary, StringComparer.InvariantCultureIgnoreCase)
        {
        }
    }
}
