using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Collections
{
    /// <summary>
    /// Double dictionary.
    /// </summary>
    /// <typeparam name="TKey1">The type of key 1.</typeparam>
    /// <typeparam name="TKey2">The type of key 2.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    public class DoubleDictionary<TKey1, TKey2, TValue> : IDictionary<TKey1, SafeDictionary<TKey2, TValue>>
    {
        protected Dictionary<TKey1, SafeDictionary<TKey2, TValue>> internalDictionary = new Dictionary<TKey1, SafeDictionary<TKey2, TValue>>();

        public SafeDictionary<TKey2, TValue> this[TKey1 key]
        {
            get
            {
                if (!this.internalDictionary.ContainsKey(key))
                {
                    this.internalDictionary[key] = new SafeDictionary<TKey2, TValue>();
                }

                return this.internalDictionary[key];
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public ICollection<TKey1> Keys => this.internalDictionary.Keys;

        public ICollection<SafeDictionary<TKey2, TValue>> Values => this.internalDictionary.Values;

        public ICollection<TValue> RealValues => this.internalDictionary.Values.SelectMany(value => value.Values).ToList();

        public int Count => this.internalDictionary.Count;

        public bool IsReadOnly => false;

        public void Add(TKey1 key, SafeDictionary<TKey2, TValue> value)
        {
            this.internalDictionary.Add(key, value);
        }

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            this[key1][key2] = value;
        }

        void ICollection<KeyValuePair<TKey1, SafeDictionary<TKey2, TValue>>>.Add(KeyValuePair<TKey1, SafeDictionary<TKey2, TValue>> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            this.internalDictionary.Clear();
        }

        bool ICollection<KeyValuePair<TKey1, SafeDictionary<TKey2, TValue>>>.Contains(KeyValuePair<TKey1, SafeDictionary<TKey2, TValue>> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey1 key)
        {
            return this.internalDictionary.ContainsKey(key);
        }

        public bool ContainsKeys(TKey1 key1, TKey2 key2)
        {
            return this.internalDictionary.ContainsKey(key1) && this.internalDictionary[key1].ContainsKey(key2);
        }

        void ICollection<KeyValuePair<TKey1, SafeDictionary<TKey2, TValue>>>.CopyTo(KeyValuePair<TKey1, SafeDictionary<TKey2, TValue>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey1, SafeDictionary<TKey2, TValue>>> GetEnumerator()
        {
            return this.internalDictionary.GetEnumerator();
        }

        public bool Remove(TKey1 key)
        {
            return this.internalDictionary.Remove(key);
        }

        public bool Remove(TKey1 key1, TKey2 key2)
        {
            if (this.internalDictionary.ContainsKey(key1))
            {
                return this.internalDictionary[key1].Remove(key2);
            }

            return false;
        }

        bool ICollection<KeyValuePair<TKey1, SafeDictionary<TKey2, TValue>>>.Remove(KeyValuePair<TKey1, SafeDictionary<TKey2, TValue>> item)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<TKey1, SafeDictionary<TKey2, TValue>>.TryGetValue(TKey1 key, out SafeDictionary<TKey2, TValue> value)
        {
            return this.internalDictionary.TryGetValue(key, out value);
        }

        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
        {
            if (this.ContainsKeys(key1, key2))
            {
                value = this[key1][key2];
                return true;
            }

            value = default(TValue);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
