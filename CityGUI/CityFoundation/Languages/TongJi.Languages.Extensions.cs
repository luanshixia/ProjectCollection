using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace System
{
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
    }

    public class TjEqualityComparer<T> : IEqualityComparer<T>
    {
        protected Func<T, T, bool> _func;

        public TjEqualityComparer(Func<T, T, bool> func)
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
    public static class LocalExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int i = 0;
            foreach (T element in source)
            {
                action(element, i);
                i++;
            }
        }

        public static double Min0<T>(this IEnumerable<T> source, Func<T, double> mapper)
        {
            if (source.Count() > 0)
            {
                return source.Min(mapper);
            }
            else
            {
                return 0;
            }
        }

        public static double Max0<T>(this IEnumerable<T> source, Func<T, double> mapper)
        {
            if (source.Count() > 0)
            {
                return source.Max(mapper);
            }
            else
            {
                return 0;
            }
        }

        public static void PairwiseForEach<T>(this IEnumerable<T> source, Action<T, T> action)
        {
            Enumerable.Range(0, source.Count() - 1).ForEach(i => action(source.ElementAt(i), source.ElementAt(i + 1)));
        }

        public static IEnumerable<U> PairwiseSelect<T, U>(this IEnumerable<T> source, Func<T, T, U> mapper)
        {
            return Enumerable.Range(0, source.Count() - 1).Select(i => mapper(source.ElementAt(i), source.ElementAt(i + 1)));
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int start, int end, int step = 1)
        {
            List<int> indices = new List<int>();
            for (int i = start; i <= end; i += step)
            {
                indices.Add(i);
            }
            return source.ElementsAt(indices);
        }

        public static IEnumerable<T> ElementsAt<T>(this IEnumerable<T> source, IEnumerable<int> indices)
        {
            return indices.Select(i => source.ElementAt(i));
        }

        public static IEnumerable<double> ListTo(this double start, double end, double step)
        {
            for (double x = start; x <= end; x += step)
            {
                yield return x;
            }
        }

        public static IEnumerable<Tuple<T, U>> Cross<T, U>(this IEnumerable<T> ts, IEnumerable<U> us)
        {
            foreach (var t in ts)
            {
                foreach (var u in us)
                {
                    yield return Tuple.Create(t, u);
                }
            }
        }

        public static bool IsSame<T, U>(this IEnumerable<T> source, Func<T, U> selector)
        {
            return source.Select(selector).Distinct().Count() == 1;
        }

        /// <summary>
        /// 一个增强版的Zip()
        /// </summary>
        /// <typeparam name="T1">第一个集合的元素类型</typeparam>
        /// <typeparam name="T2">第二个集合的元素类型</typeparam>
        /// <typeparam name="TResult">结果集合的元素类型</typeparam>
        /// <param name="first">第一个集合</param>
        /// <param name="second">第二个集合</param>
        /// <param name="resultSelector">结果生成器函数</param>
        /// <param name="method">匹配方法，短匹配、长匹配、交错匹配三者之一</param>
        /// <returns>结果集合</returns>
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
            var list = new List<T>();
            list.Add(head);
            list.AddRange(tail);
            return list;
        }
    }

    /// <summary>
    /// 匹配方法，短匹配、长匹配、交错匹配三者之一
    /// </summary>
    public enum MatchMethod
    {
        /// <summary>
        /// 短匹配
        /// </summary>
        Shortest,
        /// <summary>
        /// 长匹配
        /// </summary>
        Longest,
        /// <summary>
        /// 交错匹配
        /// </summary>
        Cross
    }
}

namespace System.Xml.Linq
{
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
