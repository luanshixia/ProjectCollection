using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace BookKeeper
{
    public class XQuery
    {
        public XElement Element { get; }

        private XQuery(XElement element)
        {
            this.Element = element;
        }

        public static XQuery Create(XElement element)
        {
            return new XQuery(element);
        }

        public XQuery Find()
        {

        }
    }
}
