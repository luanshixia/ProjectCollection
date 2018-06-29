using Dreambuild.Collections;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dreambuild.IO
{
    /// <summary>
    /// INI file creation and parsing - WY20110830 - WY20170303 - WY20180628
    /// </summary>
    public class IniFile
    {
        public CIDictionary<CIDictionary<string>> Data { get; } = new CIDictionary<CIDictionary<string>>();

        private const string GroupPattern = @"^\[[^\[\]]+\]$";

        private const string DataPattern = @"^[^=]+=[^=]+$";

        public static IniFile Load(string fileName)
        {
            var file = new IniFile();

            var lines = File
                .ReadAllLines(fileName)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line))
                .ToArray();

            var group = "%%%";
            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                {
                    if (Regex.IsMatch(line, IniFile.GroupPattern))
                    {
                        group = line.Trim('[', ']');
                        file.Data[group] = new CIDictionary<string>();
                    }
                }
                else
                {
                    if (Regex.IsMatch(line, IniFile.DataPattern))
                    {
                        var parts = line.Split('=').Select(part => part.Trim()).ToArray();
                        file.Data[group][parts[0]] = parts[1];
                    }
                }
            }

            return file;
        }

        public void Save(string fileName)
        {
            using (var sw = new StreamWriter(new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                foreach (var group in this.Data)
                {
                    sw.WriteLine("[{0}]", group.Key);
                    foreach (var entry in group.Value)
                    {
                        sw.WriteLine("{0}={1}", entry.Key, entry.Value);
                    }
                    sw.WriteLine();
                }
            }
        }

        public string[] GetGroups()
        {
            return this.Data.Keys.ToArray();
        }

        public string[] GetEntries(string group)
        {
            if (this.Data.ContainsKey(group))
            {
                return this.Data[group].Keys.ToArray();
            }

            return Array.Empty<string>();
        }
    }
}
