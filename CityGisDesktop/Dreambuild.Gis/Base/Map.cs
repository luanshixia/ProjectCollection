using Dreambuild.Extensions;
using Dreambuild.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region Properties

        public LayerCollection Layers { get; internal set; }
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; internal set; }

        public string this[string property]
        {
            get
            {
                return this.Properties.ContainsKey(property) ? this.Properties[property] : string.Empty;
            }
            set
            {
                this.Properties[property] = value;
            }
        }

        #endregion

        #region Constructors

        public Map()
        {
            this.Name = string.Empty;
            this.Properties = new Dictionary<string, string>();
            this.Layers = new LayerCollection();
        }

        public Map(string fileName)
            : this(XDocument.Load(fileName).Root) // newly 20130418  filename is the *** map.ciml
        {
        }

        public Map(XElement xe)
        {
            this._stamp = string.IsNullOrEmpty(xe.AttValue("Stamp"))
                ? xe.Descendants("Record").Count() + 1  // legacy format
                : Convert.ToInt32(xe.AttValue("Stamp"));

            this.Name = xe.AttValue("Name");
            this.Properties = new Dictionary<string, string>();
            this.Layers = new LayerCollection();

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

        #region Events

        public event Action<string, IFeature> FeatureAdded;
        protected void OnFeatureAdded(string layer, IFeature f)
        {
            this.FeatureAdded?.Invoke(layer, f);
        }

        public event EventHandler BeforeSave;
        protected void OnBeforeSave()
        {
            this.BeforeSave?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Methods

        public XElement ToXMap()
        {
            this.OnBeforeSave();

            var xroot = new XElement("Database");
            xroot.SetAttValue("Stamp", _stamp.ToString());
            xroot.SetAttValue("Name", Name);

            var xprop = new XElement("Properties");
            this.Properties.ForEach(x => xprop.Add(new XAttribute(x.Key, x.Value)));
            xroot.Add(xprop);

            this.Layers.ForEach(x => xroot.Add(x.ToXMap()));
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
                feature.FeatId = this._stamp.ToString();
                layer.Features.Add(feature);
                this.OnFeatureAdded(layerName, feature);
                this._stamp++;
                return this._stamp - 1;
            }
            else
            {
                return -1;
            }
        }

        public Extents GetExtents()
        {
            var result = Extents.Empty;
            foreach (VectorLayer layer in Layers)
            {
                result = result.Add(layer.GetExtents());
            }
            return result.IsEmpty() || result.Area() == 0 ? Extents.Create(0, 1000, 0, 1000) : result; // mod 20140628
        }

        public static Map Merge(params Map[] maps) // newly 20141111
        {
            var map = new Map();
            var layerNames = maps.SelectMany(m => m.Layers.Select(l => l.Name)).ToList();
            foreach (var name in layerNames)
            {
                var layersToMerge = maps.SelectMany(m => m.Layers.Where(l => l.Name == name)).ToList();
                var layer = new VectorLayer(name, layersToMerge[0].GeoType);
                layersToMerge.SelectMany(l => l.Features).ForEach(f => layer.Features.Add(f));
                map.Layers.Add(layer);
            }
            return map;
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
            this.Code = string.Empty;
            this._name = name;
            this._geoType = geoType;
        }

        public VectorLayer(XElement xe)
        {
            this.Code = xe.AttValue("Code");
            this._name = xe.AttValue("Name");
            this._geoType = xe.AttValue("GeoType");

            foreach (var feature in xe.Elements("Record"))
            {
                this.Features.Add(new Feature(feature));
            }
        }

        public override XElement ToXMap()
        {
            var xe = new XElement("Table",
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

        public Extents GetExtents()
        {
            var result = Extents.Empty;
            foreach (var feature in Features)
            {
                var poly = new PointString(feature.GeoData);
                result = result.Add(poly.GetExtents());
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
        List<Vector> GeoData { get; } // mod 20140620
        Dictionary<string, string> Properties { get; }
        string this[string property] { get; set; }
        Dictionary<string, object> Data { get; }
        XElement ToXMap(bool withProperties = true);
    }

    public class Feature : IFeature
    {
        protected string _featId = string.Empty;
        protected List<Vector> _geoData = new List<Vector>();
        protected Dictionary<string, string> _properties = new Dictionary<string, string>();
        protected Dictionary<string, object> _data = new Dictionary<string, object>();

        public Feature()
        {
        }

        public Feature(XElement xe)
        {
            this._featId = xe.AttValue("FeatId");
            this._geoData = ParseGeoData(xe.AttValue("Geo"));

            if (xe.HasElements)
            {
                var props = xe.Element("Properties");
                foreach (var prop in props.Attributes())
                {
                    this._properties.Add(prop.Name.ToString(), prop.Value);
                }
            }
        }

        public Feature(params Vector[] pts) // newly 20140620
        {
            _geoData = pts.ToList();
        }

        public Feature(IEnumerable<Vector> pts) // newly 20140624
        {
            _geoData = pts.ToList();
        }

        // IFeature members

        public string FeatId { get { return _featId; } set { _featId = value; } }
        public List<Vector> GeoData { get { return _geoData; } set { _geoData = value; } }
        public Dictionary<string, string> Properties { get { return _properties; } }
        public Dictionary<string, object> Data { get { return _data; } }

        public string this[string property]
        {
            get
            {
                return this.Properties.ContainsKey(property) ? this.Properties[property] : string.Empty;
            }
            set
            {
                this.Properties[property] = value;
            }
        }

        public XElement ToXMap(bool withProperties = true)
        {
            var xe = new XElement("Record");
            xe.Add(new XAttribute("FeatId", this.FeatId));
            xe.Add(new XAttribute("Geo", Feature.StringifyGeoData(GeoData))); // mod 20130301
            if (withProperties)
            {
                var xeProp = new XElement("Properties");
                foreach (var prop in this._properties)
                {
                    xeProp.Add(new XAttribute(prop.Key, prop.Value ?? string.Empty));
                }
                xe.Add(xeProp);
            }

            return xe;
        }

        public override string ToString() // newly 20140625
        {
            return string.Format("{0}|{1}", this._featId, this._geoData.Count);
        }

        // Utils

        private static List<Vector> ParseGeoData(string geoData)
        {
            return PointString.Parse(geoData).Points;
        }

        private static string StringifyGeoData(List<Vector> geoData)
        {
            return new PointString(geoData).ToString();
        }
    }

    public static class FeatureExtensions // newly 20140625
    {
        public const double AREA_THRESHOLD = 0.3;
        public const double GAP_THRESHOLD = 0.3;

        public static double Length(this IFeature f)
        {
            return new PointString(f.GeoData).Length();
        }

        public static double Area(this IFeature f)
        {
            return new PointString(f.GeoData).Area();
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
                var area = f.Area();
                var length = f.Length();
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
                    var dist = f.GeoData.First().Dist(f.GeoData.Last());
                    var length = f.Length();
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
        public Extents Extents;
        public string Layer;
        public string ID;

        public XElement ToXMap()
        {
            var xe = Feature.ToXMap();
            xe.SetAttValue("Layer", Layer);
            xe.SetAttValue("ID", ID);
            return xe;
        }

        public FeatureCache()
        {
        }

        public FeatureCache(XElement xe)
        {
            this.Feature = new Feature(xe);
            this.Layer = xe.AttValue("Layer");
            this.ID = xe.AttValue("ID");
        }
    }

    public class MapCache
    {
        public List<FeatureCache> Features { get; private set; }
        public Map Map { get; private set; }

        public MapCache()
        {
            this.Features = new List<FeatureCache>();
            this.Map = new Map();
        }

        public MapCache(Map map)
        {
            this.Features = new List<FeatureCache>();
            this.Map = map;

            var id = 1;
            foreach (VectorLayer layer in map.Layers)
            {
                foreach (var feature in layer.Features)
                {
                    var fc = new FeatureCache
                    {
                        Feature = feature,
                        Extents = new PointString(feature.GeoData).GetExtents(),
                        Layer = layer.Name,
                        ID = id.ToString()
                    };
                    Features.Add(fc);
                    id++;
                }
            }
        }

        public static IEnumerable<FeatureCache> SpatialFind(IEnumerable<FeatureCache> features, Extents extents)
        {
            return features.Where(x => x.Extents.IsCross(extents));
        }

        public static XElement Encode(IEnumerable<FeatureCache> features)
        {
            var xe = new XElement("MapCache");
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
                var fc = new FeatureCache(xFeature);
                if (!Features.Any(x => x.ID == fc.ID))
                {
                    this.Features.Add(fc);
                    this.Map.AddFeature(fc.Layer, fc.Feature);
                }
            }
        }

        public XElement FindAndEncode(Extents extents)
        {
            return MapCache.Encode(MapCache.SpatialFind(this.Features, extents));
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
        public Extents LocalExtents { get; set; }
        public Extents WorldExtents { get; set; }
        public string DataUri { get; set; }                         // 获取数据的路径。获取数据后，可填充在Data属性里。
        public DateTime Time { get; set; }                          // 时间元数据。
    }

    #endregion
}
