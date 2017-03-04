using System;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// 地图数据管理
    /// </summary>
    public static class MapDataManager
    {
        private static int _lastSaveHashCode;
        public static string LatestFileName { get; private set; } // newly 20130221
        public static Map LatestMap { get; private set; }
        public static bool IsNewGB { get; private set; } // newly 20121221

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

        public static void New()
        {
            LatestFileName = null;
            LatestMap = new Map();
            OnMapDataChanged();
            _lastSaveHashCode = GetCurrentHashCode();
        }

        public static void Open(string fileName)
        {
            LatestFileName = fileName;
            LatestMap = new Map(fileName); 
            OnMapDataChanged();
            _lastSaveHashCode = GetCurrentHashCode();
        }

        public static void Import(Map map)
        {
            LatestMap = Map.Merge(LatestMap, map);
            OnMapDataChanged();
            _lastSaveHashCode = GetCurrentHashCode();
        }

        public static void SaveAs(string fileName)
        {
            LatestMap.Save(fileName);
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
    }
}
