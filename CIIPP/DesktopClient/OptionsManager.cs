using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace DesktopClient
{
    public static class OptionsManager
    {
        public static Dictionary<string, string> Options { get; private set; }
        private const string FileName = "options.ini";

        static OptionsManager()
        {
            Options = new Dictionary<string, string>();
            var lines = File.ReadAllLines(FileName).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim().Split('=')).ToList();
            lines.ForEach(x => Options.Add(x[0], x[1]));
        }

        public static void Save()
        {
            using (StreamWriter sw = new StreamWriter(FileName))
            {
                foreach (var option in Options)
                {
                    sw.WriteLine("{0}={1}", option.Key, option.Value);
                }
            }
        }
    }
}
