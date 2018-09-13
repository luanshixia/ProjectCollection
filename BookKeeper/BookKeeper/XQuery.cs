using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BookKeeper
{
    public class XQuery
    {
        public XElement Element => this.Elements.FirstOrDefault();

        public XElement[] Elements { get; }

        private XQuery(XElement[] elements)
        {
            this.Elements = elements;
        }

        public static XQuery Create(params XElement[] elements)
        {
            return new XQuery(elements);
        }

        public static XQuery Create(IEnumerable<XElement> elements)
        {
            return new XQuery(elements.ToArray());
        }

        public XQuery Find(XName name = null, Func<XElement, bool> filter = null)
        {
            if (name == null && filter == null)
            {
                return this;
            }

            if (name == null)
            {
                return new XQuery(this.Element.Descendants().Where(filter).ToArray());
            }

            if (filter == null)
            {
                return new XQuery(this.Element.Descendants(name).ToArray());
            }

            return new XQuery(this.Element.Descendants(name).Where(filter).ToArray());
        }

        public string Attr(XName name)
        {
            return this.Element?.AttValue(name);
        }

        public string Text()
        {
            return this.Element?.Value;
        }
    }

    /// <summary>
    /// Extension methods for <see cref="XElement"/>.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Gets attribute value.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="attName"></param>
        /// <returns></returns>
        public static string AttValue(this XElement xe, XName attName)
        {
            return xe.Attribute(attName) == null ? string.Empty : xe.Attribute(attName).Value;
        }

        /// <summary>
        /// Sets attribute value.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="attName"></param>
        /// <param name="attValue"></param>
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

        /// <summary>
        /// Gets XElement safely.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement ElementX(this XElement xe, XName name)
        {
            return xe.Element(name) ?? new XElement(name);
        }

        /// <summary>
        /// Gets element value.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="eleName"></param>
        /// <returns></returns>
        public static string EleValue(this XElement xe, XName eleName)
        {
            return xe.Element(eleName) == null ? string.Empty : xe.Element(eleName).Value;
        }
    }
}
