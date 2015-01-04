using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace TongJi.Web.Visualization
{
    public class MiniChart
    {
        public string Color { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public double BarHeight { get; set; }
        public double BarMaxLength { get; set; }
        public string BarLengthUnit { get; set; }

        public MiniChart(double max, double min = 0, double length = 200, string unit = "px")
        {
            MinValue = min;
            MaxValue = max;
            BarMaxLength = length;
            BarLengthUnit = unit;
        }

        public IHtmlString GetBar(double value)
        {
            XElement span0 = new XElement("span");
            XElement span1 = new XElement("span", " ", new XAttribute("style", string.Format("width:{0:0}{1}", GetBarLength(value), BarLengthUnit)), new XAttribute("class", "miniChartBar"));
            XElement span2 = new XElement("span", value, new XAttribute("class", "miniChartLabel"));
            span0.Add(span1, span2);
            return new HtmlString(span0.ToString());
        }

        private double GetBarLength(double value)
        {
            return (value / MaxValue) * BarMaxLength;
        }
    }
}
