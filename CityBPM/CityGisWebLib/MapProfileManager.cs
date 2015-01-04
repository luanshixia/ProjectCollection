using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TongJi.Gis.Web
{
    public class MapProfileManager
    {
        public static List<string> GetAllProfileNames()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/map_profiles"); // string.Format("{0}\\storage\\map_profiles\\", HttpContext.Current.Request.PhysicalApplicationPath);
            var dir = new System.IO.DirectoryInfo(path);
            return dir.GetDirectories().Select(x => x.Name).ToList();
        }

        public static List<string> GetFilePaths(string profileName) // todo: 改为读配置
        {
            string path = GetProfileDirectoryPath(profileName);
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

        public static string GetProfileDirectoryPath(string profileName)
        {
            return HttpContext.Current.Server.MapPath(string.Format("~/App_Data/map_profiles/{0}/", profileName));
        }

        public static string GetProfileConfigFilePath(string profileName)
        {
            return HttpContext.Current.Server.MapPath(string.Format("~/App_Data/map_profiles/{0}/config.xml", profileName));
        }
    }

    public class FactorLayerParameters
    {
        [Required]
        [Display(Name = "因素名称")]
        public string Name { get; set; }
        [Required]
        [RegularExpression("spot|linear|region")]
        [Display(Name = "因素类型")]
        public string Type { get; set; }
        [Range(0, 10000)]
        [Display(Name = "影响半径")]
        public double Radius { get; set; }
        [Range(-1, 1)]
        [Display(Name = "居住权重")]
        public double RWeight { get; set; }
        [Range(-1, 1)]
        [Display(Name = "商办权重")]
        public double CWeight { get; set; }
    }

    public static class FactorLayerParametersManager
    {
        public static List<FactorLayerParameters> Load()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/PlanTechParameters.json");
            if (System.IO.File.Exists(path))
            {
                return System.Web.Helpers.Json.Decode<List<FactorLayerParameters>>(System.IO.File.ReadAllText(path));
            }
            else
            {
                return new List<FactorLayerParameters>();
            }
        }

        public static void Save(List<FactorLayerParameters> source)
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/PlanTechParameters.json");
            System.IO.File.WriteAllText(path, System.Web.Helpers.Json.Encode(source));
        }
    }
}
