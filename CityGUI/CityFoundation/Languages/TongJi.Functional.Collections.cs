using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongJi.Functional
{
    /// <summary>
    /// 提供类似F#核心库中Seq模块的功能
    /// </summary>
    public static class Seq
    {
        public static IEnumerable<U> map<T, U>(Func<T, U> mapping, IEnumerable<T> source)
        {
            return source.Select(mapping);
        }

        public static IEnumerable<U> mapi<T, U>(Func<int, T, U> mapping, IEnumerable<T> source)
        {
            return Enumerable.Range(0, source.Count()).Select(i => mapping(i, source.ElementAt(i)));
        }

        //public static IEnumerable<U> SelectI<T, U>(this IEnumerable<T> source, Func<int, T, U> mapping)
        //{
        //    return mapi(mapping, source);
        //}

        public static IEnumerable<U> map2<T1, T2, U>(Func<T1, T2, U> mapping, IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            int n = Math.Min(source1.Count(), source2.Count());
            return Enumerable.Range(0, n).Select(i => mapping(source1.ElementAt(i), source2.ElementAt(i)));
        }

        public static void iter<T>(Action<T> action, IEnumerable<T> source)
        {
            source.ToList().ForEach(action);
        }

        public static void iteri<T>(Action<int, T> action, IEnumerable<T> source)
        {
            Enumerable.Range(0, source.Count()).ToList().ForEach(i => action(i, source.ElementAt(i)));
        }

        public static void ForEachI<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            iteri(action, source);
        }

        public static void iter2<T1, T2>(Action<T1, T2> action, IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            int n = Math.Min(source1.Count(), source2.Count());
            Enumerable.Range(0, n).ToList().ForEach(i => action(source1.ElementAt(i), source2.ElementAt(i)));
        }

        public static IEnumerable<T> init<T>(int n, Func<int, T> initializer)
        {
            return Enumerable.Range(0, n).Select(initializer);
        }

        public static IEnumerable<T> Initialize<T>(int n, Func<int, T> initializer)
        {
            return init(n, initializer);
        }

        public static IEnumerable<T> filter<T>(Func<T, bool> predicate, IEnumerable<T> source)
        {
            return source.Where(predicate);
        }

        //public static IEnumerable<T> WhereI<T>(this IEnumerable<T> source, Func<int, T, bool> predicate)
        //{
        //    return Enumerable.Range(0, source.Count()).Where(i => predicate(i, source.ElementAt(i))).Select(i => source.ElementAt(i));
        //}

        public static U fold<T, U>(Func<U, T, U> folder, U state, IEnumerable<T> source)
        {
            foreach (var element in source)
            {
                state = folder(state, element);
            }
            return state;
        }

        public static U Fold<T, U>(this IEnumerable<T> source, Func<U, T, U> folder, U state)
        {
            return fold(folder, state, source);
        }

        public static IEnumerable<U> scan<T, U>(Func<U, T, U> folder, U state, IEnumerable<T> source)
        {
            foreach (var element in source)
            {
                state = folder(state, element);
                yield return state;
            }
        }

        public static IEnumerable<U> Scan<T, U>(this IEnumerable<T> source, Func<U, T, U> folder, U state)
        {
            return scan(folder, state, source);
        }

        public static T reduce<T>(Func<T, T, T> reduction, IEnumerable<T> source)
        {
            T state = source.First();
            Action<Action<T>, IEnumerable<T>> fwrapper = Seq.iter;
            Seq.tail(source).ApplyFunction(fwrapper.Currying(x => state = reduction(state, x)));
            return state;
        }

        public static T Reduce<T>(this IEnumerable<T> source, Func<T, T, T> reduction)
        {
            return reduce(reduction, source);
        }

        public static T head<T>(IEnumerable<T> source)
        {
            return source.First();
        }

        public static IEnumerable<T> tail<T>(IEnumerable<T> source)
        {
            return Enumerable.Range(1, source.Count() - 1).Select(i => source.ElementAt(i));
        }

        public static IEnumerable<Tuple<T, T>> pairwise<T>(IEnumerable<T> source)
        {
            return Enumerable.Range(0, source.Count() - 1).Select(i => Tuple.Create(source.ElementAt(i), source.ElementAt(i + 1)));
        }

        public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> source)
        {
            return pairwise(source);
        }

        public static IEnumerable<T> append<T>(IEnumerable<T> source1, IEnumerable<T> source2)
        {
            return source1.Concat(source2);
        }

        public static IEnumerable<U> collect<T, U>(Func<T, IEnumerable<U>> mapping, IEnumerable<T> source)
        {
            return source.SelectMany(mapping);
        }

        public static int compareWith<T>(Func<T, T, int> comparer, IEnumerable<T> source1, IEnumerable<T> source2)
        {
            var scan = Seq.map2(comparer, source1, source2);
            int c = scan.FirstOrDefault(x => x != 0); // mod 20130528
            if (c != 0)
            {
                return c;
            }
            else
            {
                return source1.Count() - source2.Count();
            }
        }

        public static IEnumerable<T> concat<T>(IEnumerable<IEnumerable<T>> sources)
        {
            return Seq.reduce(Seq.append, sources);
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> sources)
        {
            return sources.SelectMany(x => x);
        }

        //public static IEnumerable<T> delay<T>(Func<IEnumerable<T>> generator)
        //{
        //    return generator();
        //}

        public static IEnumerable<T> distinctBy<T, U>(Func<T, U> projection, IEnumerable<T> source) where U : IEquatable<U>
        {
            return source.Select(x => projection(x)).Distinct().Select(x => source.First(y => projection(y).Equals(x)));
        }

        public static IEnumerable<T> DistinctBy<T, U>(this IEnumerable<T> source, Func<T, U> projection) where U : IEquatable<U>
        {
            return distinctBy(projection, source);
        }

        public static IEnumerable<T> empty<T>()
        {
            return Enumerable.Empty<T>();
        }

        public static bool exists<T>(Func<T, bool> predicate, IEnumerable<T> source)
        {
            return source.Any(predicate);
        }

        public static bool exists2<T1, T2>(Func<T1, T2, bool> predicate, IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            return Seq.map2(predicate, source1, source2).Any(x => x == true);
        }

        public static T find<T>(Func<T, bool> predicate, IEnumerable<T> source)
        {
            return source.First(predicate);
        }

        public static int findIndex<T>(Func<T, bool> predicate, IEnumerable<T> source)
        {
            return source.ToList().IndexOf(source.First(predicate));
        }

        public static int FindIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return findIndex(predicate, source);
        }

        public static bool forall<T>(Func<T, bool> predicate, IEnumerable<T> source)
        {
            return source.All(predicate);
        }

        public static bool forall2<T1, T2>(Func<T1, T2, bool> predicate, IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            return Seq.map2(predicate, source1, source2).All(x => x == true);
        }

        public static IEnumerable<Tuple<U, IEnumerable<T>>> groupBy<T, U>(Func<T, U> projection, IEnumerable<T> source) where U : IEquatable<U>
        {
            return source.GroupBy(projection).Select(x => Tuple.Create(x.Key, x.Select(y => y)));
        }

        public static bool isEmpty<T>(IEnumerable<T> source)
        {
            return source.Count() == 0;
        }

        public static int length<T>(IEnumerable<T> source)
        {
            return source.Count();
        }

        public static T nth<T>(int index, IEnumerable<T> source)
        {
            return source.ElementAt(index);
        }

        public static IEnumerable<T> singleton<T>(T value)
        {
            yield return value;
        }

        public static IEnumerable<T[]> windowed<T>(int windowSize, IEnumerable<T> source)
        {
            return Enumerable.Range(0, source.Count() - windowSize + 1).Select(i => Enumerable.Range(i, windowSize).Select(j => source.ElementAt(j)).ToArray());
        }

        public static IEnumerable<T> replicate<T>(int count, T initial)
        {
            return Enumerable.Repeat(initial, count);
        }

        public static IEnumerable<T> slice<T>(IEnumerable<int> indices, IEnumerable<T> source)
        {
            return indices.Select(i => source.ElementAt(i));
        }

        public static IEnumerable<double> range(double start, double end, double step = 1)
        {
            for (double i = start; i <= end; i += step)
            {
                yield return i;
            }
        }
    }

