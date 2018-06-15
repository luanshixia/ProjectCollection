using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Collections
{
    public class FlexLookup<TKey, TValue> : ILookup<TKey, TValue>
    {
        protected SafeDictionary<TKey, List<TValue>> internalDictionary = new SafeDictionary<TKey, List<TValue>>(
            createOnMiss: true,
            valueGenerator: key => new List<TValue>());

        public IEnumerable<TValue> this[TKey key]
        {
            get
            {
                return this.internalDictionary[key];
            }
        }

        public void Add(TKey key, TValue value)
        {
            this.internalDictionary[key].Add(value);
        }

        public void AddRange(TKey key, IEnumerable<TValue> values)
        {
            this.internalDictionary[key].AddRange(values);
        }

        public int Count => this.internalDictionary.Count;

        public bool Contains(TKey key)
        {
            return this.internalDictionary.ContainsKey(key);
        }

        public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
        {
            return this.internalDictionary
                .Select(kvp => new FlexGrouping<TKey, TValue>(kvp.Key, kvp.Value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class FlexGrouping<TKey, TValue> : IGrouping<TKey, TValue>
    {
        public TKey Key { get; protected set; }
        protected readonly List<TValue> elements;

        public FlexGrouping(TKey key, IEnumerable<TValue> elements)
        {
            this.Key = key;
            this.elements = elements.ToList();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
