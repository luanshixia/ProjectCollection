using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using System.IO;

namespace TongJi.Gis.Web
{
    public static class CityEntryManager
    {
        public static List<string> GetAllCityNames()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/city_entries");
            var dir = new System.IO.DirectoryInfo(path);
            return dir.GetDirectories().Select(x => x.Name).ToList();
        }

        public static List<string> GetFilePaths(string cityName) // todo: 改为读配置
        {
            string path = GetEntryDirectoryPath(cityName);
            var dir = new System.IO.DirectoryInfo(path);
            if (dir.Exists)
            {
                return dir.GetFiles().Where(x => x.Name.EndsWith(".ciml")).Select(x => x.FullName).ToList();
            }
            else
            {
                return new List<string>();
            }
        }

        public static string GetEntryDirectoryPath(string cityName)
        {
            return HttpContext.Current.Server.MapPath(string.Format("~/App_Data/city_entries/{0}/", cityName));
        }

        public static string GetEntryMapFilePath(string cityName, string mapName)
        {
            return HttpContext.Current.Server.MapPath(string.Format("~/App_Data/city_entries/{0}/{1}.ciml", cityName, mapName));
        }
    }
}
