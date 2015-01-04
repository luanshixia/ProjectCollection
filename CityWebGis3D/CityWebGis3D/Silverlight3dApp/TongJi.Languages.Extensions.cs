using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace System
{
    public static class LocalExtensions
    {
        public static double TryParseToDouble(this string source)
        {
            double result = 0;
            double.TryParse(source, out result);
            return result;
        }

        public static int TryParseToInt32(this string source)
        {
            int result = 0;
            int.TryParse(source, out result);
            return result;
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
    }
}

namespace System.Xml.Linq
{
    public static class LocalExtensions
    {
        public static string AttValue(this XElement xe, string attName)
        {
            return xe.Attribute(attName) == null ? string.Empty : xe.Attribute(attName).Value;
        }

        public static void SetAttValue(this XElement xe, string attName, string attValue)
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
    }
}
