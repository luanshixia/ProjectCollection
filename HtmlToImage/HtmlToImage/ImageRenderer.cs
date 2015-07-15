using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.Extensions;

namespace HtmlToImage
{
    public class ImageRenderer
    {
        public void ExportAsync(string html, Stream output)
        {
            using (var driver = new PhantomJSDriver())
            {
                //string dataUri = string.Format("data:text/html;base64,{0}", 
                //    Convert.ToBase64String(new UnicodeEncoding().GetBytes(html)));
                string dataUri = string.Format("data:text/html,{0}", html);
                driver.Navigate().GoToUrl(dataUri);
                var screenshot = driver.TakeScreenshot();
                var outBytes = screenshot.AsByteArray;
                output.Write(outBytes, 0, outBytes.Length);
            }
        }
    }
}
