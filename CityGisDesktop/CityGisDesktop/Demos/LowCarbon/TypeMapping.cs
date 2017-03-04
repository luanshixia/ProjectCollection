using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    class TypeMapping
    {
        static Dictionary<string, string> _typeDict;
        const string FileName = @"Configs\LandUse.cfg";

        static TypeMapping()
        {
            LoadTypeDict();
        }

        static void LoadTypeDict()
        {
            _typeDict = System.IO.File.ReadAllLines(FileName, Encoding.Default)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(y => y.Trim())
                    .ToArray())
                .ToDictionary(x => x[0], x => x[3]);
        }

        public static BuildingType Map(string rawType)
        {
            string buildingType = rawType;
            if (_typeDict.ContainsKey(rawType))
            {
                buildingType = _typeDict[rawType];
            }
            switch (buildingType)
            {
                case "住宅":
                    return BuildingType.Residencial;
                case "办公":
                    return BuildingType.Office;
                case "旅馆":
                    return BuildingType.Hotel;
                case "商场":
                    return BuildingType.Retail;
                case "学校":
                    return BuildingType.Education;
                case "工业":
                    return BuildingType.Industry;
                case "其他":
                    return BuildingType.Other;
                case "绿地":
                    return BuildingType.GreenField;
                case "混合":
                    return BuildingType.Mixed;
                default:
                    return BuildingType.Unknown;
            }
        }
    }
}
