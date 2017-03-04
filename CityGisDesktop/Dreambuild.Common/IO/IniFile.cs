using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.IO
{
    /// <summary>
    /// 提供INI文件的解析和创建方法 WY20110830
    /// </summary>
    public class IniFile
    {
        private Dictionary<string, Dictionary<string, string>> _data = new Dictionary<string, Dictionary<string, string>>();

        public bool CaseFree { get; private set; }

        public string this[string group, string key]
        {
            get
            {
                if (_data.Keys.ContainsX(group, CaseFree))
                {
                    if (_data.DictValue(group, CaseFree).Keys.ContainsX(key, CaseFree))
                    {
                        return _data.DictValue(group, CaseFree).DictValue(key, CaseFree);
                    }
                }
                return null;
            }
            set
            {
                if (!_data.Keys.ContainsX(group, CaseFree))
                {
                    _data.Add(group, new Dictionary<string, string>());
                }
                if (_data.DictValue(group, CaseFree).Keys.ContainsX(key, CaseFree))
                {
                    _data.DictValue(group, CaseFree).SetDictValue(key, value, CaseFree);
                }
                else
                {
                    _data.DictValue(group, CaseFree).Add(key, value);
                }
            }
        }

        public IniFile(bool caseFree = false)
        {
            CaseFree = caseFree;
        }

        public static IniFile Load(string fileName, bool caseFree = false)
        {
            IniFile file = new IniFile(caseFree);
            string groupPattern = @"^\[[^\[\]]+\]$";
            string dataPattern = @"^[^=]+=[^=]+$";

            string[] lines = System.IO.File.ReadAllLines(fileName).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            string group = "%%%";
            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(line, groupPattern))
                    {
                        continue;
                    }
                    group = line.Trim('[', ']');
                }
                else
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(line, dataPattern))
                    {
                        continue;
                    }
                    string[] parts = line.Split('=').Select(x => x.Trim()).ToArray();
                    file[group, parts[0]] = parts[1];
                }
            }

            return file;
        }

        public void Save(string fileName)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName))
            {
                foreach (var group in _data)
                {
                    sw.WriteLine("[{0}]", group.Key);
                    foreach (var item in group.Value)
                    {
                        sw.WriteLine("{0}={1}", item.Key, item.Value);
                    }
                    sw.WriteLine();
                }
            }
        }

        public string[] GetGroups()
        {
            return _data.Keys.ToArray();
        }

        public string[] GetEntries(string group)
        {
            if (_data.Keys.ContainsX(group, CaseFree))
            {
                return _data.DictValue(group, CaseFree).Keys.ToArray();
            }
            else
            {
                return new string[0];
            }
        }
    }

    public static class IniFileExtensions
    {
        public static T CaseFreeDictValue<T>(this Dictionary<string, T> source, string key)
        {
            return source.First(x => x.Key.ToUpper() == key.ToUpper()).Value;
        }

        public static void SetCaseFreeDictValue<T>(this Dictionary<string, T> source, string key, T value)
        {
            string realKey = source.First(x => x.Key.ToUpper() == key.ToUpper()).Key;
            source[realKey] = value;
        }

        public static T DictValue<T>(this Dictionary<string, T> source, string key, bool caseFree)
        {
            if (caseFree)
            {
                return source.CaseFreeDictValue(key);
            }
            else
            {
                return source[key];
            }
        }

        public static void SetDictValue<T>(this Dictionary<string, T> source, string key, T value, bool caseFree)
        {
            if (caseFree)
            {
                source.SetCaseFreeDictValue(key, value);
            }
            else
            {
                source[key] = value;
            }
        }

        public static bool ContainsX(this IEnumerable<string> source, string value, bool caseFree)
        {
            if (caseFree)
            {
                return source.CaseFreeContains(value);
            }
            else
            {
                return source.Contains(value);
            }
        }

        public static bool CaseFreeContains(this IEnumerable<string> source, string value)
        {
            return source.Select(x => x.ToUpper()).Contains(value.ToUpper());
        }
    }
}
