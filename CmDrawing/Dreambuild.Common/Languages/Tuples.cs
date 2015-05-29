using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if net20

namespace System
{
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
}

#endif