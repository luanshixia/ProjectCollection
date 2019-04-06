namespace CosmosDBQueryCharge
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the value for the key or the default value if key is not found in the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="key">The key.</param>
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key)
        {
            return source != null && source.TryGetValue(key, out TValue value) ? value : default;
        }

        public static int GetDocumentSize(this object documentObject)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(documentObject)).Length;
        }

        public static T PickRandomItem<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(RandomNumber.Next(source.Count()));
        }

        public static T[] PickRandomItems<T>(this IEnumerable<T> source)
        {
            return source.Aggregate(seed: new List<T>(), func: (list, item) =>
            {
                if (RandomNumber.Next(2) == 1)
                {
                    list.Add(item);
                }

                return list;
            }).ToArray();
        }
    }
}
