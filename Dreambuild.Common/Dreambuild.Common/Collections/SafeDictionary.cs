using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Collections
{
    public class SafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        protected Dictionary<TKey, TValue> internalDictionary = new Dictionary<TKey, TValue>();
        protected bool createOnMiss = false;
        protected Func<TKey, TValue> valueGenerator = key => default(TValue);

        public SafeDictionary(bool createOnMiss = false, Func<TKey, TValue> valueGenerator = null)
        {
            this.createOnMiss = createOnMiss;
            if (valueGenerator != null)
            {
                this.valueGenerator = valueGenerator;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.internalDictionary.ContainsKey(key)
                    ? this.internalDictionary[key]
                    : this.createOnMiss
                        ? this.internalDictionary[key] = this.valueGenerator(key)
                        : default(TValue);
            }
            set
            {
                this.internalDictionary[key] = value;
            }
        }

        public ICollection<TKey> Keys => this.internalDictionary.Keys;

        public ICollection<TValue> Values => this.internalDictionary.Values;

        public int Count => this.internalDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            this.internalDictionary.Add(key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            this.internalDictionary.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            return this.internalDictionary.ContainsKey(key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.internalDictionary.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            return this.internalDictionary.Remove(key);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.internalDictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
