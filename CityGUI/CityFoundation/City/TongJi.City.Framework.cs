using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TongJi.Geometry;

namespace TongJi.City
{
    public class PropList
    {
        public static readonly string[] CityRoad = { "道路等级" };
        public static readonly string[] CityParcel = { "用地代码", "用地性质", "容积率", "建筑密度", "建筑限高", "绿地率", "人口" };
    }

    public class CityBase
    {
        private string _name = string.Empty;
        public string Name { get { return _name; } set { _name = value; } }

        protected PropertyDictionary _properties = new PropertyDictionary();
        public PropertyDictionary Properties { get { return _properties; } }

        public virtual XElement GetPropertiesXElement()
        {
            XElement xe = new XElement("Properties");
            foreach (var prop in _properties.AllEntries)
            {
                xe.Add(new XAttribute(prop, _properties[prop].ToString()));
            }
            return xe;
        }

        public virtual void SetPropertiesByXElement(XElement xprop)
        {
            if (xprop.Name == "Properties")
            {
                if (xprop.Attributes().Count() > 0)
                {
                    foreach (var attribute in xprop.Attributes())
                    {
                        this.Properties[attribute.Name.ToString()] = attribute.Value;
                    }
                }
                else // 遗留问题处理
                {
                    if (this is CityRoad)
                    {
                        PropList.CityRoad.ToList().ForEach(x => this.Properties[x] = string.Empty);
                    }
                    else if (this is CityParcel)
                    {
                        PropList.CityParcel.ToList().ForEach(x => this.Properties[x] = string.Empty);
                    }
                }
            }
        }
    }

    public class CityDistrict : CityBase
    {
        public Extent2D Extents { get; set; }
        public double BasePrice { get; set; }

        public List<CityRoad> Roads { get; protected set; }
        public List<CityParcel> Parcels { get; protected set; }
        public List<SpotEntity> CitySpots { get; protected set; }
        public List<LinearEntity> CityLinears { get; protected set; }
        public List<RegionEntity> CityRegions { get; protected set; }

        public static Extent2D DefaultSize { get { return new Extent2D(0, 0, 5000, 5000); } }

        public const string CurrentCmlVersion = "2.0";

        public CityDistrict()
        {
            Extents = new Extent2D("0,0|1000,1000");
            Roads = new List<CityRoad>();
            Parcels = new List<CityParcel>();
            CitySpots = new List<SpotEntity>();
            CityLinears = new List<LinearEntity>();
            CityRegions = new List<RegionEntity>();
        }

        public virtual void AppendEntity(CityEntity ent)
        {
        }

        public void SaveCml(string path)
        {
            XElement xroads = new XElement("Roads");
            XElement xparcels = new XElement("Parcels");
            XElement xspots = new XElement("CitySpots");
            XElement xlinears = new XElement("CityLinears");
            XElement xregions = new XElement("CityRegions");

            XElement xe = new XElement("TongjiCity", xroads, xparcels, xspots, xlinears, xregions);
            xe.Add(new XAttribute("Extents", Extents.ToString()));
            xe.Add(new XAttribute("CmlVersion", CurrentCmlVersion));

            Roads.ForEach(x => xroads.Add(x.ToXElement()));
            Parcels.ForEach(x => xparcels.Add(x.ToXElement()));
            CitySpots.ForEach(x => xspots.Add(x.ToXElement()));
            CityLinears.ForEach(x => xlinears.Add(x.ToXElement()));
            CityRegions.ForEach(x => xregions.Add(x.ToXElement()));

            xe.Save(path);
        }

        public static CityDistrict LoadCml(string path)
        {
            XDocument xd = XDocument.Load(path);
            XElement xcity = xd.Element("TongjiCity");

            XElement xroads = xcity.Element("Roads");
            XElement xparcels = xcity.Element("Parcels");
            XElement xspots = xcity.Element("CitySpots");
            XElement xlinears = xcity.ElementX("CityLinears"); // 向下兼容
            XElement xregions = xcity.ElementX("CityRegions");

            CityDistrict city = new CityDistrict();
            city.Extents = new Extent2D(xcity.AttValue("Extents"));

            xroads.Elements().ToList().ForEach(x => city.Roads.Add(new CityRoad(x)));
            xparcels.Elements().ToList().ForEach(x => city.Parcels.Add(new CityParcel(x)));
            xspots.Elements().ToList().ForEach(x => city.CitySpots.Add(new SpotEntity(x)));
            xlinears.Elements().ToList().ForEach(x => city.CityLinears.Add(new LinearEntity(x)));
            xregions.Elements().ToList().ForEach(x => city.CityRegions.Add(new RegionEntity(x)));

            return city;
        }

