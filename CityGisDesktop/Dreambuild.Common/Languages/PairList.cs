using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Collections
{
    public class PairList<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            base.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<TValue> this[TKey key]
        {
            get
            {
                return this.Where(x => x.Key.Equals(key)).Select(x => x.Value).ToList().AsReadOnly();
            }
        }
    }

    public static class PairList
    {
        public static PairList<TKey, TValue> CreatePairList<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            var result = new PairList<TKey, TValue>();
            foreach (var item in source)
            {
                result.Add(keySelector(item), valueSelector(item));
            }
            return result;
        }

        public static PairList<TKey, TValue> CreatePairList<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IEnumerable<TValue>> valuesSelector)
        {
            var result = new PairList<TKey, TValue>();
            foreach (var item in source)
            {
                var key = keySelector(item);
                var values = valuesSelector(item);
                foreach (var value in values)
                {
                    result.Add(key, value);
                }
            }
            return result;
        }
    }
}
