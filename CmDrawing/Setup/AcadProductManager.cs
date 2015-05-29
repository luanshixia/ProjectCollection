using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32;

namespace TongJi.Setup
{
    public class AcadProduct
    {
        public string Name;
        public string Path;
        public string Version;
        public string RegKey;
    }

    public static class AcadProductManager
    {
        private static List<AcadProduct> cads = new List<AcadProduct>();
        private static string allowedVersion = "18.0";

        static AcadProductManager()
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

        public static AcadProduct GetProduct()
        {
            return cads.FirstOrDefault(x => x.Version == allowedVersion);
        }
    }
}
