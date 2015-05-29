using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dreambuild.Gis
{
    public interface IMap
    {
        LayerCollection Layers { get; }
    }

    public class Map : IMap
    {
        #region Fields

        private int _stamp = 1;

        #endregion

        #region Constructors

        public Map()
        {
            Name = string.Empty;
            Properties = new Dictionary<string, string>();
            Layers = new LayerCollection();
        }

        public Map(string fileName)
            : this(XDocument.Load(fileName).Root) // newly 20130418  filename is the *** map.ciml
        {
        }

        public Map(XElement xe)
        {
            if (string.IsNullOrEmpty(xe.AttValue("Stamp")))
            {
                _stamp = xe.Descendants("Record").Count() + 1;  // 自动适应旧版
            }
            else
            {
                _stamp = Convert.ToInt32(xe.AttValue("Stamp"));
            }
            Name = xe.AttValue("Name");
            Properties = new Dictionary<string, string>();
            Layers = new LayerCollection();
            if (xe.Elements("Properties").Count() > 0)
            {
                foreach (var prop in xe.Elements("Properties").First().Attributes())
                {
                    Properties.Add(prop.Name.ToString(), prop.Value);
                }
            }
            foreach (var layer in xe.Elements("Table"))
            {
                Layers.Add(new VectorLayer(layer));
            }
        }

        #endregion

        #region Properties

        public LayerCollection Layers { get; private set; }
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; private set; }

        public string this[string property]
        {
            get
            {
                return Properties.ContainsKey(property) ? Properties[property] : string.Empty;
            }
            set
            {
                Properties[property] = value;
            }
        }

        #endregion

        #region Events

        public event Action<string, IFeature> FeatureAdded;
        protected void OnFeatureAdded(string layer, IFeature f)
        {
            if (FeatureAdded != null)
            {
                FeatureAdded(layer, f);
            }
        }

        public event EventHandler BeforeSave;
        protected void OnBeforeSave()
        {
            if (BeforeSave != null)
            {
                BeforeSave(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Methods

        public XElement ToXMap()
        {
            OnBeforeSave();
            XElement xroot = new XElement("Database");
            xroot.SetAttValue("Stamp", _stamp.ToString());
            xroot.SetAttValue("Name", Name);
            XElement xprop = new XElement("Properties");
            Properties.ForEach(x => xprop.Add(new XAttribute(x.Key, x.Value)));
            xroot.Add(xprop);
            Layers.ForEach(x => xroot.Add(x.ToXMap()));
            return xroot;
        }

        public void Save(string path)
        {
            this.ToXMap().Save(path);
        }

        public int AddFeature(string layerName, IFeature feature)
        {
            var layer = Layers[layerName];
            if (layer != null)
            {
                feature.FeatId = _stamp.ToString();
                layer.Features.Add(feature);
                OnFeatureAdded(layerName, feature);
                _stamp++;
                return _stamp - 1;
            }
            else
            {
                return -1;
            }
        }

        public Geometry.Extent2D GetExtents()
        {
            Geometry.Extent2D result = Geometry.Extent2D.Null;
            foreach (VectorLayer layer in Layers)
            {
                result.Add(layer.GetExtents());
            }
            return result.Area == 0 ? new Geometry.Extent2D(0, 0, 1000, 1000) : result; // mod 20140628
        }

        #endregion
    }

    public class LayerCollection : List<ILayer>
    {
        public ILayer this[string name]
        {
            get
            {
                return this.FirstOrDefault(x => x.Name == name); // mod 20130528
            }
        }
    }

    public interface ILayer
    {
        string Name { get; set; }
        List<IFeature> Features { get; }
        string GeoType { get; }
        bool IsVisible { get; set; }
        XElement ToXMap();
    }

    public abstract class Layer : ILayer
    {
        // fields

        protected string _name = string.Empty;
        protected List<IFeature> _features = new List<IFeature>();
        protected string _geoType = "0";
        protected bool _isVisible = true;

        // ILayer members

        public string Name { get { return _name; } set { _name = value; } }
        public string GeoType { get { return _geoType; } }
        public List<IFeature> Features { get { return _features; } }
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        public abstract XElement ToXMap();
    }

    public class VectorLayer : Layer
    {
        public const string GEOTYPE_UNKNOWN = "0"; // newly 20140625
        public const string GEOTYPE_POINT = "1"; // newly 20140624
        public const string GEOTYPE_LINEAR = "2";
        public const string GEOTYPE_REGION = "4";

        public string Code { get; set; }

        private VectorLayer()
        {
        }

        public VectorLayer(string name, string geoType)
            : this()
        {
            Code = string.Empty;
            _name = name;
            _geoType = geoType;
        }

        public VectorLayer(XElement xe)
        {
            Code = xe.AttValue("Code");
            _name = xe.AttValue("Name");
            _geoType = xe.AttValue("GeoType");

            foreach (var feature in xe.Elements("Record"))
            {
                Features.Add(new Feature(feature));
            }
        }

        public override XElement ToXMap()
        {
            XElement xe = new XElement("Table",
                new XAttribute("Code", Code),
                new XAttribute("Name", Name),
                new XAttribute("GeoType", GeoType));

            foreach (var record in _features)
            {
                xe.Add(record.ToXMap());
            }

            return xe;
        }

        private static VectorLayer _empty = new VectorLayer();
        public static VectorLayer Empty { get { return _empty; } }

        public Geometry.Extent2D GetExtents()
        {
            Geometry.Extent2D result = Geometry.Extent2D.Null;
            foreach (var feature in Features)
            {
                Geometry.Point2DString poly = new Geometry.Point2DString(feature.GeoData);
                result.Add(poly.GetExtent());
            }
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", Name, Features.Count);
        }
    }

    public class RasterLayer : Layer
    {
        public override XElement ToXMap()
        {
            throw new NotImplementedException();
        }
    }

    public interface IFeature
    {
        string FeatId { get; set; }
        List<Geometry.Point2D> GeoData { get; } // mod 20140620
        Dictionary<string, string> Properties { get; }
        string this[string property] { get; set; }
        Dictionary<string, object> Data { get; }
        XElement ToXMap(bool withProperties = true);
    }

    public class Feature : IFeature
    {
        protected string _featId = string.Empty;
        protected List<Geometry.Point2D> _geoData = new List<Geometry.Point2D>();
        protected Dictionary<string, string> _properties = new Dictionary<string, string>();
        protected Dictionary<string, object> _data = new Dictionary<string, object>();

        public Feature()
        {
        }

        public Feature(XElement xe)
        {
            _featId = xe.AttValue("FeatId");
            _geoData = ParseGeoData(xe.AttValue("Geo"));

            if (xe.HasElements)
            {
                var props = xe.Element("Properties");
                foreach (var prop in props.Attributes())
                {
                    _properties.Add(prop.Name.ToString(), prop.Value);
                }
            }
        }

        public Feature(params Geometry.Point2D[] pts) // newly 20140620
        {
            _geoData = pts.ToList();
        }

        public Feature(IEnumerable<Geometry.Point2D> pts) // newly 20140624
        {
            _geoData = pts.ToList();
        }

        // IFeature members

        public string FeatId { get { return _featId; } set { _featId = value; } }
        public List<Geometry.Point2D> GeoData { get { return _geoData; } set { _geoData = value; } }
        public Dictionary<string, string> Properties { get { return _properties; } }
        public Dictionary<string, object> Data { get { return _data; } }

        public string this[string property]
        {
            get
            {
                return Properties.ContainsKey(property) ? Properties[property] : string.Empty;
            }
            set
            {
                Properties[property] = value;
            }
        }

        public XElement ToXMap(bool withProperties = true)
        {
            XElement xe = new XElement("Record");
            xe.Add(new XAttribute("FeatId", FeatId));
            xe.Add(new XAttribute("Geo", StringifyGeoData(GeoData))); // mod 20130301
            if (withProperties)
            {
                XElement xeProp = new XElement("Properties");
                foreach (var prop in _properties)
                {
                    xeProp.Add(new XAttribute(prop.Key, prop.Value ?? string.Empty));
                }
                xe.Add(xeProp);
            }

            return xe;
        }

        public override string ToString() // newly 20140625
        {
            return string.Format("{0}|{1}", _featId, _geoData.Count);
        }

        // Utils

        private static List<Geometry.Point2D> ParseGeoData(string geoData)
        {
            return new Geometry.Polygon(geoData).Points;
        }

        private static string StringifyGeoData(List<Geometry.Point2D> geoData)
        {
            return new Geometry.Polygon(geoData).ToString();
        }
    }

    public static class FeatureExtensions // newly 20140625
    {
        public const double AREA_THRESHOLD = 0.3;
        public const double GAP_THRESHOLD = 0.3;

        public static double Length(this IFeature f)
        {
            return new Geometry.Polyline(f.GeoData).Length;
        }

        public static double Area(this IFeature f)
        {
            return new Geometry.Polygon(f.GeoData).Area;
        }

        public static string GuessGeoType(this IFeature f)
        {
            var pts = f.GeoData;
            if (pts.Count == 0)
            {
                return VectorLayer.GEOTYPE_UNKNOWN;
            }
            else if (pts.Count == 1)
            {
                return VectorLayer.GEOTYPE_POINT;
            }
            else
            {
                double area = f.Area();
                double length = f.Length();
                if (area < AREA_THRESHOLD * length * length / 16)
                {
                    return VectorLayer.GEOTYPE_LINEAR;
                }
                else
                {
                    return VectorLayer.GEOTYPE_REGION;
                }
            }
        }

        public static string GuessGeoType(this IEnumerable<IFeature> layer)
        {
            var ptCount = (double)layer.Sum(f => f.GeoData.Count) / layer.Count();
            if (ptCount < 1)
            {
                return VectorLayer.GEOTYPE_UNKNOWN;
            }
            else if (ptCount < 2)
            {
                return VectorLayer.GEOTYPE_POINT;
            }
            else
            {
                var n = layer.Average(f =>
                {
                    double dist = f.GeoData.First().DistTo(f.GeoData.Last());
                    double length = f.Length();
                    return dist / length;
                });
                if (n > GAP_THRESHOLD)
                {
                    return VectorLayer.GEOTYPE_LINEAR;
                }
                else
                {
                    return VectorLayer.GEOTYPE_REGION;
                }
            }
        }
    }

    #region Others

    public class FeatureCache
    {
        public IFeature Feature;
        public Geometry.Extent2D Extents;
        public string Layer;
        public string ID;

        public XElement ToXMap()
        {
            XElement xe = Feature.ToXMap();
            xe.SetAttValue("Layer", Layer);
            xe.SetAttValue("ID", ID);
            return xe;
        }

        public FeatureCache()
        {
        }

        public FeatureCache(XElement xe)
        {
            Feature = new Feature(xe);
            Layer = xe.AttValue("Layer");
            ID = xe.AttValue("ID");
        }
    }

    public class MapCache
    {
        public List<FeatureCache> Features { get; private set; }
        public Map Map { get; private set; }

        public MapCache()
        {
            Features = new List<FeatureCache>();
            Map = new Map();
        }

        public MapCache(Map map)
        {
            Features = new List<FeatureCache>();
            Map = map;

            int id = 1;
            foreach (VectorLayer layer in map.Layers)
            {
                foreach (var feature in layer.Features)
                {
                    FeatureCache fc = new FeatureCache();
                    fc.Feature = feature;
                    fc.Extents = new Geometry.Polygon(feature.GeoData).GetExtent();
                    fc.Layer = layer.Name;
                    fc.ID = id.ToString();
                    Features.Add(fc);
                    id++;
                }
            }
        }

        public static IEnumerable<FeatureCache> SpatialFind(IEnumerable<FeatureCache> features, Geometry.Extent2D extents)
        {
            return features.Where(x => x.Extents.IsCrossedBy(extents));
        }

        public static XElement Encode(IEnumerable<FeatureCache> features)
        {
            XElement xe = new XElement("MapCache");
            foreach (var feature in features)
            {
                xe.Add(feature.ToXMap());
            }
            return xe;
        }

        public void DecodeAndAppend(XElement xe)
        {
            foreach (var xFeature in xe.Elements())
            {
                FeatureCache fc = new FeatureCache(xFeature);
                if (!Features.Any(x => x.ID == fc.ID))
                {
                    Features.Add(fc);
                    Map.AddFeature(fc.Layer, fc.Feature);
                }
            }
        }

        public XElement FindAndEncode(Geometry.Extent2D extents)
        {
            return Encode(SpatialFind(Features, extents));
        }
    }

    public enum CollageDataType
    {
        Unknown,
        Ciml,
        Raster,
        Dem
    }

    public class CollagePiece
    {
        public string Name { get; set; }
        public string Group { get; set; }                           // 编组，可存储项目地点名称。
        public string Type { get; set; }                            // 类型，可存储图层名称。
        public CollageDataType DataType { get; set; }
        public object Data { get; set; }                            // 这个属性不是必须有值的，作为本地缓存使用。
        public Geometry.Extent2D LocalExtents { get; set; }
        public Geometry.Extent2D WorldExtents { get; set; }
        public string DataUri { get; set; }                         // 获取数据的路径。获取数据后，可填充在Data属性里。
        public DateTime Time { get; set; }                          // 时间元数据。
    }

    #endregion
}
