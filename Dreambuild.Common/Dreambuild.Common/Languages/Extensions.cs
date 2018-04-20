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
