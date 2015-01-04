using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopClient
{
    /// <summary>
    /// 提供以CSV配置的字符串资源的解析方式，通过数据绑定实现换肤、多语言等。
    /// </summary>
    public class StringManager
    {
        private Dictionary<string, string> _dict = new Dictionary<string, string>();

        public StringManager(string fileName)
        {
            var lines = System.IO.File.ReadAllLines(fileName, Encoding.Default).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim().Split(',')).ToList();
            foreach (var line in lines)
            {
                _dict.Add(line[0], line[1]);
            }
        }

        public string this[string key]
        {
            get
            {
                return _dict[key];
            }
        }
    }

    public static class StringResources
    {
        public static Dictionary<string, string> Test = new Dictionary<string, string>
        {
            { "banner", "title.jpg" }
        };

        public static Dictionary<string, string> ChineseLanguage = new Dictionary<string, string>
        {
            { "banner", "title.jpg" }
        };

        public static Dictionary<string, string> EnglishLanguage = new Dictionary<string, string>
        {
            { "banner", "title.jpg" }
        };

        public static Dictionary<string, string> YellowTheme = new Dictionary<string, string>
        {
            { "banner", "title.jpg" }
        };
    }
}
