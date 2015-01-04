using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if DONE

namespace TongJi.Gis.Web
{
    public class PlanManager
    {
        public static Dictionary<string, string> ParcelUsage2011;
        public static Dictionary<string, string> ParcelColor2011;
        public static Dictionary<string, string> ParcelUsage;
        public static Dictionary<string, string> ParcelColor;

        static PlanManager()
        {
            var data1 = System.IO.File.ReadAllLines(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/LandUse.cfg")).Select(x => x.Split(',')).ToArray();
            var data2 = System.IO.File.ReadAllLines(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/LandUseOld.cfg")).Select(x => x.Split(',')).ToArray();
            ParcelUsage2011 = data1.ToDictionary(x => x[0], x => x[1]);
            ParcelColor2011 = data1.ToDictionary(x => x[0], x => x[2]);
            ParcelUsage = data2.ToDictionary(x => x[0], x => x[1]);
            ParcelColor = data2.ToDictionary(x => x[0], x => x[2]);
        }

        public static IEnumerable<IGrouping<string, entity_plan>> GetAllParcels(int cityId)
        {
            return LinqHelper.DataContext.entity_plan.Where(x => x.fk_cityid == cityId && x.active).ToList().GroupBy(x => string.IsNullOrEmpty(x.unit) ? "未归组地块" : x.unit).OrderBy(x => x.Key);
        }
        public static IEnumerable<IGrouping<string, entity_dynamic>> GetAllDynamicParcels(int cityId)
        {
            return LinqHelper.DataContext.entity_dynamic.Where(x => x.fk_cityid == cityId && x.active).ToList().GroupBy(x => string.IsNullOrEmpty(x.unit) ? "未归组地块" : x.unit).OrderBy(x => x.Key);
        }
        public static List<Tuple<string, List<entity_plan>>> GetAllTypeParcels(int cityId)
        {
            var parcels = LinqHelper.DataContext.entity_plan.Where(x => x.fk_cityid == cityId && x.active).ToList();
            return PlanManager.GetParcelTypeGroups(cityId).Select(x => Tuple.Create(x.Key, parcels.Where(y => x.Value(y.code)).ToList())).ToList();
            //return LinqHelper.DataContext.entity_plan.Where(x => x.fk_cityid == cityId && x.active).ToList().GroupBy(x => string.IsNullOrEmpty(x.code) ? "未知类型" : x.code).OrderBy(x => x.Key);
        }

        public static List<Tuple<string, List<entity_dynamic>>> GetAllTypeDynamicParcels(int cityId)
        {
            var parcels = LinqHelper.DataContext.entity_dynamic.Where(x => x.fk_cityid == cityId && x.active).ToList();
            return PlanManager.GetParcelTypeGroups(cityId).Select(x => Tuple.Create(x.Key, parcels.Where(y => x.Value(y.code)).ToList())).ToList();
            //return LinqHelper.DataContext.entity_plan.Where(x => x.fk_cityid == cityId && x.active).ToList().GroupBy(x => string.IsNullOrEmpty(x.code) ? "未知类型" : x.code).OrderBy(x => x.Key);
        }

        public static List<entity_plan> GetAllRecords(int cityId)
        {
            return LinqHelper.DataContext.entity_plan.Where(x => x.fk_cityid == cityId && x.active).ToList();
        }

        public static List<entity_dynamic> GetAllDynamicRecords(int cityId)
        {
            return LinqHelper.DataContext.entity_dynamic.Where(x => x.fk_cityid == cityId && x.active).ToList();
        }

        public static void SaveChanges()
        {
            LinqHelper.DataContext.SubmitChanges();
        }

        public static Dictionary<string, string> GetParcelUsage(int cityId)
        {
            bool useNewGB = CityEntryManager.GetDbRecord(cityId).is_new_gb == true;
            return GetParcelUsage(useNewGB);
        }

        public static Dictionary<string, string> GetParcelUsage(bool useNewGB)
        {
            if (useNewGB)
            {
                return PlanManager.ParcelUsage2011;
            }
            else
            {
                return PlanManager.ParcelUsage;
            }
        }

        public static Dictionary<string, string> GetParcelColor(int cityId)
        {
            bool useNewGB = CityEntryManager.GetDbRecord(cityId).is_new_gb == true;
            return GetParcelColor(useNewGB);
        }

        public static Dictionary<string, string> GetParcelColor(bool useNewGB)
        {
            if (useNewGB)
            {
                return PlanManager.ParcelColor2011;
            }
            else
            {
                return PlanManager.ParcelColor;
            }
        }

        public static Dictionary<string, Func<string, bool>> GetParcelTypeGroups(int cityId)
        {
            bool useNewGB = CityEntryManager.GetDbRecord(cityId).is_new_gb == true;
            return GetParcelTypeGroups(useNewGB);
        }

        public static Dictionary<string, Func<string, bool>> GetParcelTypeGroups(bool useNewGB)
        {
            if (useNewGB)
            {
                return ParcelTypes2011.AllTypes;
            }
            else
            {
                return ParcelTypes.AllTypes;
            }
        }

        public static UnitData GetUnitData(int cityId, int mapId, string unit = "")
        {
            var map = MapManager.GetMapData(mapId);
            var features = unit == "" ? map.Layers["地块"].Features : map.Layers["地块"].Features.Where(x => x.Unit() == unit);
            UnitData ud = new UnitData();
            ud.ParcelCount = features.Count();
            ud.Area = features.Sum(x => x.Area);
            ud.BuildingArea = features.Sum(x => x.Far() * x.Area);
            ud.FAR = ud.BuildingArea / ud.Area;
            ud.Parks = features.Sum(x => x.Parks());
            ud.GreenRate = 100 * (features.Sum(x => x.Area * x.Gr() / 100) + features.Where(x => x.Type().StartsWith("G")).Sum(x => x.Area)) / ud.Area;

            List<BalanceBlock> bbs = new List<BalanceBlock>();
            foreach (var type in PlanManager.GetParcelTypeGroups(cityId))
            {
                BalanceBlock bb = new BalanceBlock();
                var parcelsOfType = features.Where(x => type.Value(x.Type())).ToList();
                bb.Name = type.Key;
                bb.Count = parcelsOfType.Count;
                bb.Area = parcelsOfType.Sum(x => x.Area * x.Far());
                if (PlanManager.GetParcelColor(cityId).ContainsKey(type.Key))
                {
                    bb.Color = PlanManager.GetParcelColor(cityId)[type.Key].Remove(1, 2);
                }
                bbs.Add(bb);
            }
            ud.UnitsBalance = bbs;
            return ud;
        }

        public static List<ParcelData> GetParcelsData(int mapId, string unit)
        {
            List<ParcelData> pds = new List<ParcelData>();
            var map = MapManager.GetMapData(mapId);
            var features = map.Layers["地块"].Features.Where(x => x.Unit() == unit);
            foreach (var f in features)
            {
                ParcelData pd = new ParcelData();
                pd.Name = f.Name();
                pd.Code = f.Type();
                pd.Area = f.Area;
                pd.FAR = f.Far();
                pd.BuildingArea = f.Area * f.Far();
                pd.GreenRate = f.Gr();
                pd.BuildingHeight = f.Hl();
                pd.BuildingDensity = f.Bd();
                pd.Parks = f.Parks();
                pd.Remarks = f.Remark();
                pds.Add(pd);
            }
            return pds;
        }
    }

    public class UnitData
    {
        public int ParcelCount { get; set; }
        public double Area { get; set; }
        public double BuildingArea { get; set; }
        public double FAR { get; set; }
        public double GreenRate { get; set; }
        public int Parks { get; set; }
        public List<BalanceBlock> UnitsBalance { get; set; }
    }

    public class BalanceBlock
    {
        public string Color { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public double Area { get; set; }
    }

    public class ParcelData
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public double Area { get; set; }
        public double FAR { get; set; }
        public double BuildingArea { get; set; }
        public double GreenRate { get; set; }
        public double BuildingHeight { get; set; }
        public double BuildingDensity { get; set; }
        public int Parks { get; set; }
        public string Remarks { get; set; }
    }
}

#endif