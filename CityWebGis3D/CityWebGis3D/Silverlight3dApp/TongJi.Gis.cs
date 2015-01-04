using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TongJi.Gis
{
    public interface IMap
    {
        LayerCollection Layers { get; }
    }

    public class Map : IMap
    {
        private LayerCollection _layers = new LayerCollection();
        private int _stamp = 1;

        public LayerCollection Layers { get { return _layers; } }

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

        public event Action<string, IFeature> FeatureAdded;
        public void OnFeatureAdded(string layer, IFeature f)
        {
            if (FeatureAdded != null)
            {
                FeatureAdded(layer, f);
            }
        }

        public Map()
        {
            Name = string.Empty;
            Properties = new Dictionary<string, string>();
        }

        public Map(XElement xe)
        {
            Name = xe.AttValue("Name");
            Properties = new Dictionary<string, string>();
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

        public XElement ToXMap()
        {
            throw new NotImplementedException();
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
            return result;
        }
    }

    public class LayerCollection : List<ILayer>
    {
        public ILayer this[string name]
        {
            get
            {
                if (this.Any(x => x.Name == name))
                {
                    return this.First(x => x.Name == name);
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public interface ILayer
    {
        string Name { get; set; }
        List<IFeature> Features { get; }
        bool IsVisible { get; set; }
        //VectorStyle Style { get; }
        XElement ToXMap();
    }

    public abstract class Layer : ILayer
    {
        // fields

        protected string _name = string.Empty;
        protected List<IFeature> _features = new List<IFeature>();
        protected bool _isVisible = true;
        //protected VectorStyle _style = new VectorStyle();

        // ILayer members

        public string Name { get { return _name; } set { _name = value; } }

        public List<IFeature> Features { get { return _features; } }

        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }

        //public VectorStyle Style { get { return _style; } }

        public abstract XElement ToXMap();
    }

    public class VectorLayer : Layer
    {
        private string _code = string.Empty;
        private string _geoType = "0";
        //private static Functional.Delay<UserProperty> _userProperty = new Functional.Delay<UserProperty>(() => new UserProperty("UserProperty.xml"));

        public string Code { get { return _code; } }

        public string GeoType { get { return _geoType; } }

        //public VectorLayer(string name)
        //{
        //    _name = name;
        //    try
        //    {
        //        XElement component = _userProperty.Value.GetComponentByName(name);
        //        _code = component.AttValue("Number");
        //        _geoType = component.AttValue("FDOType");
        //    }
        //    catch
        //    {
        //    }
        //}

        private VectorLayer()
        {
        }

        public VectorLayer(string name, string geoType)
        {
            _name = name;
            _geoType = geoType;
        }

        public VectorLayer(XElement xe)
        {
            _code = xe.AttValue("Code");
            _name = xe.AttValue("Name");
            _geoType = xe.AttValue("GeoType");

            foreach (var feature in xe.Elements())
            {
                Features.Add(new Feature(feature));
            }
        }

        public override XElement ToXMap()
        {
            throw new NotImplementedException();
        }

        private static VectorLayer _empty = new VectorLayer();
        public static VectorLayer Empty { get { return _empty; } }

        public Geometry.Extent2D GetExtents()
        {
            Geometry.Extent2D result = Geometry.Extent2D.Null;
            foreach (var feature in Features)
            {
                Geometry.Point2DSet poly = new Geometry.Point2DSet(feature.GeoData);
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
        string GeoData { get; set; }
        Dictionary<string, string> Properties { get; }
        string this[string property] { get; set; }
        XElement ToXMap();
    }

    public class Feature : IFeature
    {
        protected string _featId = string.Empty;
        protected string _geoData = string.Empty;
        protected Dictionary<string, string> _properties = new Dictionary<string, string>();

        public Feature()
        {
        }

        public Feature(XElement xe)
        {
            _featId = xe.AttValue("FeatId");
            _geoData = xe.AttValue("Geo");

            if (xe.HasElements)
            {
                var props = xe.Element("Properties");
                foreach (var prop in props.Attributes())
                {
                    _properties.Add(prop.Name.ToString(), prop.Value);
                }
            }
        }

        // IFeature members

        public string FeatId { get { return _featId; } set { _featId = value; } }

        public string GeoData { get { return _geoData; } set { _geoData = value; } }

        public Dictionary<string, string> Properties { get { return _properties; } }

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

        public XElement ToXMap()
        {
            throw new NotImplementedException();
        }
    }

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
                    fc.Extents = new Geometry.Polyline(feature.GeoData).GetExtent();
                    fc.Layer = layer.Name;
                    fc.ID = id.ToString();
                    Features.Add(fc);
                    id++;
                }
            }
        }

        public static IEnumerable<FeatureCache> SpatialFind(IEnumerable<FeatureCache> features, Geometry.Extent2D extents)
        {
            return features.Where(x => x.Extents.IsExtentsCross(extents));
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
}
