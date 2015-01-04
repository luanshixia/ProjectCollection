using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Gis;
using System.Xml.Linq;

#if DONE

namespace TongJi.Gis.Web
{
    public static class DPlan
    {
        //
        // DPlan 作为模块名
        //

        public const string ParcelLayerName = "地块";
        public const string ParcelUnitPropName = "控规单元";
        public const string ControlLineLayerName = "控制线";
        public const string UnitLayerName = "控规单元";

        public static List<string> GetAllUnits(Map plan)
        {
            return plan.Layers[DPlan.ParcelLayerName].Features.Select(x => x.Unit()).Distinct().OrderBy(x => x).ToList();
        }
    }

    // todo: 清理第四期开发中用到DPlan/ParcelUnit的内容

    //public class DPlan
    //{
    //    public List<string> Messages { get; private set; }
    //    private int _cityId;
    //    public int CityId { get { return _cityId; } }

    //    public Map Plan { get; private set; }
    //    public Map Permitted { get; private set; }
    //    public string PlanFile { get; private set; }
    //    public string PermittedFile { get; private set; }

    //    public DPlan(int cityId)
    //    {
    //        _cityId = cityId;
    //        var city = CityEntryManager.GetDbRecord(cityId);
    //        Messages = new List<string>();

    //        if (city.map_planning == null)
    //        {
    //            throw new Exception("旧版数据，请升级到新版地图。");
    //        }
    //        PlanFile = MapManager.GetFullFileName((int)city.map_planning);
    //        PermittedFile = MapManager.GetFullFileName((int)city.map_dynamic);
    //        if (System.IO.File.Exists(PlanFile))
    //        {
    //            Plan = new Map(XDocument.Load(PlanFile).Root);
    //        }
    //        else
    //        {
    //            Plan = new Map();
    //            Plan.Layers.Add(new VectorLayer(ParcelLayerName, "4"));
    //        }
    //        if (System.IO.File.Exists(PermittedFile))
    //        {
    //            Permitted = new Map(XDocument.Load(PermittedFile).Root);
    //        }
    //        else
    //        {
    //            Permitted = new Map();
    //            Permitted.Layers.Add(new VectorLayer(ParcelLayerName, "4"));
    //        }
    //    }

    //    public DPlan(string planFile, string permittedFile)
    //    {
    //        Messages = new List<string>();
    //        PlanFile = planFile;
    //        PermittedFile = permittedFile;
    //        if (System.IO.File.Exists(PlanFile))
    //        {
    //            Plan = new Map(XDocument.Load(PlanFile).Root);
    //        }
    //        else
    //        {
    //            Plan = new Map();
    //            Plan.Layers.Add(new VectorLayer(ParcelLayerName, "4"));
    //        }
    //        if (System.IO.File.Exists(PermittedFile))
    //        {
    //            Permitted = new Map(XDocument.Load(PermittedFile).Root);
    //        }
    //        else
    //        {
    //            Permitted = new Map();
    //            Permitted.Layers.Add(new VectorLayer(ParcelLayerName, "4"));
    //        }
    //    }

    //    public static bool IsCityReadyToOfferHongxian(int cityId)
    //    {
    //        return HongxianManager.GetAllHongxians(cityId).All(x => !x.can_mod);
    //    }

    //    public static bool IsHongxianAvailable(int hongxianId)
    //    {
    //        return !HongxianManager.GetDbRecord(hongxianId).can_mod;
    //    }

    //    public void JustInsertOrUpdate(IFeature hongxian)
    //    {
    //        int toUpdate = Permitted.Layers[DPlan.ParcelLayerName].Features.FindIndex(x => x.Name() == hongxian.Name());
    //        if (toUpdate > -1)
    //        {
    //            Permitted.Layers[DPlan.ParcelLayerName].Features[toUpdate] = hongxian;
    //        }
    //        else
    //        {
    //            Permitted.Layers[DPlan.ParcelLayerName].Features.Add(hongxian);
    //        }
    //        Permitted.ToXMap().Save(PermittedFile);
    //    }

    //    public void JustDelete(string name)
    //    {
    //        var record = Permitted.Layers[DPlan.ParcelLayerName].Features.First(x => x.Name() == name);
    //        Permitted.Layers[DPlan.ParcelLayerName].Features.Remove(record);
    //        Permitted.ToXMap().Save(PermittedFile);
    //    }

    //    public bool Verify()
    //    {
    //        List<Constraint> constraints = GetAllUnits(Plan).SelectMany(x => ConstraintHelper.BuildBasicConstraintList(x, CityEntryManager.GetDbRecord(_cityId).is_new_gb == true)).ToList();
    //        constraints.Add(GetBizConstraint());
    //        return Verify(Plan, Permitted, constraints);
    //    }

    //    public bool Verify(Map plan, Map dynamic, List<Constraint> constraints)
    //    {
    //        Messages.Clear();
    //        var bools = constraints.Select(x => x(plan, dynamic, Messages)).ToList(); ;
    //        return bools.All(x => x);
    //    }

    //    public Constraint GetBizConstraint()
    //    {
    //        return (plan, dynamic, messages) =>
    //        {
    //            var hongxians = HongxianManager.GetAllHongxians(_cityId);
    //            int count0 = messages.Count;
    //            foreach (var hongxian in hongxians)
    //            {
    //                if (string.IsNullOrEmpty(hongxian.name))
    //                {
    //                    messages.Add(string.Format("ID={0}的红线没有名称。（忘记填写信息？）", hongxian.id));
    //                }
    //                if (!hongxian.has_geodata)
    //                {
    //                    messages.Add(string.Format("ID={0}的红线没有GIS数据。（是否是无用记录，需要清理？）", hongxian.id));
    //                }
    //            }
    //            return messages.Count == count0;
    //        };
    //    }

