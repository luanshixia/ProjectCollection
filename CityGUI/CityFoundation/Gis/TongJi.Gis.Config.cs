using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

using TongJi.IO;

namespace TongJi.Gis
{
    public class MapProfile
    {
        public List<MapFile> MapFiles { get; set; }
        public List<string> TopLayerOrders { get; set; }
        public List<string> DefaultLayers { get; set; }
        public List<LayerStyle> Styles { get; set; }
        public string DefaultMap { get; set; }

        public MapProfile()
        {
            MapFiles = new List<MapFile>();
            TopLayerOrders = new List<string>();
            DefaultLayers = new List<string>();
            Styles = new List<LayerStyle>();
            DefaultMap = string.Empty;
        }

        public void Save(string fileName)
        {
            Serialization.XmlSave(this, fileName);
        }

        public static MapProfile Load(string fileName)
        {
            return Serialization.XmlLoad<MapProfile>(fileName);
        }

        public override string ToString()
        {
            return this.XmlEncode();
        }

        public static MapProfile Parse(string source)
        {
            return source.XmlDecode<MapProfile>();
        }

        public List<string> GetLayers()
        {
            var layers = MapFiles.SelectMany(x => x.Layers).Distinct().OrderBy(x => x).ToList();
            var topLayers = TopLayerOrders.Where(x => layers.Contains(x)).ToList();
            var otherLayers = layers.Where(x => !topLayers.Contains(x)).ToList();
            return topLayers.Concat(otherLayers).ToList();
        }

        public List<MapFile> GetFilesByLayers(params string[] layers)
        {
            return MapFiles.Where(x => x.Layers.Any(y => layers.Contains(y))).ToList();
        }

        public List<MapFile> GetFilesByLods(params int[] lods)
        {
            return MapFiles.Where(x => x.Lods.Any(y => lods.Contains(y))).ToList();
        }

        public List<MapFile> GetFilesByExtents(Geometry.Extent2D extents)
        {
            return MapFiles.Where(x => x.Extents.IsCrossedBy(extents)).ToList();
        }

        public static Map MergeMaps(string[] mapFiles, string[] layers)
        {
            if (mapFiles.Length > 0)
            {
                var maps = mapFiles.Select(x => new Map(x)).ToList();
                Map mergedMap = new Map();
                foreach (var layer in layers)
                {
                    var layersFromMaps = maps.SelectMany(x => x.Layers).Where(x => x.Name == layer).ToList();
                    if (layersFromMaps.Count > 0 && layersFromMaps.IsSame(x => x.GeoType))
                    {
                        VectorLayer mergedLayer = new VectorLayer(layer, layersFromMaps[0].GeoType);
                        mergedLayer.Features.AddRange(layersFromMaps.SelectMany(x => x.Features));
                        mergedMap.Layers.Add(mergedLayer);
                    }
                }
                return mergedMap;
            }
            else
            {
                return new Map();
            }
        }
    }

    public class MapFile
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string FileName { get; set; }
        [XmlAttribute]
        public string ScaleRange { get; set; }
        [XmlAttribute]
        public string HotLoad { get; set; }
        public Geometry.Extent2D Extents { get; set; }
        public List<string> Layers { get; set; }
        public List<int> Lods { get; set; }

        public MapFile()
        {
            Name = string.Empty;
            FileName = string.Empty;
            ScaleRange = "1024|0.015625";
            Extents = Geometry.Extent2D.Null;
            Layers = new List<string>();
            Lods = new List<int>();
        }
    }

    public class LayerStyle
    {
        [XmlAttribute]
        public string LayerName { get; set; }
        [XmlAttribute]
        public double StrokeWeight { get; set; }
        public LayerColor Stroke { get; set; }
        public List<double> StrokeDashArray { get; set; }
        public LayerColor Fill { get; set; }
        [XmlAttribute]
        public double SpotSize { get; set; }
        [XmlAttribute]
        public double FontSize { get; set; }
        public LayerColor FontBrush { get; set; }

        public LayerStyle()
        {
            StrokeWeight = 1;
            Stroke = LayerColor.FromArgb(255, 128, 128, 128);
            Fill = LayerColor.FromArgb(255, 255, 255, 180);
            SpotSize = 20;
            FontSize = 20;
            FontBrush = LayerColor.FromArgb(255, 0, 0, 0);
        }
    }

    public class LayerColor
    {
        [XmlAttribute]
        public byte A { get; set; }
        [XmlAttribute]
        public byte R { get; set; }
        [XmlAttribute]
        public byte G { get; set; }
        [XmlAttribute]
        public byte B { get; set; }

        public static LayerColor FromArgb(byte a, byte r, byte g, byte b)
        {
            return new LayerColor { A = a, R = r, G = g, B = b };
        }
    }
}
