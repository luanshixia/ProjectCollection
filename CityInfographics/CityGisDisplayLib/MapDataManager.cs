using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;

namespace TongJi.Gis.Display
{
    /// <summary>
    /// 查询操作
    /// </summary>
    public enum QueryOperation
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan
    }

    /// <summary>
    /// 地图数据管理
    /// </summary>
    public static class MapDataManager
    {
        private static int _lastSaveHashCode;
        public static string LatestFileName { get; private set; } // newly 20130221
        public static Map LatestMap { get; private set; }
        public static bool IsNewGB { get; private set; } // newly 20121221
        public static System.Windows.Window AppMainWindow { get; set; }

        public static event Action MapDataChanged;
        public static void OnMapDataChanged()
        {
            if (MapDataChanged != null)
            {
                MapDataChanged();
            }
        }

        static MapDataManager()
        {
            LatestMap = new Map();
            _lastSaveHashCode = GetCurrentHashCode();
        }

        public static void Open(string fileName)
        {
            LatestFileName = fileName;
            LatestMap = new Map(XDocument.Load(fileName).Root);
            //MapDataManager.CleanMap(LatestMap);
            OnMapDataChanged();
            _lastSaveHashCode = GetCurrentHashCode();
        }

        public static void SaveAs(string fileName)
        {
            LatestMap.ToXMap().Save(fileName);
            _lastSaveHashCode = GetCurrentHashCode();
        }

        private static int GetCurrentHashCode()
        {
            return LatestMap.ToXMap().ToString().GetHashCode();
        }

        public static bool IsHashChanged()
        {
            return _lastSaveHashCode != GetCurrentHashCode();
        }

        public static void FindAndMarkFeatures(string searchWord)
        {
            MarkFeatures(FindFeatures(searchWord));
        }

        public static IEnumerable<IFeature> QueryFeatures(string layer, string prop, QueryOperation operation, object param)
        {
            return MapControl.Current.AllMaps.SelectMany(x => x.Layers[layer].Features).Where(x => FeatureSelector(x, prop, operation, param)).ToList();
        }

        public static IEnumerable<IFeature> FindFeatures(string searchWord)
        {
            foreach (var map in MapControl.Current.AllMaps)
            {
                foreach (var layer in map.Layers)
                {
                    foreach (var feature in layer.Features)
                    {
                        if (FeatureSelector(feature, searchWord))
                        {
                            yield return feature;
                        }
                    }
                }
            }
        }

        public static bool FeatureSelector(IFeature feature, string searchWord)
        {
            return feature.FeatId.ToUpper().Contains(searchWord.ToUpper()) || feature.Properties.Any(x => x.Value.ToUpper().Contains(searchWord.ToUpper()));
        }

        public static bool FeatureSelector(IFeature feature, string prop, QueryOperation operation, object param)
        {
            if (operation == QueryOperation.Equals)
            {
                return feature.Properties.ContainsKey(prop) && ((param.IsNumber() && feature[prop].TryParseToDouble() == Convert.ToDouble(param)) || (feature[prop] == param.ToString()));
            }
            else if (operation == QueryOperation.NotEquals)
            {
                return feature.Properties.ContainsKey(prop) && ((param.IsNumber() && feature[prop].TryParseToDouble() != Convert.ToDouble(param)) || (feature[prop] != param.ToString()));
            }
            else if (operation == QueryOperation.GreaterThan)
            {
                return feature.Properties.ContainsKey(prop) && (param.IsNumber() && feature[prop].TryParseToDouble() > Convert.ToDouble(param));
            }
            else if (operation == QueryOperation.LessThan)
            {
                return feature.Properties.ContainsKey(prop) && (param.IsNumber() && feature[prop].TryParseToDouble() < Convert.ToDouble(param));
            }
            return false;
        }

        private static bool IsNumber(this object param)
        {
            double num;
            return param is int || param is double || param is decimal || param is float || double.TryParse(param.ToString(), out num);
        }

        public static void MarkFeatures(IEnumerable<IFeature> features)
        {
            MapControl.Current.Mark(features.Select(x => GetMarkPoint(x)).ToArray());
        }

        private static Geometry.Point2D GetMarkPoint(IFeature feature)
        {
            return new Geometry.Polygon(feature.GeoData).Points[0];
        }

        //private static void CleanMap(Map map)
        //{
        //    foreach (var layer in map.Layers)
        //    {
        //        var features = layer.Features.ToList();
        //        foreach (var feature in features)
        //        {
        //            if (string.IsNullOrEmpty(feature.GeoData))
        //            {
        //                layer.Features.Remove(feature);
        //            }
        //        }
        //    }
        //}
    }
}