        public string GetCmlVersion(string path)
        {
            XDocument xd = XDocument.Load(path);
            XElement xcity = xd.Element("TongjiCity");
            string version = xcity.AttValue("CmlVersion");
            if (version == string.Empty)
            {
                version = "1.0";
            }
            return version;
        }
    }

    [Flags]
    public enum CityEntityType
    {
        None = 0,
        Spot = 0x10,
        Linear = 0x20,
        Region = 0x40,

        Parcel = 0x41,
        Building = 0x42,
        Green = 0x43,
        Water = 0x44,

        Road = 0x21,
        Railway = 0x22,
        Metroline = 0x23,
        River = 0x24,

        Retail = 0x11,
        Transit = 0x12,
        CrossCityTransportation = 0x13,
        Education = 0x14,
        Medical = 0x15,
        Pollution = 0x16,
        ZeroOne = 0x17
    }

    public class CityEntity : CityBase, IFactor
    {
        public CityEntityType EntityType { get; protected set; }

        public bool IsSpot { get { return (EntityType & CityEntityType.Spot) == CityEntityType.Spot; } }
        public bool IsLinear { get { return (EntityType & CityEntityType.Linear) == CityEntityType.Linear; } }
        public bool IsRegion { get { return (EntityType & CityEntityType.Region) == CityEntityType.Region; } }

        public CityEntity()
        {
        }

        public CityEntity(XElement xe)
            : this()
        {
            Name = xe.AttValue("Name");
            SetPropertiesByXElement(xe.ElementX("Properties"));
            if (xe.Attribute("Type") != null)
            {
                EntityType = xe.AttValue("Type").ParseToEnum<CityEntityType>();
            }
        }

        public virtual XElement ToXElement()
        {
            return new XElement("UnknownEntity");
        }

        // IFactor constructs

        protected FactorFormula _formula;

        public double Formula(Point2D p)
        {
            return _formula(p);
        }
    }

    public class SpotEntity : CityEntity
    {
        public Point2D Position { get; set; }
        public virtual double Coefficient { get; set; }// { get { return Convert.ToDouble(this["Coefficient"]); } }
        public virtual double ServingRadius { get; set; }// { get { return Convert.ToDouble(this["ServingRadius"]); } }

        public SpotEntity()
        {
            EntityType = CityEntityType.Spot;
            BuildFormula();
        }

        public SpotEntity(CityEntityType type)
            : this()
        {
            if ((type & CityEntityType.Spot) == CityEntityType.Spot)
            {
                EntityType = type;
            }
        }

        public SpotEntity(XElement xe)
            : base(xe)
        {
            Position = new Point2D(xe.AttValue("Pos"));
            ServingRadius = Convert.ToDouble(xe.AttValue("Rad"));
            Coefficient = Convert.ToDouble(xe.AttValue("Coeffi"));

            BuildFormula();
        }

        public virtual double GetDistTo(Point2D p)
        {
            return this.Position.DistTo(p);
        }

        public override XElement ToXElement()
        {
            XElement xe = new XElement("Spot");
            xe.Add(new XAttribute("Name", Name));
            xe.Add(new XAttribute("Type", EntityType.ToString()));
            xe.Add(new XAttribute("Pos", Position.ToString()));
            xe.Add(new XAttribute("Rad", ServingRadius.ToString()));
            xe.Add(new XAttribute("Coeffi", Coefficient.ToString()));
            xe.Add(this.GetPropertiesXElement());
            return xe;
        }

        // IFactor constructs

