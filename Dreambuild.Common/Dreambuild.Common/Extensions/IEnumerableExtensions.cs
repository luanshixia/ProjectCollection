using Dreambuild.Collections;
using Dreambuild.Functional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Seq.iter(action, source);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            Seq.iteri((i, x) => action(x, i), source);
        }

        public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> source)
        {
            return Seq.pairwise(source);
        }

        public static void PairwiseForEach<T>(this IEnumerable<T> source, Action<T, T> action)
        {
            Enumerable.Range(0, source.Count() - 1).ForEach(i => action(source.ElementAt(i), source.ElementAt(i + 1)));
        }

        public static IEnumerable<U> PairwiseSelect<T, U>(this IEnumerable<T> source, Func<T, T, U> mapper)
        {
            return Enumerable.Range(0, source.Count() - 1).Select(i => mapper(source.ElementAt(i), source.ElementAt(i + 1)));
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int start, int count)
        {
            return source.Skip(start).Take(count);
        }

        public static IEnumerable<T> StepSlice<T>(this IEnumerable<T> source, int start, int end, int step) // mod 20140422
        {
            return source.ElementsAt(start.SeqTo(end, step));
        }

        public static IEnumerable<T> ElementsAt<T>(this IEnumerable<T> source, IEnumerable<int> indices)
        {
            return Seq.fetch(indices, source);
        }

        public static IEnumerable<int> SeqTo(this int start, int end, int step = 1)
        {
            return Seq.range(start, end, step);
        }

        public static IEnumerable<T> Every<T>(this IEnumerable<T> source, int step = 1, int start = 0)
        {
            return Seq.fetch(SeqTo(start, source.Count() - 1, step), source);
        }

        public static IEnumerable<T> Generate<T>(this int n, Func<int, T> initializer)
        {
            return Seq.init(n, initializer);
        }

        public static IEnumerable<T> Recurrent<T>(this int n, Func<T, T> recurrence, T element0)
        {
            var temp = element0;
            for (var i = 0; i < n; i++)
            {
                yield return temp;
                temp = recurrence(temp);
            }
        }

        public static IEnumerable<T> Recurrent<T>(this int n, Func<T, T, T> recurrence, T element0, T element1)
        {
            var temp1 = element0;
            var temp2 = element1;
            for (var i = 0; i < n; i++)
            {
                yield return temp1;
                var temp11 = temp1;
                temp1 = temp2;
                temp2 = recurrence(temp11, temp2);
            }
        }

        public static U Fold<T, U>(this IEnumerable<T> source, Func<U, T, U> folder, U state)
        {
            return Seq.fold(folder, state, source);
        }

        public static IEnumerable<U> Scan<T, U>(this IEnumerable<T> source, Func<U, T, U> folder, U state)
        {
            return Seq.scan(folder, state, source);
        }

        public static T Reduce<T>(this IEnumerable<T> source, Func<T, T, T> reduction)
        {
            return Seq.reduce(reduction, source);
        }

        public static IEnumerable<Tuple<T, U>> Cross<T, U>(this IEnumerable<T> ts, IEnumerable<U> us)
        {
            return Seq.cross(ts, us, Tuple.Create);
        }

        public static IEnumerable<V> Cross<T, U, V>(this IEnumerable<T> ts, IEnumerable<U> us, Func<T, U, V> selector)
        {
            return Seq.cross(ts, us, selector);
        }

        public static void Cross<T, U>(this IEnumerable<T> ts, IEnumerable<U> us, Action<T, U> action)
        {
            foreach (var t in ts)
            {
                foreach (var u in us)
                {
                    action(t, u);
                }
            }
        }

        public static bool AreElementsEqual<T, U>(this IEnumerable<T> source, Func<T, U> selector)
        {
            return source.Select(selector).Distinct().Count() == 1;
        }

        public static IEnumerable<T> DistinctBy<T, U>(this IEnumerable<T> source, Func<T, U> selector) where U : IEquatable<U>
        {
            return Seq.distinctBy(selector, source);
        }

        public static int FindIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return Seq.findIndex(predicate, source);
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> sources)
        {
            return Seq.flatten(sources);
        }

        /// <summary>
        /// An enhanced Zip().
        /// </summary>
        /// <typeparam name="T1">Element type of first.</typeparam>
        /// <typeparam name="T2">Element type of second.</typeparam>
        /// <typeparam name="TResult">Element type of result.</typeparam>
        /// <param name="first">The first collection.</param>
        /// <param name="second">The second collection.</param>
        /// <param name="resultSelector">The result selector function.</param>
        /// <param name="method">The match method.</param>
        /// <returns>The result collection.</returns>
        public static IEnumerable<TResult> Match<T1, T2, TResult>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, TResult> resultSelector, MatchMethod method = MatchMethod.Shortest) // newly 20130424
        {
            if (method == MatchMethod.Shortest)
            {
                int n = Math.Min(first.Count(), second.Count());
                for (int i = 0; i < n; i++)
                {
                    yield return resultSelector(first.ElementAt(i), second.ElementAt(i));
                }
            }
            else if (method == MatchMethod.Longest)
            {
                int n = Math.Max(first.Count(), second.Count());
                for (int i = 0; i < n; i++)
                {
                    int i1 = i < first.Count() ? i : first.Count() - 1;
                    int i2 = i < second.Count() ? i : second.Count() - 1;
                    yield return resultSelector(first.ElementAt(i1), second.ElementAt(i2));
                }
            }
            else // method == MatchMethod.Cross
            {
                foreach (var t1 in first)
                {
                    foreach (var t2 in second)
                    {
                        yield return resultSelector(t1, t2);
                    }
                }
            }
        }

        /// <summary>
        /// Prepends element to sequence.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="head">The head.</param>
        /// <param name="tail">The tail.</param>
        /// <returns>The result sequence.</returns>
        public static IEnumerable<T> Cons<T>(this T head, IEnumerable<T> tail)
        {
            yield return head;
            foreach(var element in tail)
            {
                yield return element;
            }
        }

        /// <summary>
        /// Counts numbers of unique elements in a collection. - newly 20170303
        /// </summary>
        /// <typeparam name="TElement">Type of elements.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>A dictionary representing the counting result.</returns>
        public static IDictionary<TElement, int> CountElements<TElement>(this IEnumerable<TElement> collection, IEqualityComparer<TElement> equalityComparer)
        {
            return collection
                .GroupBy(element => element, equalityComparer)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        /// <summary>
        /// Breaks the source collection into chunks. - newly 20180420
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="chunkSize">The chunk size.</param>
        /// <returns>A collection of chunks.</returns>
        public static IEnumerable<T[]> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            for (var offset = 0; offset < source.Count(); offset += chunkSize)
            {
                var size = Math.Min(chunkSize, source.Count() - offset);
                yield return source.Slice(offset, size).ToArray();
            }
        }

        /// <summary>
        /// Replaces null source collection with an empty collection.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A safe collection.</returns>
        public static T[] Safe<T>(this T[] source)
        {
            return source ?? Array.Empty<T>();
        }

        /// <summary>
        /// Replaces null source collection with an empty collection.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A safe collection.</returns>
        public static List<T> Safe<T>(this List<T> source)
        {
            return source ?? EmptyList<T>.Instance;
        }

        /// <summary>
        /// Replaces null source collection with an empty collection.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A safe collection.</returns>
        public static IEnumerable<T> Safe<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Determines if source collection is null or empty.
        /// </summary>
        /// <typeparam name="T">The element type of the source collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>The result.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || source.Count() == 0;
        }

        /// <summary>
        /// Versatile minimum.
        /// </summary>
        /// <typeparam name="TSource">The element type of the source collection.</typeparam>
        /// <typeparam name="TCompare">The type of value to compare on.</typeparam>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The comparing value selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns>The result.</returns>
        public static TResult Min<TSource, TCompare, TResult>(this IEnumerable<TSource> source, Func<TSource, TCompare> selector, Func<TSource, int, TResult> resultSelector, Comparison<TCompare> comparison = null)
        {
            if (source.IsNullOrEmpty())
            {
                throw new ArgumentException("Empty source.");
            }

            if (comparison == null)
            {
                comparison = Comparer<TCompare>.Default.Compare;
            }

            var minElement = default(TSource);
            var minValue = default(TCompare);
            var minIndex = default(Int32);

            source.ForEach((element, index) =>
            {
                if (index == 0)
                {
                    minElement = element;
                    minValue = selector(minElement);
                    minIndex = 0;
                }
                else
                {
                    var value = selector(element);
                    if (comparison(value, minValue) < 0)
                    {
                        minElement = element;
                        minValue = value;
                        minIndex = index;
                    }
                }
            });

            return resultSelector(minElement, minIndex);
        }

        /// <summary>
        /// Versatile maximum.
        /// </summary>
        /// <typeparam name="TSource">The element type of the source collection.</typeparam>
        /// <typeparam name="TCompare">The type of value to compare on.</typeparam>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The comparing value selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns>The result.</returns>
        public static TResult Max<TSource, TCompare, TResult>(this IEnumerable<TSource> source, Func<TSource, TCompare> selector, Func<TSource, int, TResult> resultSelector, Comparison<TCompare> comparison = null)
        {
            if (source.IsNullOrEmpty())
            {
                throw new ArgumentException("Empty source.");
            }

            if (comparison == null)
            {
                comparison = Comparer<TCompare>.Default.Compare;
            }

            var maxElement = default(TSource);
            var maxValue = default(TCompare);
            var maxIndex = default(Int32);

            source.ForEach((element, index) =>
            {
                if (index == 0)
                {
                    maxElement = element;
                    maxValue = selector(maxElement);
                    maxIndex = 0;
                }
                else
                {
                    var value = selector(element);
                    if (comparison(value, maxValue) > 0)
                    {
                        maxElement = element;
                        maxValue = value;
                        maxIndex = index;
                    }
                }
            });

            return resultSelector(maxElement, maxIndex);
        }

        /// <summary>
        /// Shorthand minimum.
        /// </summary>
        /// <typeparam name="TSource">The element type of the source collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="comparer">The comparer.</param>
        public static TSource Min<TSource>(this IEnumerable<TSource> source, Comparison<TSource> comparison = null)
        {
            return source.Min(selector: element => element, resultSelector: (element, index) => element, comparison: comparison);
        }

        /// <summary>
        /// Shorthand maximum.
        /// </summary>
        /// <typeparam name="TSource">The element type of the source collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="comparer">The comparer.</param>
        public static TSource Max<TSource>(this IEnumerable<TSource> source, Comparison<TSource> comparison = null)
        {
            return source.Max(selector: element => element, resultSelector: (element, index) => element, comparison: comparison);
        }

        /// <summary>
        /// Creates an ILookup.
        /// </summary>
        /// <typeparam name="TSource">The element type of the source collection.</typeparam>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="sources">The source collection.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="valuesSelector">The value selector.</param>
        /// <returns>The ILookup.</returns>
        public static ILookup<TKey, TValue> ToLookup<TSource, TKey, TValue>(this IEnumerable<TSource> sources, Func<TSource, TKey> keySelector, Func<TSource, IEnumerable<TValue>> valuesSelector)
        {
            var result = new FlexLookup<TKey, TValue>();
            sources.ForEach(element =>
            {
                result.AddRange(keySelector(element), valuesSelector(element));
            });
            return result;
        }
    }

    /// <summary>
    /// Match method: shortest, longest, or cross.
    /// </summary>
    public enum MatchMethod
    {
        /// <summary>
        /// Shortest match.
        /// </summary>
        Shortest,
        /// <summary>
        /// Longest match.
        /// </summary>
        Longest,
        /// <summary>
        /// Cross match.
        /// </summary>
        Cross
    }
}
