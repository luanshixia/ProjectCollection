using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Web.Controls;
using System.Web.UI.HtmlControls;

namespace TongJi.Web.CMS
{
    public class ImageRotator
    {
        public string Name { get; private set; }
        public Dictionary<string, string> Images { get; private set; }
        public int TransitInterval { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public ImageRotator(string name, Dictionary<string, string> images, int transitInterval, int width, int height)
        {
            Name = name;
            Images = images;
            TransitInterval = transitInterval;
            Width = width;
            Height = height;
        }

        public string GetHtml()
        {
            HtmlDivision div = new HtmlDivision();
            HtmlImage img = new HtmlImage();
            img.Attributes.Add("id", Name);
            HtmlScript script = new HtmlScript();
            System.IO.StringWriter js = new System.IO.StringWriter();
            js.Write(string.Format("var images = new Array({0});", Images.Count));
            js.Write(string.Format("var hrefs = new Array({0});", Images.Count));
            int i = 0;
            foreach (var image in Images)
            {
                js.Write(string.Format("images[{0}] = \"{1}\";", i, image.Key));
                js.Write(string.Format("hrefs[{0}] = \"{1}\";", i, image.Value));
                i++;
            }
            //js.Write("$(document).ready(function() { $('{0}').setInterval(function() {}, {}) });");
            script.InnerText = js.ToString();

            // todo: finish this

            return div.ToHtml();
        }
    }
}
