using Dreambuild.Geometry;
using Dreambuild.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    public static class Parameter // todo: persistence
    {
        public static BaseParams Base { get; private set; }
        public static BuildingParams Building { get; private set; }
        public static TripParams Trip { get; set; }
        public static GreenParams Green { get; private set; }
        public static MaintenanceParams Maintenance { get; private set; }

        static Parameter()
        {
            Base = new BaseParams();
            Building = new BuildingParams();
            Trip = new TripParams();
            Green = new GreenParams();
            Maintenance = new MaintenanceParams();
        }

        public static int GetDistIntervalIndex(double distance)
        {
            for (int i = 4; i > 0; i--)
            {
                if (distance > Trip.TripDistanceIntervalLowerLimits[i])
                {
                    return i;
                }
            }
            return 0;
        }

        public static double GetTripMethodProportion(double distance, int method)
        {
            int interval = GetDistIntervalIndex(distance);
            return Trip.TripMethodProportions[interval][method];
        }

        public static Vector Centroid(this IFeature f)
        {
            return new PointString(f.GeoData).Centroid();
        }

        public static double EzDistTo(this Vector p1, Vector p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }
    }

    /// <summary>
    /// 城市气候类型
    /// </summary>
    public enum CityClimateType
    {
        NorthClimate,
        MiddleClimate,
        SouthClimate
    }

    public class BaseParams
    {
        public int CitySelectedIndex = 0;
        public int BaseYearSelectedIndex = 0;
        public EnergySavingRatio Standard;
        public int BaseYear { get; set; }
        public int CoalElectricityRatio { get; set; }
        public int NaturalGasRatio { get; set; }
        public int OtherCleanEnergyRatio { get; set; }
        public double ElectricityProductionFactor { get; set; }
        public double GasProductionFactor { get; set; }
        public Dictionary<string, CityClimateType> CityDict { get; set; }
        public Dictionary<string, CityClimateType> CityDictE { get; set; }
        private string[] BaseYearStrings { set; get; }
        private string[] BaseYearStringsE { set; get; }
        public int LifeCycle { get; set; }

        public BaseParams()
        {
            Standard = new EnergySavingRatio(EnergySavingType.SavingRatioType, 0);
            BaseYear = 2005;
            CoalElectricityRatio = 100;
            NaturalGasRatio = 0;
            OtherCleanEnergyRatio = 0;
            ElectricityProductionFactor = 0.8115;
            GasProductionFactor = 0.20736;
            LifeCycle = 50;

            BaseYearStrings = new string[] { "2005年", "2006年", "2007年", "2008年", "2009年", "2010年", 
                "2011年", "2012年", "2013年", "2014年", "2015年", "2016年", "2017年", "2018年", "2019年", "2020年" };

            BaseYearStringsE = new string[] { "Year 2005", "Year 2006", "Year 2007", "Year 2008", "Year 2009", "Year 2010", 
                "Year 2011", "Year 2012", "Year 2013", "Year 2014", "Year 2015", "Year 2016", "Year 2017", "Year 2018", "Year 2019", "Year 2020" };

            //Optimize: . use XML to update this way
            CityDict = new Dictionary<string, CityClimateType>
            {
                // Todo Fill the data
                { "北京", CityClimateType.NorthClimate },
                { "上海", CityClimateType.MiddleClimate },
                { "广东", CityClimateType.SouthClimate },
                { "天津", CityClimateType.NorthClimate },
                { "重庆", CityClimateType.MiddleClimate },
                { "河北", CityClimateType.NorthClimate },
                { "山西", CityClimateType.NorthClimate },
                { "内蒙", CityClimateType.NorthClimate },
                { "辽宁", CityClimateType.NorthClimate },
                { "吉林", CityClimateType.NorthClimate },
                { "黑龙江", CityClimateType.NorthClimate },
                { "江苏", CityClimateType.MiddleClimate },
                { "浙江", CityClimateType.MiddleClimate },
                { "安徽", CityClimateType.MiddleClimate },
                { "福建", CityClimateType.MiddleClimate },
                { "江西", CityClimateType.MiddleClimate },
                { "山东", CityClimateType.NorthClimate },
                { "河南", CityClimateType.MiddleClimate },
                { "湖北", CityClimateType.MiddleClimate },
                { "湖南", CityClimateType.MiddleClimate },
                { "广西", CityClimateType.SouthClimate },
                { "海南", CityClimateType.SouthClimate },
                { "四川", CityClimateType.MiddleClimate },
                { "贵州", CityClimateType.SouthClimate },
                { "云南", CityClimateType.SouthClimate },
                { "陕西", CityClimateType.NorthClimate },
                { "甘肃", CityClimateType.NorthClimate },
                { "青海", CityClimateType.NorthClimate },
                { "宁夏", CityClimateType.NorthClimate },
                { "新疆", CityClimateType.NorthClimate },
                { "西藏", CityClimateType.NorthClimate },
                { "台湾", CityClimateType.SouthClimate },
                { "香港", CityClimateType.SouthClimate },
                { "澳门", CityClimateType.SouthClimate }
            };

            CityDictE = new Dictionary<string, CityClimateType>
            {
                // Todo Fill the data
                {"Beijing", CityClimateType.NorthClimate},
                {"Shanghai", CityClimateType.MiddleClimate},
                {"Guangdong", CityClimateType.SouthClimate},
                {"Tianjin", CityClimateType.NorthClimate},
                {"Chongqing", CityClimateType.MiddleClimate},
                {"Hebei", CityClimateType.NorthClimate},
                {"Shanxi", CityClimateType.NorthClimate},
                {"Neimeng", CityClimateType.NorthClimate},
                {"Liaoning", CityClimateType.NorthClimate},
                {"Jilin", CityClimateType.NorthClimate},
                {"Heilongjiang", CityClimateType.NorthClimate},
                {"Jiangsu", CityClimateType.MiddleClimate},
                {"Zhejiang", CityClimateType.MiddleClimate},
                {"Anhui", CityClimateType.MiddleClimate},
                {"Fujian", CityClimateType.MiddleClimate},
                {"Jiangxi", CityClimateType.MiddleClimate},
                {"Shandong", CityClimateType.NorthClimate},
                {"Henan", CityClimateType.MiddleClimate},
                {"Hubei", CityClimateType.MiddleClimate},
                {"Hunan", CityClimateType.MiddleClimate},
                {"Guangxi", CityClimateType.SouthClimate},
                {"Hainan", CityClimateType.SouthClimate},
                {"Sichuan", CityClimateType.MiddleClimate},
                {"Guizhou", CityClimateType.SouthClimate},
                {"Yunnan", CityClimateType.SouthClimate},
                {"Shaanxi", CityClimateType.NorthClimate},
                {"Gansu", CityClimateType.NorthClimate},
                {"Qinghai", CityClimateType.NorthClimate},
                {"Ningxia", CityClimateType.NorthClimate},
                {"Xinjiang", CityClimateType.NorthClimate},
                {"Tibet", CityClimateType.NorthClimate},
                {"Taiwan", CityClimateType.SouthClimate},
                {"Hongkong", CityClimateType.SouthClimate},
                {"Macau", CityClimateType.SouthClimate}
            };

        }

        public string CurrentCity
        {
            get
            {
                return CityNames[CitySelectedIndex];
            }
        }

        public List<string> CityNames
        {
            get
            {
                switch (LocalizationHelper.CurrentLocale)
                {
                    case Locales.ZH_CN:
                        return CityDict.Keys.ToList();
                    case Locales.EN_US:
                        return CityDictE.Keys.ToList();
                    default:
                        throw new Exception("Unknown Language!");
                }
            }
        }

        public string[] BaseYears
        {
            get
            {
                switch (LocalizationHelper.CurrentLocale)
                {
                    case Locales.ZH_CN:
                        return BaseYearStrings;
                    case Locales.EN_US:
                        return BaseYearStringsE;
                    default:
                        throw new Exception("Unknown Language!");
                }
            }
        }

        public CityClimateType CityClimate
        {
            get
            {
                CityClimateType result = CityClimateType.MiddleClimate;

                switch (LocalizationHelper.CurrentLocale)
                {
                    case Locales.ZH_CN:
                        result = CityDict[CurrentCity];
                        break;
                    case Locales.EN_US:
                        result = CityDictE[CurrentCity];
                        break;
                    default:
                        throw new Exception("Unknown Language!");
                }

                return result;
            }
        }

        public string CityLocation
        {
            get
            {
                //if (CurrentCity != null)
                //{
                switch (CityClimate)
                {
                    case CityClimateType.NorthClimate:
                        return "北方";
                    case CityClimateType.MiddleClimate:
                        return "中部";
                    case CityClimateType.SouthClimate:
                        return "南方";
                    default:
                        throw new Exception("Unknown CityClimateType!");
                }
                //}
                //throw new Exception("call GetCurrentCityLocation but CurrentCity is null");
            }
        }

    }

    public class BuildingParams
    {
        [Category("建材准备二氧化碳排放"), DisplayName("公共建筑")]
        public double PublicBuildingMaterial { get; set; }
        [Category("建材准备二氧化碳排放"), DisplayName("住宅建筑")]
        public double ResidentialBuildingMaterial { get; set; }

        [Category("施工二氧化碳排放"), DisplayName("钢结构")]
        public double SteelStructureContruction { get; set; }
        [Category("施工二氧化碳排放"), DisplayName("混凝土结构")]
        public double ConcreteStructureContruction { get; set; }
        [Category("施工二氧化碳排放"), DisplayName("木结构")]
        public double TimberStructureContruction { get; set; }

        [Category("拆毁回收二氧化碳排放"), DisplayName("钢结构")]
        public double SteelStructureRecycle { get; set; }
        [Category("拆毁回收二氧化碳排放"), DisplayName("混凝土结构")]
        public double ConcreteStructureRecycle { get; set; }
        [Category("拆毁回收二氧化碳排放"), DisplayName("木结构")]
        public double TimberStructureRecycle { get; set; }

        public BuildingParams()
        {
            PublicBuildingMaterial = 593.6;
            ResidentialBuildingMaterial = 540.6;

            SteelStructureContruction = 0.4;
            ConcreteStructureContruction = 5.5;
            TimberStructureContruction = 0.9;

            SteelStructureRecycle = -2.1;
            ConcreteStructureRecycle = -10.6;
            TimberStructureRecycle = -6.4;
        }
    }

    public static class TripMethod
    {
        public const int WalkAndBicycle = 0;
        public const int Car = 1;
        public const int BusAndMetro = 2;

        public static int[] All
        {
            get
            {
                return new int[] { WalkAndBicycle, Car, BusAndMetro };
            }
        }
    }

    public static class TripDistanceInterval
    {
        public static int[] All
        {
            get
            {
                return Enumerable.Range(0, 5).ToArray();
            }
        }
    }

    public class TripParams
    {
        public double[] TripDistanceIntervalLowerLimits { get; set; }
        public double[] TripDistanceIntervalAverageDistances { get; set; }
        public double[] TripDistanceIntervalProportions { get; set; }
        public double[][] TripMethodProportions { get; set; }
        public double[] TripMethodUnitCarbonProductions { get; set; }
        public double PerCapitaResidencial { get; set; }
        public double PerCapitaOffice { get; set; }
        public double PerCapitaIndustry { get; set; }
        [Range(0, 1)]
        public double AdultRate { get; set; }
        [Range(0, 1)]
        public double StudentRate { get; set; }
        public double TripFrequencyOfTransit { get; set; }
        public double TripFrequencyOfSchool { get; set; }
        public double TripFrequencyOfOtherEvent { get; set; }

        public TripParams()
        {
            TripDistanceIntervalLowerLimits = new double[] { 0, 2, 5, 10, 20 };
            TripDistanceIntervalAverageDistances = new double[] { 1, 3.5, 7.5, 15, 30 };
            TripDistanceIntervalProportions = new double[] { 0.05, 0.20, 0.50, 0.20, 0.05 };
            TripMethodProportions = new double[][] 
            {
                new double[] { 1.00, 0.00, 0.00 },
                new double[] { 0.30, 0.40, 0.30 },
                new double[] { 0.15, 0.45, 0.40 },
                new double[] { 0.05, 0.65, 0.30 },
                new double[] { 0.00, 0.80, 0.20 }
            };
            TripMethodUnitCarbonProductions = new double[] { 0, 0.22, 0.03 };
            PerCapitaResidencial = 30;
            PerCapitaOffice = 30;
            PerCapitaIndustry = 30;
            AdultRate = 0.7;
            StudentRate = 0.2;
            TripFrequencyOfTransit = 480;
            TripFrequencyOfSchool = 480;
            TripFrequencyOfOtherEvent = 240;
        }

        public TripParams(TripParams other)
        {
            TripDistanceIntervalLowerLimits = (double[])other.TripDistanceIntervalLowerLimits.Clone();
            TripDistanceIntervalAverageDistances = (double[])other.TripDistanceIntervalAverageDistances.Clone();
            TripDistanceIntervalProportions = (double[])other.TripDistanceIntervalProportions.Clone();
            TripMethodProportions = (double[][])other.TripMethodProportions.Clone();
            TripMethodUnitCarbonProductions = (double[])other.TripMethodUnitCarbonProductions.Clone();
            PerCapitaIndustry = other.PerCapitaIndustry;
            PerCapitaOffice = other.PerCapitaOffice;
            PerCapitaResidencial = other.PerCapitaResidencial;
            AdultRate = other.AdultRate;
            StudentRate = other.StudentRate;
            TripFrequencyOfTransit = other.TripFrequencyOfTransit;
            TripFrequencyOfSchool = other.TripFrequencyOfSchool;
            TripFrequencyOfOtherEvent = other.TripFrequencyOfOtherEvent;
        }
    }

    public class GreenParams
    {
        [Category("各类型绿地单位面积二氧化碳吸收"), DisplayName("公园")]
        public double ParkUnit { get; set; }
        [Category("各类型绿地单位面积二氧化碳吸收"), DisplayName("森林")]
        public double ForestUnit { get; set; }
        [Category("各类型绿地单位面积二氧化碳吸收"), DisplayName("灌木林")]
        public double BushesUnit { get; set; }
        [Category("各类型绿地单位面积二氧化碳吸收"), DisplayName("农田")]
        public double FarmUnit { get; set; }

        [Category("各类型绿地面积"), DisplayName("公园")]
        public double ParkArea { get; set; }
        [Category("各类型绿地面积"), DisplayName("森林")]
        public double ForestArea { get; set; }
        [Category("各类型绿地面积"), DisplayName("灌木林")]
        public double BushesArea { get; set; }
        [Category("各类型绿地面积"), DisplayName("农田")]
        public double FarmArea { get; set; }

        public GreenParams()
        {
            ParkUnit = 1.147;
            ForestUnit = 0.934;
            BushesUnit = 0.570;
            FarmUnit = 0.835;

            ParkArea = 2500000;
            ForestArea = 200000;
            BushesArea = 100000;
            FarmArea = 100000;
        }
    }
}