    //    //
    //    // DPlan 作为模块名
    //    //

    //    public const string ParcelLayerName = "地块";
    //    public const string ParcelUnitPropName = "控规单元";

    //    public static List<string> GetAllUnits(Map plan)
    //    {
    //        return plan.Layers[DPlan.ParcelLayerName].Features.Select(x => x.Unit()).Distinct().OrderBy(x => x).ToList();
    //    }
    //}

    public class UPlanDetectCategory
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Result { get; set; }
        public List<string> Messages { get; set; }

        public UPlanDetectCategory(string name, string displayName, Map plan, Map dynamic, List<Constraint> constraints)
        {
            Name = name;
            DisplayName = displayName;
            Messages = new List<string>();
            UPlanDetect.Verify(plan, dynamic, constraints, this);
        }
    }

    public class UPlanDetect
    {
        public List<string> Messages { get; private set; }
        public Map Plan0 { get; private set; }
        public Map Plan1 { get; private set; }
        public string Unit { get; private set; }
        public bool UseNewGB { get; private set; }
        public List<UPlanDetectCategory> Categories { get; private set; }

        public UPlanDetect(bool useNewGB, Map plan0, Map plan1, string unit = null)
        {
            Messages = new List<string>();
            Unit = unit;
            Plan0 = plan0;
            Plan1 = plan1;
            UseNewGB = useNewGB;
            Categories = new List<UPlanDetectCategory>();
        }

        public bool Verify()
        {
            List<Constraint> constraints = (Unit == null) ? DPlan.GetAllUnits(Plan0).SelectMany(x => ConstraintHelper.BuildBasicConstraintList(x, UseNewGB)).ToList() : ConstraintHelper.BuildBasicConstraintList(Unit, UseNewGB);
            List<Constraint> clConstraints = ConstraintHelper.BuildCtrlLineConstraintList(Unit);
            List<Constraint> pfConstraints = ConstraintHelper.BuildPublicFacilityConstraintList(Unit);
            List<Constraint> areaConstraints = (Unit == null) ? DPlan.GetAllUnits(Plan0).SelectMany(x => ConstraintHelper.BuildAreaConstraintList(x, UseNewGB)).ToList() : ConstraintHelper.BuildAreaConstraintList(Unit, UseNewGB);
            List<Constraint> muConstraints = ConstraintHelper.BuildMainUseConstraintList(Unit);

            Categories.Add(new UPlanDetectCategory("All", "全部", Plan0, Plan1, constraints));
            Categories.Add(new UPlanDetectCategory("ControlLine", "五线要求", Plan0, Plan1, clConstraints));
            Categories.Add(new UPlanDetectCategory("PublicFacility", "三大设施", Plan0, Plan1, pfConstraints));
            Categories.Add(new UPlanDetectCategory("Area", "地块指标", Plan0, Plan1, areaConstraints));
            Categories.Add(new UPlanDetectCategory("MainUse", "单元指标", Plan0, Plan1, muConstraints));

            return Categories[0].Result;
        }

        public bool Verify(Map plan, Map dynamic, List<Constraint> constraints, List<string> messages)
        {
            var bools = constraints.Select(x => x(plan, dynamic, messages)).ToList();
            return bools.All(x => x);
        }

        public static void Verify(Map plan, Map dynamic, List<Constraint> constraints, UPlanDetectCategory category) // newly 20130407
        {
            var bools = constraints.Select(x => x(plan, dynamic, category.Messages)).ToList();
            category.Result = bools.All(x => x);
        }
    }

    public class UPlanMutate
    {
        public Map Plan { get; private set; }
        public string PlanMapFile { get; private set; }

        public UPlanMutate(string mapFile)
        {
            PlanMapFile = mapFile;
            if (System.IO.File.Exists(PlanMapFile))
            {
                Plan = new Map(PlanMapFile);
            }
            else
            {
                Plan = new Map();
                Plan.Layers.Add(new VectorLayer(DPlan.ParcelLayerName, "4"));
            }
        }

        public void JustInsertOrUpdate(IFeature hongxian)
        {
            int toUpdate = Plan.Layers[DPlan.ParcelLayerName].Features.FindIndex(x => x.Name() == hongxian.Name());
            if (toUpdate > -1)
            {
                Plan.Layers[DPlan.ParcelLayerName].Features[toUpdate] = hongxian;
            }
            else
            {
                Plan.Layers[DPlan.ParcelLayerName].Features.Add(hongxian);
            }
            Plan.ToXMap().Save(PlanMapFile);
        }

        public void JustDelete(string name)
        {
            var record = Plan.Layers[DPlan.ParcelLayerName].Features.First(x => x.Name() == name);
            Plan.Layers[DPlan.ParcelLayerName].Features.Remove(record);
            Plan.ToXMap().Save(PlanMapFile);
        }
    }

    //public class ParcelUnit
    //{
    //    private DPlan _dyn;
    //    public string Name { get; private set; }
    //    public List<ParcelIndex> InfoList { get; private set; }
    //    public string TypeFilter { get; private set; }
    //    public List<IFeature> Plan { get; private set; }
    //    public List<IFeature> Permitted { get; private set; }

    //    public ParcelUnit(DPlan dyn, string name, string type = null)
    //    {
    //        _dyn = dyn;
    //        Name = name;

    //        TypeFilter = type;
    //        bool useNewGB = CityEntryManager.GetDbRecord(_dyn.CityId).is_new_gb == true;
    //        var usage = new ParcelUsageType(useNewGB, TypeFilter);
    //        Plan = _dyn.Plan.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(Name) && x.IsType(usage)).ToList();
    //        Permitted = _dyn.Permitted.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(Name) && x.IsType(usage)).ToList();

    //        InfoList = new List<ParcelIndex>();
    //        InfoList.Add(new ParcelIndex());
    //        InfoList.Add(new ParcelIndex());
    //        InfoList.Add(new ParcelIndex());
    //        MakeInfoList();
    //    }

    //    private void MakeInfoList()
    //    {
    //        InfoList[0].Tag = "规划";
    //        InfoList[0].Count = Plan.Count;
    //        InfoList[0].Area = Plan.Sum(x => x.Area());
    //        InfoList[0].Floor = Plan.Sum(x => x.Area() * x.Far());
    //        InfoList[0].Far = InfoList[0].Floor / InfoList[0].Area;
    //        InfoList[0].Bd = Plan.Sum(x => x.Area() * x.Bd()) / InfoList[0].Area;

    //        InfoList[1].Tag = "已批";
    //        InfoList[1].Count = Permitted.Count;
    //        InfoList[1].Area = Permitted.Sum(x => x.Area());
    //        InfoList[1].Floor = Permitted.Sum(x => x.Area() * x.Far());
    //        InfoList[1].Far = InfoList[1].Floor / InfoList[1].Area;
    //        InfoList[1].Bd = Permitted.Sum(x => x.Area() * x.Bd()) / InfoList[1].Area;

    //        InfoList[2].Tag = "剩余";
    //        InfoList[2].Count = -1;
    //        InfoList[2].Area = InfoList[0].Area - InfoList[1].Area;
    //        InfoList[2].Floor = InfoList[0].Floor - InfoList[1].Floor;
    //        InfoList[2].Far = InfoList[2].Floor / InfoList[2].Area;
    //        InfoList[2].Bd = (Plan.Sum(x => x.Area() * x.Bd()) - Permitted.Sum(x => x.Area() * x.Bd())) / InfoList[2].Area;
    //    }
    //}

    //public class ParcelIndex
    //{
    //    public string Tag;
    //    public int Count;
    //    public double Area;
    //    public double Floor;
    //    public double Far;
    //    public double Bd;
    //}

    //public class ConstraintResult
    //{
    //    public bool Passed { get; private set; }
    //}

    // todo: 重新设计这个模式，使得messages直接包含出错的单元和地块信息。
    public delegate bool Constraint(Map plan, Map dynamic, List<string> messages);

    public class ParcelUsageType
    {
        public bool IsNewGB;
        public string TypeCode;

        public ParcelUsageType(bool isNewGB, string typeCode)
        {
            IsNewGB = isNewGB;
            TypeCode = typeCode;
        }

        public override string ToString()
        {
            return TypeCode;
        }
    }

    public static class ConstraintHelper
    {
        public static bool IsType(this IFeature f, ParcelUsageType type)
        {
            //return f.Properties["用地代码"].StartsWith(type);
            if (string.IsNullOrEmpty(type.TypeCode))
            {
                return true;
            }
            if (type.IsNewGB)
            {
                if (ParcelTypes2011.AllTypes.ContainsKey(type.TypeCode))
                {
                    return ParcelTypes2011.AllTypes[type.TypeCode](f.Type());
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (ParcelTypes.AllTypes.ContainsKey(type.TypeCode))
                {
                    return ParcelTypes.AllTypes[type.TypeCode](f.Type());
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsType(this IFeature f, string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return true;
            }
            if (ParcelTypes2011.AllTypes.ContainsKey(type))
            {
                return ParcelTypes2011.AllTypes[type](f.Type());
            }
            else
            {
                return false;
            }
        }

        private static bool IsIndustrial(this IFeature f)
        {
            return f.IsType("M") || f.IsType("W") || f.IsType("U");
        }

        public static bool IsInUnit(this IFeature f, string unit)
        {
            if (unit == "全部地块")
            {
                return true;
            }
            else if (unit == "未归组地块")
            {
                return (!f.Properties.ContainsKey(DPlan.ParcelUnitPropName)) || (string.IsNullOrEmpty(f.Properties[DPlan.ParcelUnitPropName]));
            }
            else
            {
                return (f.Properties.ContainsKey(DPlan.ParcelUnitPropName)) && (f.Properties[DPlan.ParcelUnitPropName] == unit);
            }
            //return (unit == "全部地块") || ((!f.Properties.ContainsKey(DPlan.ParcelUnitPropName)) && unit == "未归组地块") || (f.Properties.ContainsKey(DPlan.ParcelUnitPropName) && f.Properties[DPlan.ParcelUnitPropName] == unit);
        }

        public static double Far(this IFeature f)
        {
            return f.Properties["容积率"].TryParseToDouble();
        }

        public static double Bd(this IFeature f)
        {
            return f.Properties["建筑密度"].TryParseToDouble();
        }

        public static double Gr(this IFeature f)
        {
            return f.Properties["绿地率"].TryParseToDouble();
        }

        public static double Hl(this IFeature f)
        {
            return f.Properties["建筑限高"].TryParseToDouble();
        }

        public static int Parks(this IFeature f)
        {
            return f.Properties["机动车停车位"].TryParseToInt32();
        }

        public static string Remark(this IFeature f)
        {
            return f.Properties["备注"];
        }

        public static string Unit(this IFeature f)
        {
            if (f.Properties.ContainsKey(DPlan.ParcelUnitPropName))
            {
                return string.IsNullOrEmpty(f.Properties[DPlan.ParcelUnitPropName]) ? "未归组地块" : f.Properties[DPlan.ParcelUnitPropName];
            }
            else
            {
                return "未归组地块";
            }
        }

        public static string Name(this IFeature f)
        {
            string name = f.Properties["地块编码"];
            if (string.IsNullOrEmpty(name))
            {
                return f.FeatId;
            }
            else
            {
                return name;
            }
        }

        public static string Type(this IFeature f)
        {
            return f.Properties["用地代码"];
        }

        // newly 20120619 精确到0.01公顷
        public static double Area(this IFeature f)
        {
            return Math.Round(f.Area / 100) * 100;
        }

        public static string AdditionalValue(this IFeature f, string key)
        {
            var values = f.Properties["dplan"].Split('&').Select(x => x.Split('=')).ToArray();
            if (values.Any(x => x[0] == key))
            {
                return values.First(x => x[0] == key)[1];
            }
            else
            {
                return string.Empty;
            }
        }

        public static Constraint AreaOfType(string unit, ParcelUsageType type)
        {
            return (plan, dynamic, messages) =>
            {
                var plans = plan.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(unit) && x.IsType(type));
                var dynamics = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(unit) && x.IsType(type));
                double plansArea = plans.Sum(x => x.Area);
                double dynamicsArea = dynamics.Sum(x => x.Area);
                double deltaRatio = Math.Abs(dynamicsArea - plansArea) / plansArea * 100;
                if (type.TypeCode.StartsWith("G"))
                {
                    return true;
                    //if (dynamicsArea >= plansArea)
                    //{
                    //    return true;
                    //}
                    //else
                    //{
                    //    messages.Add(string.Format("单元：{0}|绿地面积缩水|规划{1:0.##}/试图{2:0.00}/偏差{3:0.00}%", unit, plansArea, dynamicsArea, deltaRatio));
                    //    return false;
                    //}
                }
                else
                {
                    if (dynamicsArea <= plansArea)
                    {
                        return true;
                    }
                    else
                    {
                        messages.Add(string.Format("单元：{0}|{1}类用地面积超标|规划{2:0.##}/试图{3:0.00}/偏差{4:0.00}%", unit, type, plansArea, dynamicsArea, deltaRatio));
                        return false;
                    }
                }
            };
        }

        public static Constraint FarOfType(string unit, ParcelUsageType type)
        {
            return (plan, dynamic, messages) =>
            {
                if (type.TypeCode.StartsWith("G"))
                {
                    return true;
                }
                else
                {
                    var plans = plan.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(unit) && x.IsType(type));
                    var dynamics = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(unit) && x.IsType(type));
                    double plansFar = plans.Sum(x => x.Area * x.Far());
                    double dynamicsFar = dynamics.Sum(x => x.Area * x.Far());
                    double deltaRatio = Math.Abs(dynamicsFar - plansFar) / plansFar * 100;
                    if (dynamicsFar <= plansFar)
                    {
                        return true;
                    }
                    else
                    {
                        messages.Add(string.Format("单元：{0}|{1}类开发强度超标|规划{2:0.##}/试图{3:0.00}/偏差{4:0.00}%", unit, type, plansFar, dynamicsFar, deltaRatio));
                        return false;
                    }
                }
            };
        }

        public static Constraint BdOfType(string unit, ParcelUsageType type)
        {
            return (plan, dynamic, messages) =>
            {
                if (type.TypeCode.StartsWith("G"))
                {
                    return true;
                }
                else
                {
                    var plans = plan.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(unit) && x.IsType(type));
                    var dynamics = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(unit) && x.IsType(type));
                    double plansBd = plans.Sum(x => x.Area * x.Bd());
                    double dynamicsBd = dynamics.Sum(x => x.Area * x.Bd());
                    double deltaRatio = Math.Abs(dynamicsBd - plansBd) / plansBd * 100;
                    if (dynamicsBd <= plansBd)
                    {
                        return true;
                    }
                    else
                    {
                        messages.Add(string.Format("单元：{0}|{1}类平均建筑密度超标|规划{2:0.##}/试图{3:0.00}/偏差{4:0.00}%", unit, type, plansBd, dynamicsBd, deltaRatio));
                        return false;
                    }
                }
            };
        }

        public static Constraint TotalFar(string unit)
        {
            return (plan, dynamic, messages) =>
            {
                bool result = true;
                var plans = plan.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(unit));
                var dynamics = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(unit));
                double plansFar = plans.Sum(x => x.Area * x.Far());
                double dynamicsFar = dynamics.Sum(x => x.Area * x.Far());
                double deltaRatio = Math.Abs(dynamicsFar - plansFar) / plansFar * 100;
                if (dynamicsFar <= plansFar)
                {

                }
                else
                {
                    messages.Add(string.Format("单元：{0}|整体开发强度超标|规划{1:0.##}/试图{2:0.00}/偏差{3:0.00}%", unit, plansFar, dynamicsFar, deltaRatio));
                    result = false;
                }
                return result;
            };
        }

        public static Constraint FarOfParcel(string unit)
        {
            return (plan, dynamic, messages) =>
            {
                var dynamics = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(unit));
                bool result = true;
                foreach (var parcel in dynamics)
                {
                    double min = 0;
                    double max = 4;
                    if (parcel.IsIndustrial())
                    {
                        min = 0.6;
                        max = 3;
                    }
                    if (parcel.Far() >= min && parcel.Far() <= max)
                    {
                    }
                    else
                    {
                        messages.Add(string.Format("单元：{0}^地块：{1}|容积率超出容许范围|容许{2:0.##}~{3:0.##}/试图{4:0.00}", unit, parcel.Name(), min, max, parcel.Far()));
                        //messages.Add(string.Format("地块：{0}|容积率超出容许范围|容许{1:0.##}~{2:0.##}/试图{3:0.00}", parcel.Name(), min, max, parcel.Far()));
                        result = false;
                    }
                }
                return result;
            };
        }

        public static Constraint Immutable(string unit)
        {
            return (plan, dynamic, messages) =>
            {
                var parcels = plan.Layers[DPlan.ParcelLayerName].Features.Where(x => x.AdditionalValue("Immutable") == "true").ToList();
                var dynamics = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(unit));
                bool result = true;
                foreach (var parcel in parcels)
                {
                    if (dynamics.Any(x => x.AdditionalValue("SameTo") == parcel.Name()))
                    {
                        var parcel1 = dynamics.First(x => x.AdditionalValue("SameTo") == parcel.Name());
                        if (Math.Abs(parcel.Area - parcel1.Area) < 0.001)
                        {
                            if (parcel.Far() != parcel1.Far())
                            {
                                result = false;
                                messages.Add(string.Format("地块{0}不可调整，但在新图中的对应地块其容积率已被改变。", parcel.Name()));
                            }
                            if (parcel.Bd() != parcel1.Bd())
                            {
                                result = false;
                                messages.Add(string.Format("地块{0}不可调整，但在新图中的对应地块其建筑密度已被改变。", parcel.Name()));
                            }
                            if (parcel.Gr() != parcel1.Gr())
                            {
                                result = false;
                                messages.Add(string.Format("地块{0}不可调整，但在新图中的对应地块其绿地率已被改变。", parcel.Name()));
                            }
                            if (parcel.Type() != parcel1.Type())
                            {
                                result = false;
                                messages.Add(string.Format("地块{0}不可调整，但在新图中的对应地块其用地性质已被改变。", parcel.Name()));
                            }
                        }
                        else
                        {
                            result = false;
                            messages.Add(string.Format("地块{0}不可调整，但在新图中声称与之对应的地块其形状或位置已被改变。", parcel.Name()));
                        }
                    }
                    else
                    {
                        result = false;
                        messages.Add(string.Format("地块{0}不可调整，但在新图中找不到对应，这意味着它可能已被调整。", parcel.Name()));
                    }
                }
                return result;
            };
        }

        //public static Constraint UnitBasic(string unit) // newly 第六轮
        //{

        //}

        public static Constraint ControlLine(string unit = null) // newly 第六轮
        {
            return (plan, dynamic, messages) =>
            {
                if (!plan.Layers.Any(x => x.Name == DPlan.ControlLineLayerName))
                {
                    return true;
                }
                bool result = true;
                var dynamics = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x["地块编码"] != "" && (!x["用地代码"].StartsWith("G")));
                if (unit != null)
                {
                    dynamics = dynamics.Where(x => x.IsInUnit(unit)).ToList();
                }

                var clPolys = plan.Layers[DPlan.ControlLineLayerName].Features.Select(f => new TongJi.Geometry.Polygon(f.GeoData)).ToList();
                foreach (var parcel in dynamics)
                {
                    var parcelPoly = new TongJi.Geometry.Polygon(parcel.GeoData);

                    foreach (var clPoly in clPolys)
                    {
                        //if (parcelPoly.Points.Any(p => clPoly.IsPointIn(p)) && parcelPoly.Points.Any(p => !clPoly.IsPointIn(p)))
                        //{

                        //List<List<clipper.IntPoint>> list = new List<List<IntPoint>>();
                        //list.Add(parcelPoly.Points.Select(y => new clipper.IntPoint((long)y.x * 100, (long)y.y * 100)).ToList());
                        //list.Add(clPoly.Points.Select(y => new clipper.IntPoint((long)y.x * 100, (long)y.y * 100)).ToList());
                        //var polys = clipper.Clipper.OffsetPolygons(list, 0);

                        //clipper.Clipper c = new clipper.Clipper();
                        //c.AddPolygons(polys, clipper.PolyType.ptSubject);

                        //var pgs = new List<List<clipper.IntPoint>>();
                        //c.Execute(clipper.ClipType.ctUnion, pgs, clipper.PolyFillType.pftNonZero, clipper.PolyFillType.pftNonZero);

                        //var area = pgs.Sum(x =>
                        //{
                        //    var pts = new List<TongJi.Geometry.Point2D>();
                        //    foreach (var xx in x)
                        //    {
                        //        pts.Add(new TongJi.Geometry.Point2D(xx.X/100, xx.Y/100));
                        //    }
                        //    var pg = new TongJi.Geometry.Polygon(pts);
                        //    return pg.Area;
                        //});

                        //var area1 = parcelPoly.Area + clPoly.Area;

                        //if (area1 - area > 800)
                        //{
                        //    result = false;
                        //    messages.Add(string.Format("地块{0}不符合要求，突破了控制线。", parcel.Name()));
                        //    break;
                        //}
                        //}
                    }
                }
                return result;
            };
        }

        public static Constraint UnitPublicFacility(string unit = null)
        {
            return (plan, dynamic, messages) =>
            {
                bool result = true;

                if (plan.Layers[DPlan.UnitLayerName] != null)
                {
                    var plans = plan.Layers[DPlan.UnitLayerName].Features.Where(x => x["主导用地代码"].StartsWith("R"));
                    if (unit != null)
                    {
                        plans = plans.Where(x => x["名称"] == unit).ToList();
                    }
                    foreach (var d in plans)
                    {
                        List<string> list = new List<string>();
                        foreach (var f in PublicFacilities.BlockAllTypes.Keys)
                        {
                            var facilities = d["配套设施"].Split(',');
                            if (!facilities.Any(x => x.Contains(f)))
                            {
                                result = false;
                                list.Add(f);
                            }
                        }
                        if (list.Count() > 0)
                        {
                            messages.Add(string.Format("单元：{0}|规划设计公共服务设施不完善|缺失:{1}", d["名称"], string.Join(",", list)));
                        }
                    }
                }
                return result;
            };
        }

        public static Constraint PublicFacility(string unit = null)
        {
            return (plan, dynamic, messages) =>
            {
                bool result = true;
                if (plan.Layers[DPlan.UnitLayerName] != null)
                {
                    var units = plan.Layers[DPlan.UnitLayerName].Features;
                    if (unit != null)
                    {
                        units = units.Where(x => x["名称"] == unit).ToList();
                    }

                    foreach (var u in units)
                    {
                        var planfacility = u["配套设施"].Split(',');
                        var facility = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(u["名称"])).Select(x => x["配套设施"]);
                        List<string> list = new List<string>();
                        foreach (var p in planfacility)
                        {
                            if (!facility.Any(x => x.Contains(p)))
                            {
                                result = false;
                                list.Add(p);
                            }
                        }
                        if (list.Count() > 0)
                        {
                            messages.Add(string.Format("单元：{0}|公共服务设施建设不完善|缺失:{1}", u["名称"], string.Join(",", list)));
                        }
                    }
                }
                return result;
            };
        }

        public static Constraint ParcelArea(string unit = null) //现只有工厂用地
        {
            return (plan, dynamic, messages) =>
                {
                    double standard = 120000;
                    bool result = true;
                    var dynamics = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x["用地代码"].StartsWith("M"));
                    if (unit != null)
                    {
                        dynamics = dynamics.Where(x => x.IsInUnit(unit)).ToList();
                    }
                    foreach (var d in dynamics)
                    {
                        if (d.Area > standard)
                        {
                            result = false;
                            messages.Add(string.Format("地块{0}不符合规模要求|最大容许{1:0.##}/试图{2:0.00}", d.Name(), standard, d.Area));
                        }
                    }
                    return result;
                };
        }

        public static Constraint ParcelGreenRate(string unit = null)
        {
            return (plan, dynamic, messages) =>
            {
                bool result = true;
                var dynamics = dynamic.Layers[DPlan.ParcelLayerName].Features;
                if (unit != null)
                {
                    dynamics = dynamics.Where(x => x.IsInUnit(unit)).ToList();
                }
                foreach (var d in dynamics)
                {
                    double standard = 0;
                    if (d["用地代码"].StartsWith("A5"))
                    {
                        standard = 30;
                    }
                    else if (d["用地代码"].StartsWith("R"))
                    {
                        standard = 30;
                    }
                    if (d.Gr() < standard)
                    {
                        result = false;
                        messages.Add(string.Format("地块{0}不符合绿地率要求|小于{1:0.##}/试图{2:0.00}", d.Name(), standard, d.Gr()));
                    }
                }
                return result;
            };
        }

        public static Constraint UnitUse(string unit = null)
        {
            return (plan, dynamic, messages) =>
            {
                bool result = true;
                if (plan.Layers[DPlan.UnitLayerName] != null)
                {
                    var units = plan.Layers[DPlan.UnitLayerName].Features;
                    if (unit != null)
                    {
                        units = units.Where(x => x["名称"] == unit).ToList();
                    }
                    foreach (var u in units)
                    {
                        var use = u["主导用地代码"].First().ToString();
                        var parcels = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(u["名称"]));
                        var mainarea = parcels.Where(x => x["用地代码"].StartsWith(use)).Sum(x => x.Area);
                        var allarea = parcels.Sum(x => x.Area);
                        //var percent = mainarea / Convert.ToDouble(u["净用地面积"]);
                        var percent = mainarea / allarea;
                        if (percent < 0.35)
                        {
                            messages.Add(string.Format("单元：{0}|主导用地代码发生改变|主导用地{1}类用地面积所占百分比仅为{2}", u["名称"], use, string.Format("{0}%", (percent * 100).ToString("0.00"))));
                            result = false;
                        }
                    }
                }
                return result;
            };
        }

        public static Constraint BuildingHeight(string unit = null)
        {
            return (plan, dynamic, messages) =>
            {
                bool result = true;
                if (plan.Layers[DPlan.UnitLayerName] != null)
                {
                    var units = plan.Layers[DPlan.UnitLayerName].Features;
                    if (unit != null)
                    {
                        units = units.Where(x => x["名称"] == unit).ToList();
                    }
                    foreach (var u in units)
                    {
                        var height = Convert.ToDouble(u["建筑限高"]);
                        var parcels = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(u["名称"]));

                        foreach (var p in parcels)
                        {
                            if (Convert.ToDouble(p["建筑限高"]) > height)
                            {
                                result = false;
                                messages.Add(string.Format("地块{0}不符合建筑限高要求|最大容许{1}米/试图{2}米", p.Name(), height, p["建筑限高"]));
                            }
                        }
                    }
                }
                return result;
            };
        }

        public static Constraint UnitLandArea(string unit = null)
        {
            return (plan, dynamic, messages) =>
            {
                bool result = true;
                if (plan.Layers[DPlan.UnitLayerName] != null)
                {
                    var units = plan.Layers[DPlan.UnitLayerName].Features;
                    if (unit != null)
                    {
                        units = units.Where(x => x["名称"] == unit).ToList();
                    }
                    foreach (var u in units)
                    {
                        var area = Convert.ToDouble(u["净用地面积"]);
                        var unitarea = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(u["名称"])).Sum(x => x.Area);
                        if (area < unitarea)
                        {
                            result = false;
                            messages.Add(string.Format("单元：{0}|用地面积超标|规划净用地面积{1}/试图{2}", u["名称"], area.ToString("0.00"), unitarea.ToString("0.00")));
                        }
                    }
                }
                return result;
            };
        }

        public static Constraint UnitFAR(string unit = null)
        {
            return (plan, dynamic, messages) =>
            {
                bool result = true;
                if (plan.Layers[DPlan.UnitLayerName] != null)
                {
                    var units = plan.Layers[DPlan.UnitLayerName].Features;
                    if (unit != null)
                    {
                        units = units.Where(x => x["名称"] == unit).ToList();
                    }
                    foreach (var u in units)
                    {
                        var unitFAR = Convert.ToDouble(u["平均净容积率"]);
                        var unitarea = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(u["名称"])).Sum(x => x.Area);
                        var buildingarea = dynamic.Layers[DPlan.ParcelLayerName].Features.Where(x => x.IsInUnit(u["名称"])).Sum(x => x.Far() * x.Area);
                        var FAR = buildingarea / unitarea;

                        if (unitFAR < FAR)
                        {
                            result = false;
                            messages.Add(string.Format("单元：{0}|开发强度超标|规划平均净容积率{1}/试图{2}", u["名称"], unitFAR.ToString("0.00"), FAR.ToString("0.00")));
                        }
                    }
                }
                return result;
            };
        }

        public static List<Constraint> BuildBasicConstraintList(string unit, bool useNewGB)
        {
            List<Constraint> result = new List<Constraint>();
            foreach (var type in PlanManager.GetParcelTypeGroups(useNewGB))
            {
                //result.Add(ConstraintHelper.AreaOfType(unit, new ParcelUsageType(useNewGB, type.Key)));
                result.Add(ConstraintHelper.FarOfType(unit, new ParcelUsageType(useNewGB, type.Key)));
                //result.Add(ConstraintHelper.BdOfType(unit, new ParcelUsageType(useNewGB, type.Key)));
            }
            result.Add(ConstraintHelper.TotalFar(unit));
            result.Add(ConstraintHelper.FarOfParcel(unit));
            result.Add(ConstraintHelper.ControlLine(unit));
            //result.Add(ConstraintHelper.UnitPublicFacility(unit));
            result.Add(ConstraintHelper.PublicFacility(unit));
            result.Add(ConstraintHelper.ParcelArea(unit));
            result.Add(ConstraintHelper.UnitUse(unit));
            result.Add(ConstraintHelper.BuildingHeight(unit));
            result.Add(ConstraintHelper.UnitLandArea(unit));
            result.Add(ConstraintHelper.UnitFAR(unit));
            result.Add(ConstraintHelper.ParcelGreenRate(unit));
            return result;
        }

        public static List<Constraint> BuildPublicFacilityConstraintList(string unit)
        {
            List<Constraint> result = new List<Constraint>();
            //result.Add(ConstraintHelper.UnitPublicFacility(unit));
            result.Add(ConstraintHelper.PublicFacility(unit));
            return result;
        }

        public static List<Constraint> BuildCtrlLineConstraintList(string unit)
        {
            List<Constraint> result = new List<Constraint>();
            result.Add(ConstraintHelper.ControlLine(unit));
            return result;
        }

        public static List<Constraint> BuildAreaConstraintList(string unit, bool useNewGB)
        {
            List<Constraint> result = new List<Constraint>();
            foreach (var type in PlanManager.GetParcelTypeGroups(useNewGB))
            {
                result.Add(ConstraintHelper.FarOfType(unit, new ParcelUsageType(useNewGB, type.Key)));
            }
            result.Add(ConstraintHelper.ParcelArea(unit));
            result.Add(ConstraintHelper.ParcelGreenRate(unit));
            result.Add(ConstraintHelper.TotalFar(unit));
            result.Add(ConstraintHelper.FarOfParcel(unit));
            result.Add(ConstraintHelper.BuildingHeight(unit));
            return result;
        }

        public static List<Constraint> BuildMainUseConstraintList(string unit)
        {
            List<Constraint> result = new List<Constraint>();
            result.Add(ConstraintHelper.UnitUse(unit));
            result.Add(ConstraintHelper.UnitLandArea(unit));
            result.Add(ConstraintHelper.UnitFAR(unit));
            return result;
        }
        // 这个函数是没有用的。仅仅在这里用新用地代码，IsType()函数却基于旧用地。
        //public static List<Constraint> BuildBasicConstraintList2011(string unit)
        //{
        //    List<Constraint> result = new List<Constraint>();
        //    foreach (var type in ParcelTypes2011.AllTypes)
        //    {
        //        result.Add(ConstraintHelper.AreaOfType(unit, type.Key));
        //        result.Add(ConstraintHelper.FarOfType(unit, type.Key));
        //        result.Add(ConstraintHelper.BdOfType(unit, type.Key));
        //    }
        //    result.Add(ConstraintHelper.TotalFar(unit));
        //    result.Add(ConstraintHelper.FarOfParcel(unit));
        //    return result;
        //}
    }

    /// <summary>
    /// 封装一些地块类型的判别方法。
    /// </summary>
    public static class ParcelTypes
    {
        private static Dictionary<string, Func<string, bool>> _allTypes = new Dictionary<string, Func<string, bool>> 
        {
            {"R",      ParcelTypes.Resident},
            {"C2",     ParcelTypes.Commercial},
            {"C1",     ParcelTypes.Office},
            {"C3+C4",  ParcelTypes.CulturalAndEntertainment},
            {"C5",     ParcelTypes.Medical},
            {"C6",     ParcelTypes.Education},
            {"M",      ParcelTypes.Industrial},
            {"W",      ParcelTypes.Depository},
            {"T",      ParcelTypes.Transportation},
            {"S",      ParcelTypes.RoadAndSquare},
            {"U",      ParcelTypes.Infrastructure},
            {"G",      ParcelTypes.Green},
            {"D",      ParcelTypes.Special},
            {"E",      ParcelTypes.Agricultural}
            //{"总计",    ParcelTypes.Sum},
        };
        public static Dictionary<string, Func<string, bool>> AllTypes { get { return _allTypes; } }

        public static Dictionary<string, string> TypeColors
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (var type in _allTypes)
                {
                    result[type.Key] = PlanManager.ParcelColor[type.Key].Remove(1, 2);
                }
                return result;
            }
        }

        public static Func<IFeature, bool> TypeRule(string typeCode)
        {
            return parcel => parcel.Properties["用地代码"].StartsWith(typeCode);
        }

        public static bool Resident(string code)
        {
            return code.StartsWith("R");
        }

        public static bool Commercial(string code)
        {
            return code.StartsWith("C2");
        }

        public static bool Office(string code)
        {
            return code.StartsWith("C1");
        }

        public static bool CulturalAndEntertainment(string code)
        {
            return code.StartsWith("C3") || code.StartsWith("C4");
        }

        public static bool Medical(string code)
        {
            return code.StartsWith("C5");
        }

        public static bool Education(string code)
        {
            return code.StartsWith("C6");
        }

        public static bool Industrial(string code)
        {
            return code.StartsWith("M");
        }

        public static bool Depository(string code)
        {
            return code.StartsWith("W");
        }

        public static bool Transportation(string code)
        {
            return code.StartsWith("T");
        }

        public static bool RoadAndSquare(string code)
        {
            return code.StartsWith("S");
        }

        public static bool Infrastructure(string code)
        {
            return code.StartsWith("U");
        }

        public static bool Green(string code)
        {
            return code.StartsWith("G");
        }

        public static bool Special(string code)
        {
            return code.StartsWith("D");
        }

        public static bool Agricultural(string code)
        {
            return code.StartsWith("E");
        }

        public static bool Sum(string code)
        {
            return true;
        }
    }

    public static class ParcelTypes2011
    {
        private static Dictionary<string, Func<string, bool>> _allTypes = new Dictionary<string, Func<string, bool>> 
        {
            {"R",           ParcelTypes2011.Resident},
            {"B",           ParcelTypes2011.Commercial},
            {"A1+A6+A8+A9", ParcelTypes2011.Office},            
            {"A2+A4+A7",    ParcelTypes2011.CulturalAndEntertainment},
            {"A5",          ParcelTypes2011.Medical},
            {"A3",          ParcelTypes2011.Education},
            {"M",           ParcelTypes2011.Industrial},
            {"W",           ParcelTypes2011.Depository},
            {"S",           ParcelTypes2011.CityTransit},
            {"U",           ParcelTypes2011.Infrastructure},
            {"G",           ParcelTypes2011.GreenAndSquare},
            {"H",           ParcelTypes2011.Construction},
            {"E",           ParcelTypes2011.Agricultural}
            //{"总计",    ParcelTypes.Sum},
        };
        public static Dictionary<string, Func<string, bool>> AllTypes { get { return _allTypes; } }

        public static Dictionary<string, string> TypeColors
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (var type in _allTypes)
                {
                    result[type.Key] = PlanManager.ParcelColor2011[type.Key].Remove(1, 2);
                }
                return result;
            }
        }

        public static Func<IFeature, bool> TypeRule(string typeCode)
        {
            return parcel => parcel.Properties["用地代码"].StartsWith(typeCode);
        }

        public static bool Resident(string code)
        {
            return code.StartsWith("R");
        }

        public static bool Commercial(string code)
        {
            return code.StartsWith("B");
        }

        public static bool Office(string code)
        {
            return code.StartsWith("A1") || code.StartsWith("A6") || code.StartsWith("A8") || code.StartsWith("A9");
        }

        public static bool CulturalAndEntertainment(string code)
        {
            return code.StartsWith("A2") || code.StartsWith("A4") || code.StartsWith("A7");
        }

        public static bool Medical(string code)
        {
            return code.StartsWith("A5");
        }

        public static bool Education(string code)
        {
            return code.StartsWith("A3");
        }

        public static bool Industrial(string code)
        {
            return code.StartsWith("M");
        }

        public static bool Depository(string code)
        {
            return code.StartsWith("W");
        }

        public static bool CityTransit(string code)
        {
            return code.StartsWith("S");
        }

        public static bool Infrastructure(string code)
        {
            return code.StartsWith("U");
        }

        public static bool GreenAndSquare(string code)
        {
            return code.StartsWith("G");
        }

        public static bool Construction(string code)
        {
            return code.StartsWith("H");
        }

        public static bool Agricultural(string code)
        {
            return code.StartsWith("E");
        }

        public static bool Sum(string code)
        {
            return true;
        }
    }

    public static class PublicFacilities
    {
        public static Dictionary<string, int> PlanRegionAllTypes = new Dictionary<string, int>
        {
            {"社区卫生服务中心",1},
            {"综合文化活动中心",1},
            {"文化广场",1},
            {"社区居民健身活动中心",1},
            {"运动场",1},
            {"小学",1},
            {"初中",1},
            {"高中",1},
            {"老年人服务中心",1},
            {"街道办事处",1},
            {"街道综合服务中心",1},
            {"派出所",1},
            {"垃圾收集站",1},
            {"避灾点（200人）",1},
            {"社区商业中心",1}
        };

        public static Dictionary<string, int> BlockAllTypes = new Dictionary<string, int>
        {
            {"社区综合服务基础设施",1},
            {"幼儿园",1},
            {"公共厕所",1},
            {"三合一环卫设施",1},
            {"垃圾转运站",1},
            {"生活垃圾收集点",1},
            {"避灾点（100人）",1},
            {"居民停车场",1},
            {"居民存车处",1},
            {"开闭所",1},
            {"变电站",1},
            {"邻里商业",1}
        };
    }
}

#endif