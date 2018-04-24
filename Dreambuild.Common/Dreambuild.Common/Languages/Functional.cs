using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Functional
{
    /// <summary>
    /// Mimics Seq in F#
    /// </summary>
    public static class Seq
    {
        public static IEnumerable<U> map<T, U>(Func<T, U> mapping, IEnumerable<T> source)
        {
            return source.Select(mapping);
        }

        public static IEnumerable<U> mapi<T, U>(Func<int, T, U> mapping, IEnumerable<T> source)
        {
            return source.Select((element, i) => mapping(i, element));
        }

        public static IEnumerable<U> map2<T1, T2, U>(Func<T1, T2, U> mapping, IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            return source1.Zip(source2, mapping);
        }

        public static void iter<T>(Action<T> action, IEnumerable<T> source)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }

        public static void iteri<T>(Action<int, T> action, IEnumerable<T> source)
        {
            int i = 0;
            foreach (T element in source)
            {
                action(i, element);
                i++;
            }
        }

        public static void iter2<T1, T2>(Action<T1, T2> action, IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            source1.Zip(source2, (t1, t2) =>
            {
                action(t1, t2);
                return 0;
            });
        }

        public static IEnumerable<T> init<T>(int n, Func<int, T> initializer)
        {
            return Enumerable.Range(0, n).Select(initializer);
        }

        public static IEnumerable<T> filter<T>(Func<T, bool> predicate, IEnumerable<T> source)
        {
            return source.Where(predicate);
        }

        public static U fold<T, U>(Func<U, T, U> folder, U state, IEnumerable<T> source)
        {
            return source.Aggregate(state, folder);
        }

        public static IEnumerable<U> scan<T, U>(Func<U, T, U> folder, U state, IEnumerable<T> source)
        {
            foreach (var element in source)
            {
                state = folder(state, element);
                yield return state;
            }
        }

        public static T reduce<T>(Func<T, T, T> reduction, IEnumerable<T> source)
        {
            return source.Aggregate(reduction);
        }

        public static T head<T>(IEnumerable<T> source)
        {
            return source.First();
        }

        public static IEnumerable<T> tail<T>(IEnumerable<T> source)
        {
            return source.Skip(1);
        }

        public static IEnumerable<Tuple<T, T>> pairwise<T>(IEnumerable<T> source)
        {
            var list = source as IList<T> ?? source.ToList();
            return Enumerable.Range(0, list.Count - 1).Select(i => Tuple.Create(list[i], list[i + 1]));
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
            var list1 = source1 as IList<T> ?? source1.ToList();
            var list2 = source2 as IList<T> ?? source2.ToList();
            var scan = Seq.map2(comparer, list1, list2);
            int c = scan.FirstOrDefault(x => x != 0); // mod 20130528
            if (c != 0)
            {
                return c;
            }
            else
            {
                return list1.Count - list2.Count;
            }
        }

        public static IEnumerable<T> concat<T>(IEnumerable<IEnumerable<T>> sources)
        {
            return Seq.reduce(Seq.append, sources);
        }

        public static IEnumerable<T> flatten<T>(IEnumerable<IEnumerable<T>> sources)
        {
            return sources.SelectMany(x => x);
        }

        //public static IEnumerable<T> delay<T>(Func<IEnumerable<T>> generator)
        //{
        //    return generator();
        //}

        public static IEnumerable<T> distinctBy<T, U>(Func<T, U> projection, IEnumerable<T> source) where U : IEquatable<U>
        {
            return source.Select(projection).Distinct().Select(x => source.First(y => projection(y).Equals(x)));
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

        // option?
        public static T find<T>(Func<T, bool> predicate, IEnumerable<T> source)
        {
            return source.First(predicate);
        }

        public static int findIndex<T>(Func<T, bool> predicate, IEnumerable<T> source)
        {
            return source.ToList().IndexOf(source.First(predicate)); // TODO: optimize
        }

        public static bool forall<T>(Func<T, bool> predicate, IEnumerable<T> source)
        {
            return source.All(predicate);
        }

        public static bool forall2<T1, T2>(Func<T1, T2, bool> predicate, IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            return Seq.map2(predicate, source1, source2).All(x => x == true);
        }

        public static IEnumerable<IGrouping<U, T>> groupBy<T, U>(Func<T, U> projection, IEnumerable<T> source) where U : IEquatable<U>
        {
            return source.GroupBy(projection);
        }

        public static bool isEmpty<T>(IEnumerable<T> source)
        {
            return !source.Any();
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

        public static IEnumerable<T> fetch<T>(IEnumerable<int> indices, IEnumerable<T> source)
        {
            var indexSet = indices.ToHashSet();
            return source.Where((x, i) => indexSet.Contains(i));
        }

        public static IEnumerable<double> range(double start, double end, double step)
        {
            for (double x = start; x <= end; x += step)
            {
                yield return x;
            }
        }

        public static IEnumerable<int> range(int start, int end, int step)
        {
            for (int x = start; x <= end; x += step)
            {
                yield return x;
            }
        }

        public static IEnumerable<V> cross<T, U, V>(IEnumerable<T> ts, IEnumerable<U> us, Func<T, U, V> selector) // newly 20140423
        {
            return from t in ts from u in us select selector(t, u);
        }
    }

    /// <summary>
    /// Functional stuff
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

    public static class Option
    {
        public static Option<T> Some<T>(T value)
        {
            return Option<T>.Some(value);
        }
    }

    public class Option<T>
    {
        public bool IsNone { get; private set; }
        public bool IsSome { get { return !IsNone; } }
        public T Value { get; private set; }

        public static Option<T> None { get; private set; }

        static Option()
        {
            None = new Option<T>();
        }

        private Option()
        {
            IsNone = true;
        }

        private Option(T value)
        {
            IsNone = false;
            Value = value;
        }

        public static Option<T> Some(T value)
        {
            return new Option<T>(value);
        }
    }
}
