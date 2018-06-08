using Dreambuild.Extensions;
using Dreambuild.Geometry;
using Dreambuild.Properties;
using Dreambuild.Utils;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    public enum ComputationUnitType
    {
        [Description("CUD:Other")]
        Other = 0,
        [Description("CUD:Building")]
        Building = 1,
        [Description("CUD:Parcel")]
        Parcel = 2
    }

    public enum GreenFieldType
    {
        [Description("CUD:NoneGreenField")]
        NoneGreenField = 0,
        [Description("CUD:Park")]
        Park = 1,
        [Description("CUD:Bush")]
        Bush = 2,
        [Description("CUD:Farm")]
        Farm = 3,
        [Description("CUD:Forest")]
        Forest = 4
    }

    [Flags]
    public enum BuildingType
    {
        [Description("CUD:Unknown")]
        Unknown = 0x00,

        [Description("CUD:Office")]
        Office = 0x01,
        [Description("CUD:Residencial")]
        Residencial = 0x02,
        [Description("CUD:Hotel")]
        Hotel = 0x04,
        [Description("CUD:Retail")]
        Retail = 0x08,
        [Description("CUD:Education")]
        Education = 0x10,
        [Description("CUD:GreenField")]
        GreenField = 0x20,
        [Description("CUD:Industry")]
        Industry = 0x40,
        [Description("CUD:Other")]
        Other = 0x80,

        [Browsable(false)]
        MidHigh = 0x100,
        [Browsable(false)]
        High = 0x200,

        [Browsable(false)]
        MidHighOffice = MidHigh | Office,
        [Browsable(false)]
        MidHighResidencial = MidHigh | Residencial,
        [Browsable(false)]
        HighOffice = High | Office,
        [Browsable(false)]
        HighResidencial = High | Residencial,

        [Description("CUD:Mixed")]
        Mixed = 0x400
    }

    public enum StructuralType
    {
        [Description("CUD:Concrete")]
        Concrete,
        [Description("CUD:Steel")]
        Steel,
        [Description("CUD:Timber")]
        Timber
    }

    public class ComputationUnit
    {
        [Browsable(false)]
        public IFeature Feature { get; protected set; }

        [ReadOnly(true), Category("Normal")]
        public int ID { get; set; }

        [DisplayName("CU:Name"), Category("Normal"), ReadOnly(true)]
        public string Name { get; set; }

        [DisplayName("CU:UnitType"), Category("Normal"), TypeConverter(typeof(EnumValueDescriptionConverter)), DefaultValue(ComputationUnitType.Other)]
        public ComputationUnitType UnitType { get; set; }

        [ReadOnly(true), DisplayName("CU:OriginalType"), Category("Normal")]
        public string LayerName { get; set; }

        [DisplayName("CU:Structure"), Category("Parameters"), TypeConverter(typeof(EnumValueDescriptionConverter)), DefaultValue(StructuralType.Concrete)]
        public StructuralType Structure { get; set; }

        [DisplayName("CU:Type"), Category("Parameters"), TypeConverter(typeof(EnumValueDescriptionConverter)), DefaultValue(BuildingType.MidHighOffice)]
        public BuildingType Type { get; set; }

        [DisplayName("CU:MixHotelPercent"), Category("Mixed"), Range(0.0, 1.0), DefaultValue(0.0)]
        public double MixHotelPercent { get; set; }

        [DisplayName("CU:MixRetailPercent"), Category("Mixed"), Range(0.0, 1.0), DefaultValue(0.0)]
        public double MixRetailPercent { get; set; }

        [DisplayName("CU:MixEducationPercent"), Category("Mixed"), Range(0.0, 1.0), DefaultValue(0.0)]
        public double MixEducationPercent { get; set; }

        [DisplayName("CU:MixGreenFieldPercent"), Category("Mixed"), Range(0.0, 1.0), DefaultValue(0.0)]
        public double MixGreenFieldPercent { get; set; }

        [DisplayName("CU:MixIndustryPercent"), Category("Mixed"), Range(0.0, 1.0), DefaultValue(0.0)]
        public double MixIndustryPercent { get; set; }

        [DisplayName("CU:MixResidencialPercent"), Category("Mixed"), Range(0.0, 1.0), DefaultValue(0.0)]
        public double MixResidencialPercent { get; set; }

        [DisplayName("CU:MixOfficePercent"), Category("Mixed"), Range(0.0, 1.0), DefaultValue(0.0)]
        public double MixOfficePercent { get; set; }

        [DisplayName("CU:MixOtherPercent"), Category("Mixed"), Range(0.0, 1.0), DefaultValue(0.0)]
        public double MixOtherPercent { get; set; }

        [DisplayName("CU:Area"), Category("Parameters")]
        public double Area { get; set; }

        [DisplayName("CU:FAR"), Category("Parameters"), DefaultValue(1.0)]
        public double FAR { get; set; }

        [DisplayName("CU:Levels"), Category("Parameters"), DefaultValue(5)]
        public int Levels { get; set; }

        [DisplayName("CU:PerCapitaResidencial"), Category("Parameters"), DefaultValue(30)]
        public double PerCapitaResidencial { get; set; }

        [DisplayName("CU:PerCapitaOffice"), Category("Parameters"), DefaultValue(30)]
        public double PerCapitaOffice { get; set; }

        [DisplayName("CU:PerCapitaIndustry"), Category("Parameters"), DefaultValue(30)]
        public double PerCapitaIndustry { get; set; }

        [DisplayName("CU:Populations"), Category("Parameters"), DefaultValue(100)]
        public double Populations { get; set; }

        [DisplayName("CU:EnergySavingCoefficient"), Category("Parameters"), Range(0.0, 1.0), DefaultValue(0.0), EditorAttribute(typeof(EnergySavingEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public EnergySavingRatio EnergySavingCoefficient { get; set; }

        [DisplayName("CU:Floor"), Category("Parameters")]
        public double Floor
        {
            get
            {
                return Demo.CaculateType == ComputationUnitType.Building ? Area * Levels : Area * FAR;
            }
        }

        [DisplayName("CU:Position"), Category("Normal"), ReadOnly(true)]
        public Vector Position { get; set; }

        [DisplayName("CU:GreenField"), Category("Parameters"), TypeConverter(typeof(EnumValueDescriptionConverter)), DefaultValue(GreenFieldType.NoneGreenField)]
        public GreenFieldType GreenField { get; set; }

        [DisplayName("CU:GreenFieldConsumption"), Category("Result"), ReadOnly(true)]
        public double GreenFieldConsumption { get; set; }

        [ReadOnly(true), DisplayName("CU:CarbonProduction"), Category("Result")]
        public BuildingResult CarbonProduction { get; set; }

        public ComputationUnit()
        {
            this.Feature = null;
            this.UnitType = ComputationUnitType.Other;
            this.Type = BuildingType.MidHighOffice;
            this.MixOfficePercent = 0.0;
            this.MixIndustryPercent = 0.0;
            this.MixEducationPercent = 0.0;
            this.MixHotelPercent = 0.0;
            this.MixGreenFieldPercent = 0.0;
            this.MixOtherPercent = 0.0;
            this.MixRetailPercent = 0.0;
            this.MixResidencialPercent = 0.0;
            this.FAR = 1.0;
            this.Levels = 5;
            this.PerCapitaResidencial = 30;
            this.PerCapitaOffice = 30;
            this.PerCapitaIndustry = 30;
            this.Populations = 100;
            this.EnergySavingCoefficient = new EnergySavingRatio(EnergySavingType.SavingRatioType, 0.0);
            this.GreenField = GreenFieldType.NoneGreenField;
            UpdateResult();
        }

        public ComputationUnit(ComputationUnit other)
        {
            this.Feature = other.Feature;
            this.UnitType = other.UnitType;
            this.Type = other.Type;
            this.MixOfficePercent = other.MixOfficePercent;
            this.MixIndustryPercent = other.MixIndustryPercent;
            this.MixEducationPercent = other.MixEducationPercent;
            this.MixHotelPercent = other.MixHotelPercent;
            this.MixGreenFieldPercent = other.MixGreenFieldPercent;
            this.MixOtherPercent = other.MixOtherPercent;
            this.MixRetailPercent = other.MixRetailPercent;
            this.MixResidencialPercent = other.MixResidencialPercent;
            this.FAR = other.FAR;
            this.Levels = other.Levels;
            this.PerCapitaResidencial = other.PerCapitaResidencial;
            this.PerCapitaOffice = other.PerCapitaOffice;
            this.PerCapitaIndustry = other.PerCapitaIndustry;
            this.Populations = other.Populations;
            this.EnergySavingCoefficient = other.EnergySavingCoefficient;
            this.GreenField = other.GreenField;
            this.Position = other.Position;

            UpdateFeature();
        }

        public ComputationUnit(IFeature f)
        {
            Feature = f;
            //this.UnitType = f["unitType"] == "Parcel" ? ComputationUnitType.Parcel : ComputationUnitType.Building;
            this.LayerName = f["layerName"];

            if (this.LayerName.Contains("YD-")) // 用地图层
            {
                this.UnitType = ComputationUnitType.Parcel;
            }
            else if (this.LayerName.Contains("JZ-")) // 建筑图层
            {
                this.UnitType = ComputationUnitType.Building;
            }
            else   // 其他图层
            {
                this.UnitType = ComputationUnitType.Other;
            }
            f["unitType"] = this.UnitType.ToString();

            this.FAR = f["far"].TryParseToDouble();
            this.Levels = f["levels"].TryParseToInt32();
            this.Type = string.IsNullOrEmpty(f["buildType"]) ? GetDefaultType(this.LayerName, this.FAR, this.Levels) : f["buildType"].ParseToEnum<BuildingType>();
            this.Structure = string.IsNullOrEmpty(f["struct"]) ? StructuralType.Concrete : f["struct"].ParseToEnum<StructuralType>();
            this.MixOfficePercent = f["mixOffic"].TryParseToDouble();
            this.MixIndustryPercent = f["mixIndu"].TryParseToDouble();
            this.MixEducationPercent = f["mixEdu"].TryParseToDouble();
            this.MixHotelPercent = f["mixHotel"].TryParseToDouble();
            this.MixGreenFieldPercent = f["mixGreen"].TryParseToDouble();
            this.MixOtherPercent = f["mixOther"].TryParseToDouble();
            this.MixRetailPercent = f["mixRetai"].TryParseToDouble();
            this.MixResidencialPercent = f["mixResi"].TryParseToDouble();
            this.Area = string.IsNullOrEmpty(f["area"]) ? f.Area() : f["area"].TryParseToDouble();
            this.PerCapitaResidencial = CheckPerCapitaResidencial(f["perResi"].TryParseToDouble());
            this.PerCapitaOffice = CheckPerCapitaOffice(f["perOffic"].TryParseToDouble());
            this.PerCapitaIndustry = CheckPerCapitaIndustry(f["perIndu"].TryParseToDouble());
            this.Populations = IsEmptyPopulation(f["pop"]) ? UpdatePopulation() : f["pop"].TryParseToDouble();
            EnergySavingType tmpType = (EnergySavingType)Enum.Parse(typeof(EnergySavingType), f["escType"]);
            double tmpValue = f["escValue"].TryParseToDouble();
            this.EnergySavingCoefficient = new EnergySavingRatio(tmpType, tmpValue);
            this.GreenField = string.IsNullOrEmpty(f["green"]) ? GetDefaultGreenType(this.Type) : f["green"].ParseToEnum<GreenFieldType>();
            this.Position = new PointString(f.GeoData).Centroid();
            UpdateResult();
        }

        public void UpdateFeature()
        {
            IFeature f = this.Feature;
            f["unitType"] = this.UnitType.ToString();
            f["layerName"] = this.LayerName;
            f["buildType"] = this.Type.ToString();
            f["struct"] = this.Structure.ToString();
            f["mixOffic"] = this.MixOfficePercent.ToString();
            f["mixIndu"] = this.MixIndustryPercent.ToString();
            f["mixEdu"] = this.MixEducationPercent.ToString();
            f["mixHotel"] = this.MixHotelPercent.ToString();
            f["mixGreen"] = this.MixGreenFieldPercent.ToString();
            f["mixOther"] = this.MixOtherPercent.ToString();
            f["mixRetai"] = this.MixRetailPercent.ToString();
            f["mixResi"] = this.MixResidencialPercent.ToString();
            f["area"] = f.Area() == this.Area ? string.Empty : this.Area.ToString("0.##");
            f["far"] = this.FAR.ToString();
            f["levels"] = this.Levels.ToString();
            f["perResi"] = this.PerCapitaResidencial.ToString();
            f["perOffic"] = this.PerCapitaOffice.ToString();
            f["perIndu"] = this.PerCapitaIndustry.ToString();
            f["pop"] = this.Populations.ToString();
            f["escType"] = this.EnergySavingCoefficient.EnergyType.ToString();
            f["escValue"] = this.EnergySavingCoefficient.TempValue.ToString();
            f["green"] = this.GreenField.ToString();
        }

        private double CheckPerCapitaResidencial(double value)
        {
            if (value == 0)
            {
                return Parameter.Trip.PerCapitaResidencial;
            }
            return value;
        }

        private double CheckPerCapitaOffice(double value)
        {
            if (value == 0)
            {
                return Parameter.Trip.PerCapitaOffice;
            }
            return value;
        }

        private double CheckPerCapitaIndustry(double value)
        {
            if (value == 0)
            {
                return Parameter.Trip.PerCapitaIndustry;
            }
            return value;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1:0.00}", this.Type, this.Area);
        }

        public void UpdateResult()
        {
            LcComputation.GetBuildingCarbonProduction(this);
            LcComputation.GetGreenCarbonConsumption(this);
        }

        public double GetPerCapitaLivingSpace(BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.Office:
                    return PerCapitaOffice;
                case BuildingType.Residencial:
                    return PerCapitaResidencial;
                case BuildingType.Industry:
                    return PerCapitaIndustry;
                default:
                    return 0;
            }
        }

        public double UpdatePopulation()
        {
            if (this.Type != BuildingType.Mixed)
            {
                double PerCapitaLivingSpace = GetPerCapitaLivingSpace(this.Type);
                this.Populations = GetDefaultPopulation(this.Floor, PerCapitaLivingSpace, this.Type);
            }
            else
            {
                this.Populations = GetDefaultPopulation(this.Floor * this.MixOfficePercent, this.PerCapitaOffice, BuildingType.Office)
                + GetDefaultPopulation(this.Floor * this.MixResidencialPercent, this.PerCapitaResidencial, BuildingType.Residencial)
                + GetDefaultPopulation(this.Floor * this.MixIndustryPercent, this.PerCapitaIndustry, BuildingType.Industry);
            }

            return this.Populations;
        }

        public void UpdateType()
        {
            this.Type = AdjustDefaultType(this.Type, this.FAR, this.Levels);
        }

        private static bool IsEmptyPopulation(string pop)
        {
            if (string.IsNullOrEmpty(pop) || pop == "0")
            {
                return true;
            }

            return false;
        }

        private static double GetDefaultPopulation(double buildingArea, double perCapitaLivingSpace, BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.Office:
                case BuildingType.Residencial:
                case BuildingType.Industry:
                    if (perCapitaLivingSpace == 0)
                    {
                        throw new Exception("PerCapitaLivingSpace is '0'!");
                    }
                    return Convert.ToInt32(buildingArea / perCapitaLivingSpace);
                default:
                    return 0;
            }
        }

        private static BuildingType GetDefaultType(string typeString, double far, int levels)
        {
            if (typeString.StartsWith("YD-"))
            {
                typeString = typeString.Split('-')[1];
            }
            var type = TypeMapping.Map(typeString);
            //type = AdjustDefaultType(type, far, levels);
            return type;
        }

        private static GreenFieldType GetDefaultGreenType(BuildingType type)
        {
            return type == BuildingType.GreenField ? GreenFieldType.Park : GreenFieldType.NoneGreenField;
        }

        private static BuildingType AdjustDefaultType(BuildingType type, double far, int levels)
        {
            if (type.HasFlag(BuildingType.Residencial) || type.HasFlag(BuildingType.Office))
            {
                type = type | BuildingType.High | BuildingType.MidHigh;
                if (IsHighBuilding(far, levels))
                {
                    type = type ^ BuildingType.MidHigh;
                }
                else
                {
                    type = type ^ BuildingType.High;
                }
            }
            return type;
        }

        private static bool IsHighBuilding(double far, int levels)
        {
            return (far > 2 || levels >= 7);
        }

        public bool IsHighBuilding()
        {
            return IsHighBuilding(this.FAR, this.Levels);
        }

        public bool IsValidMixedValue()
        {
            double sum = MixEducationPercent + MixIndustryPercent + MixGreenFieldPercent + MixHotelPercent +
                         MixResidencialPercent + MixRetailPercent + MixOfficePercent + MixOtherPercent;
            //if (Math.Abs(sum - 1.0) > 0.1)
            //{
            //    throw new Exception("Sum of Mixed value Not equal '1'");
            //}
            return true;
        }

        public const string KEY = "LCCU";

        public static ComputationUnit Get(IFeature f)
        {
            Ensure(f);
            return f.Data[KEY] as ComputationUnit;
        }

        public static void Set(IFeature f, ComputationUnit cu)
        {
            f.Data[KEY] = cu;
        }

        public static void Ensure(IFeature f)
        {
            if (!f.Data.ContainsKey(KEY))
            {
                f.Data[KEY] = new ComputationUnit(f);
            }
        }

        public static void Attach(IFeature f, bool createNew = false)
        {
            if (createNew)
            {
                var cu = new ComputationUnit(f);
                cu.Area = f.Area();
                f.Data[KEY] = cu;
            }
            else
            {
                f.Data[KEY] = new ComputationUnit(f);
            }
        }

        public bool IsOffice()
        {
            if (this.Type.HasFlag(BuildingType.Office) || this.Type == BuildingType.Industry)
            {
                return true;
            }
            return false;
        }

        public bool IsHome()
        {
            return this.Type.HasFlag(BuildingType.Residencial);
        }

        public bool IsSchool()
        {
            return this.Type == BuildingType.Education;
        }

        public bool IsEntertainment()
        {
            return this.Type == BuildingType.Retail || this.Type == BuildingType.GreenField;
        }
    }

    #region Output

    public class ResultSummary
    {
        [DisplayName("RS:Count"), Category("RS:Normal"), ReadOnly(true)]
        public int Count { get; set; }

        [DisplayName("RS:Area"), Category("RS:Normal"), ReadOnly(true)]
        public double Area { get; set; }

        [DisplayName("RS:Green"), Category("RS:Result"), ReadOnly(true)]
        public double Green { get; set; }

        [DisplayName("RS:Building"), Category("RS:Result"), ReadOnly(true)]
        public BuildingResult Building { get; set; }

        [DisplayName("RS:Trip"), Category("RS:Result"), ReadOnly(true)]
        public TripResult Trip { get; set; }
    }

    [TypeConverter(typeof(LocalizedExpandableObjectConverter)), DefaultValue(null)]
    public class BuildingResult
    {
        [DisplayName("CU:Material"), ReadOnly(true)]
        public double Material { get; set; }

        [DisplayName("CU:Construction"), ReadOnly(true)]
        public double Construction { get; set; }

        [DisplayName("CU:Maintenance"), ReadOnly(true)]
        public MaintenanceResult Maintenance { get; set; }

        [DisplayName("CU:Recycle"), ReadOnly(true)]
        public double Recycle { get; set; }

        public BuildingResult()
        {
            Maintenance = new MaintenanceResult();
        }

        [Browsable(false)]
        public double Sum
        {
            get
            {
                return Material + Construction + Maintenance.Sum + Recycle;
            }
        }

        public override string ToString()
        {
            return Sum.ToString();
        }

        public void Clear()
        {
            Material = 0;
            Construction = 0;
            Maintenance.Clear();
            Recycle = 0;
        }
    }

    [TypeConverter(typeof(LocalizedExpandableObjectConverter))]
    public class MaintenanceResult
    {
        [DisplayName("CU:AirConditioning"), ReadOnly(true)]
        public double AirConditioning { get; set; }
        [DisplayName("CU:Equipment"), ReadOnly(true)]
        public double Equipment { get; set; }
        [DisplayName("CU:Heating"), ReadOnly(true)]
        public double Heating { get; set; }
        [DisplayName("CU:Lighting"), ReadOnly(true)]
        public double Lighting { get; set; }

        [Browsable(false)]
        public double Sum
        {
            get
            {
                return AirConditioning + Equipment + Heating + Lighting;
            }
        }

        public override string ToString()
        {
            return Sum.ToString();
        }

        public void Clear()
        {
            AirConditioning = 0;
            Equipment = 0;
            Heating = 0;
            Lighting = 0;
        }
    }

    [TypeConverter(typeof(LocalizedExpandableObjectConverter)), DefaultValue(null)]
    public class TripResult
    {
        [DisplayName("TR:OutWork"), ReadOnly(true)]
        public double OutWork { get; set; }

        [DisplayName("TR:InWork"), ReadOnly(true)]
        public double InWork { get; set; }

        [DisplayName("TR:School"), ReadOnly(true)]
        public double School { get; set; }

        [DisplayName("TR:Other"), ReadOnly(true)]
        public double Other { get; set; }

        [Browsable(false)]
        public double Sum
        {
            get
            {
                return OutWork + InWork + School + Other;
            }
        }

        public override string ToString()
        {
            return Sum.ToString();
        }
    }

    #endregion

}
