using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Microsoft.Win32;

namespace StartUp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class AcadProduct
    {
        public string Name;
        public string Path;
        public string Version;
        public string RegKey;
    }

    public class AcadProductManager
    {
        private List<AcadProduct> cads = new List<AcadProduct>();
        //private string[] allowedVersions = { "18.0", "18.1", "18.2" };

        public AcadProductManager()
        {
            string[] cadLocations = { "SOFTWARE\\Autodesk\\AutoCAD", "SOFTWARE\\Wow6432Node\\Autodesk\\AutoCAD" };

            foreach (var location in cadLocations)
            {
                RegistryKey x64 = Registry.LocalMachine.OpenSubKey(location);
                if (x64 == null) // running on x86 machines cannot get the second location
                {
                    continue;
                }
                string[] versions = x64.GetSubKeyNames();
                foreach (string version in versions)
                {
                    RegistryKey versionKey = x64.OpenSubKey(version);
                    string[] products = versionKey.GetSubKeyNames();
                    foreach (string product in products)
                    {
                        RegistryKey productKey = versionKey.OpenSubKey(product);
                        try
                        {
                            AcadProduct productDef = new AcadProduct { Name = productKey.GetValue("ProductName").ToString(), Path = productKey.GetValue("AcadLocation").ToString(), Version = version.Substring(1), RegKey = productKey.Name };
                            cads.Add(productDef);
                        }
                        catch
                        {
                        }
                        productKey.Close();
                    }
                    versionKey.Close();
                }
                x64.Close();
            }
        }

        public IEnumerable<AcadProduct> GetAllowedProducts()
        {
            return cads.Where(x => FileManager.AllowedVersions.Contains(x.Version));
        }

        public void AddLaunchInfo(string productRegKey)
        {
            Registry.SetValue(productRegKey + "\\Applications\\TongjiTemp", "LOADCTRLS", 2);
            Registry.SetValue(productRegKey + "\\Applications\\TongjiTemp", "MANAGED", 1);
            Registry.SetValue(productRegKey + "\\Applications\\TongjiTemp", "LOADER", FileManager.LoaderFullPath);
        }

        public void RemoveLaunchInfo(string productRegKey)
        {
            string key = productRegKey.Substring(productRegKey.IndexOf('\\') + 1);
            Registry.LocalMachine.DeleteSubKey(key + "\\Applications\\TongjiTemp", false);
        }
    }

    public class FileManager
    {
        public const string Cfg = "StartUp.cfg";
        public static Dictionary<string, Dictionary<string, string>> Config { get; private set; } // newly 20130119

        static FileManager()
        {
            Config = new Dictionary<string, Dictionary<string, string>>();
            ParseIniFile(Cfg, Config);
        }

        public static bool ParseIniFile(string fileName, Dictionary<string, Dictionary<string, string>> result)
        {
            string groupPattern = @"^\[[^\[\]]+\]$";
            string dataPattern = @"^[^=]+=[^=]+$";

            string[] lines = System.IO.File.ReadAllLines(fileName).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            Dictionary<string, string> group = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(line, groupPattern))
                    {
                        return false;
                    }
                    group = new Dictionary<string, string>();
                    string groupName = line.Trim('[', ']');
                    result.Add(groupName, group);
                }
                else
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(line, dataPattern))
                    {
                        return false;
                    }
                    string[] parts = line.Split('=').Select(x => x.Trim()).ToArray();
                    group.Add(parts[0], parts[1]);
                }
            }
            return true;
        }

        public static string CurrentFolder
        {
            get
            {
                string path = typeof(FileManager).Assembly.Location;
                return path.Remove(path.LastIndexOf('\\') + 1);
            }
        }

        public static string Loader
        {
            get
            {
                string loader = Config["StartUp"]["Loader"];
                if (string.IsNullOrEmpty(loader))
                {
                    return "Drawing.dll";
                }
                else
                {
                    return loader;
                }
            }
        }

        public static string[] AllowedVersions
        {
            get
            {
                return Config["StartUp"]["AllowedVersions"].Split(',').Select(x => x.Trim()).ToArray();
            }
        }

        public static string Server
        {
            get
            {
                return Config["StartUp"]["Server"];
            }
        }

        public static string LoaderFullPath
        {
            get
            {
                return CurrentFolder + Loader;
            }
        }
    }
}