#if net20

    /// <summary>
    /// 二元组
    /// </summary>
    /// <typeparam name="T1">元素1类型</typeparam>
    /// <typeparam name="T2">元素2类型</typeparam>
    public class Tuple<T1, T2>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }

        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public TResult ApplyFunction<TResult>(Func<T1, T2, TResult> f)
        {
            return f(Item1, Item2);
        }
    }

    /// <summary>
    /// 三元组
    /// </summary>
    /// <typeparam name="T1">元素1类型</typeparam>
    /// <typeparam name="T2">元素2类型</typeparam>
    /// <typeparam name="T3">元素3类型</typeparam>
    public class Tuple<T1, T2, T3>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }

        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public TResult ApplyFunction<TResult>(Func<T1, T2, T3, TResult> f)
        {
            return f(Item1, Item2, Item3);
        }
    }

    /// <summary>
    /// 元组创建
    /// </summary>
    public static class Tuple
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }

        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }
    }

#endif

    /// <summary>
    /// 函数的柯里化（也叫部分应用）、流水线、复合；流程控制表达式
    /// </summary>
    public static class FP
    {
        public static Action<T2> Currying<T1, T2>(this Action<T1, T2> f, T1 arg1)
        {
            return arg2 => f(arg1, arg2);
        }

        public static Action<T2, T3> Currying<T1, T2, T3>(this Action<T1, T2, T3> f, T1 arg1)
        {
            return (arg2, arg3) => f(arg1, arg2, arg3);
        }

        public static Func<T2, TResult> Currying<T1, T2, TResult>(this Func<T1, T2, TResult> f, T1 arg1)
        {
            return arg2 => f(arg1, arg2);
        }

        public static Func<T2, T3, TResult> Currying<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> f, T1 arg1)
        {
            return (arg2, arg3) => f(arg1, arg2, arg3);
        }

        public static U ApplyFunction<T, U>(this T arg, Func<T, U> f)
        {
            return f(arg);
        }

        public static void ApplyFunction<T>(this T arg, Action<T> f)
        {
            f(arg);
        }

        public static Func<T, V> ComposeWith<T, U, V>(this Func<U, V> f1, Func<T, U> f2)
        {
            return x => f1(f2(x));
        }

        public static Func<T, V> ComposedBy<T, U, V>(this Func<T, U> f1, Func<U, V> f2)
        {
            return x => f2(f1(x));
        }

        public static T IfThenElse<T>(Func<bool> ifExpr, Func<T> thenExpr, Func<T> elseExpr)
        {
            if (ifExpr())
            {
                return thenExpr();
            }
            else
            {
                return elseExpr();
            }
        }

        public static T TryCatch<T>(Func<T> tryExpr, Func<Exception, T> catchExpr)
        {
            try
            {
                return tryExpr();
            }
            catch (Exception ex)
            {
                return catchExpr(ex);
            }
        }

    }

    public class Option<T>
    {
        private bool _isNone = false;
        public bool IsNone
        {
            get
            {
                return _isNone;
            }
        }

        private static Option<T> _none = new Option<T> { _isNone = true };
        public static Option<T> None
        {
            get
            {
                return _none;
            }
        }

        private Option()
        {
        }

        private Option(T value)
        {
            _value = value;
        }

        private T _value = default(T);
        public static Option<T> Some(T value)
        {
            return new Option<T>(value);
        }
    }

    public class Delay<T> where T : class
    {
        private T _value = null;
        private Func<T> _expression;

        public Delay(Func<T> expression)
        {
            _expression = expression;
        }

        public T Value
        {
            get
            {
                if (_value == null)
                {
                    _value = _expression();
                }
                return _value;
            }
        }

        public bool IsValueCreated
        {
            get
            {
                return _value != null;
            }
        }
    }
}
