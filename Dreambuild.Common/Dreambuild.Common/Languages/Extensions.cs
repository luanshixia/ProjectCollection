using Dreambuild.Functional;
using System.Collections.Generic;
using System.Reflection;

namespace System
{
    /// <summary>
    /// Useful extensions.
    /// </summary>
    public static class LocalExtensions
    {
        public static double TryParseToDouble(this string source, double defaultValue = 0)
        {
            double result = defaultValue;
            double.TryParse(source, out result);
            return result;
        }

        public static int TryParseToInt32(this string source, int defaultValue = 0)
        {
            int result = defaultValue;
            int.TryParse(source, out result);
            return result;
        }

        // null proof
        public static string TryToString(this object source) // newly 20130521
        {
            return Convert.ToString(source);
        }

        public static T ParseToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static bool CanParseToNumber(this string source) // newly 20130308
        {
            double result = 0;
            return double.TryParse(source, out result);
        }

        public static List<T> WrapInList<T>(this T obj) // newly 20140620
        {
            return new List<T> { obj };
        }

        public static T[] WrapInArray<T>(this T obj) // newly 20140620
        {
            return new[] { obj };
        }

        public static object GetPropertyValue(this object obj, string path) // newly 20141218 - mod 20170303
        {
            var paths = path.Split('.');
            var host = obj;
            foreach (var prop in paths)
            {
                host = host.GetType().GetRuntimeProperty(prop).GetValue(host);
            }
            return host;
        }
    }

    /// <summary>
    /// A handy EqualityComparer base, so no need to subclass.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EzEqualityComparer<T> : IEqualityComparer<T>
    {
        protected Func<T, T, bool> _func;

        public EzEqualityComparer(Func<T, T, bool> func)
        {
            _func = func;
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
}

namespace System.Linq
{
    /// <summary>
    /// Useful extensions.
    /// </summary>
    public static class LocalExtensions
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
            var indices = new List<int>();
            for (int i = start; i <= end; i += step)
            {
                indices.Add(i);
            }
            return source.ElementsAt(indices);
        }

        public static IEnumerable<T> ElementsAt<T>(this IEnumerable<T> source, IEnumerable<int> indices)
        {
            return Seq.fetch(indices, source);
        }

        public static IEnumerable<double> ListTo(this double start, double end, double step)
        {
            return Seq.range(start, end, step);
        }

        public static IEnumerable<int> ListTo(this int start, int end, int step)
        {
            return Seq.range(start, end, step);
        }

        public static IEnumerable<T> Every<T>(this IEnumerable<T> source, int step, int start = 0)
        {
            return ListTo(start, source.Count() - 1, step).Select(i => source.ElementAt(i));
        }

        public static IEnumerable<T> Initialize<T>(this int n, Func<int, T> initializer)
        {
            return Seq.init(n, initializer);
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

        public static bool IsSame<T, U>(this IEnumerable<T> source, Func<T, U> selector)
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

        public static List<T> Cons<T>(this T head, List<T> tail)
        {
            var list = new List<T> { head };
            list.AddRange(tail);
            return list;
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

namespace System.Xml.Linq
{
    /// <summary>
    /// Useful extensions.
    /// </summary>
    public static class LocalExtensions
    {
        public static string AttValue(this XElement xe, XName attName)
        {
            return xe.Attribute(attName) == null ? string.Empty : xe.Attribute(attName).Value;
        }

        public static void SetAttValue(this XElement xe, XName attName, string attValue)
        {
            if (xe.Attribute(attName) == null)
            {
                xe.Add(new XAttribute(attName, attValue));
            }
            else
            {
                xe.Attribute(attName).Value = attValue;
            }
        }

        public static XElement ElementX(this XElement xe, XName name)
        {
            return xe.Element(name) ?? new XElement(name);
        }

        public static string EleValue(this XElement xe, XName eleName)
        {
            return xe.Element(eleName) == null ? string.Empty : xe.Element(eleName).Value;
        }
    }
}