        private static Dictionary<CityEntityType, FactorFormulaType> _formulaCfg = new Dictionary<CityEntityType, FactorFormulaType>
        {
            { CityEntityType.Retail, FactorFormulaType.LinearToZeroInRadius },
            { CityEntityType.Transit, FactorFormulaType.LinearToZeroInRadius },
            { CityEntityType.CrossCityTransportation, FactorFormulaType.LinearToZeroInRadius },
            { CityEntityType.Medical, FactorFormulaType.ExponentialAttenution },
            { CityEntityType.Education, FactorFormulaType.LinearToZeroInRadius },
            { CityEntityType.Pollution, FactorFormulaType.LinearToZeroInRadius },
            { CityEntityType.ZeroOne, FactorFormulaType.ZeroOne },
            { CityEntityType.Spot, FactorFormulaType.LinearToZeroInRadius } // 向下兼容
        };

        private void BuildFormula()
        {
            var type = _formulaCfg[EntityType];
            if (type == FactorFormulaType.LinearToZeroInRadius)
            {
                _formula = Formulas.LinearToZeroInRadius(this);
            }
            else if (type == FactorFormulaType.ExponentialAttenution)
            {
                _formula = Formulas.ExponentialAttenuation(this);
            }
            else
            {
                _formula = Formulas.ZeroOne(this);
            }
        }
    }

    public class LinearEntity : CityEntity
    {
        public Polyline Alignment { get; set; }
        public virtual double Coefficient { get; set; }// { get { return Convert.ToDouble(this["Coefficient"]); } }
        public virtual double BufferSize { get; set; }// { get { return Convert.ToDouble(this["BufferSize"]); } }

        public LinearEntity()
        {
            EntityType = CityEntityType.Linear;
            BuildFormula();
        }

        public LinearEntity(XElement xe)
            : base(xe)
        {
            Alignment = new Polyline(xe.AttValue("P"));
            BufferSize = xe.Attribute("Size") == null ? 0 : Convert.ToDouble(xe.AttValue("Size"));
            Coefficient = xe.Attribute("Coeffi") == null ? 0 : Convert.ToDouble(xe.AttValue("Coeffi"));

            BuildFormula();
        }

        public virtual string XElementName()
        {
            return "Linear";
        }

        public override XElement ToXElement()
        {
            XElement xe = new XElement(XElementName());
            xe.Add(new XAttribute("Name", Name));
            xe.Add(new XAttribute("Type", EntityType.ToString()));
            xe.Add(new XAttribute("P", Alignment.ToString()));
            xe.Add(new XAttribute("Size", BufferSize.ToString()));
            xe.Add(new XAttribute("Coeffi", Coefficient.ToString()));
            xe.Add(this.GetPropertiesXElement());
            return xe;
        }

        // IFactor constructs

        private void BuildFormula()
        {
            _formula = Formulas.LinearEntity(this);
        }
    }

    public class RegionEntity : CityEntity
    {
        public Polygon Domain { get; set; }
        public virtual double Coefficient { get; set; }
        public virtual double BufferSize { get; set; }

        public RegionEntity()
        {
            EntityType = CityEntityType.Region;
            BuildFormula();
        }

        public RegionEntity(XElement xe)
            : base(xe)
        {
            Domain = new Polygon(xe.AttValue("P"));
            BuildFormula();
        }

        public virtual string XElementName()
        {
            return "Region";
        }

        public override XElement ToXElement()
        {
            XElement xe = new XElement(XElementName());
            xe.Add(new XAttribute("Name", Name));
            xe.Add(new XAttribute("Type", EntityType.ToString()));
            xe.Add(new XAttribute("P", Domain.ToString()));
            xe.Add(this.GetPropertiesXElement());
            return xe;
        }

        // IFactor constructs

        private void BuildFormula()
        {
            _formula = Formulas.RegionEntity(this);
        }
    }

    public class CityParcel : RegionEntity
    {
        public CityParcel()
        {
            EntityType = CityEntityType.Parcel;
        }

        public CityParcel(XElement xe)
            : base(xe)
        {
        }

        public override string XElementName()
        {
            return "Parcel";
        }
    }

    public class CityRoad : LinearEntity
    {
        public CityRoad()
        {
            EntityType = CityEntityType.Road;
        }

        public CityRoad(XElement xe)
            : base(xe)
        {
        }

        public override string XElementName()
        {
            return "Road";
        }
    }
}