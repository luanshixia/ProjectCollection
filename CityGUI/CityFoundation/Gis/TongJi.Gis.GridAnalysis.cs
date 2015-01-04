using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;
using System.Drawing.Imaging;

using TongJi.Geometry;
using TongJi.IO;

namespace TongJi.Gis
{
    /// <summary>
    /// GA服务器端位图渲染 20130423
    /// </summary>
    public class GABitmapRenderer
    {
        private int _width;
        private int _height;
        private double[] _data;
        private Color[] _colors;
        private double[] _stops;
        private double[] _borders;
        private double _min;
        private double _max;

        public GABitmapRenderer(int width, int height, double[] data)
        {
            _width = width;
            _height = height;
            _data = data;
            _colors = new Color[] { Color.Blue, Color.Cyan, Color.Yellow, Color.Red };
            _min = data.Min();
            _max = data.Max();
            _stops = new double[] { GetValueAtParam(0), GetValueAtParam(0.33), GetValueAtParam(0.67), GetValueAtParam(1) };
            _borders = new double[] { GetValueAtParam(0.25), GetValueAtParam(0.5), GetValueAtParam(0.75) };
        }

        public void SetColors(params Color[] colors)
        {
            _colors = colors;
        }

        public void SetStops(params double[] stops)
        {
            _stops = stops.OrderBy(x => x).ToArray();
        }

        public void SetBorders(params double[] borders)
        {
            _borders = borders.OrderBy(x => x).ToArray();
        }

        public Color GetColorOfValue(double value, bool graded)
        {
            if (graded)
            {
                for (int i = 0; i < _borders.Length; i++)
                {
                    if (value < _borders[i])
                    {
                        return _colors[i];
                    }
                }
                return _colors[_borders.Length];
            }
            else
            {
                if (value < _stops[0])
                {
                    return _colors[0];
                }
                for (int i = 0; i < _stops.Length - 1; i++)
                {
                    if (value < _stops[i + 1])
                    {
                        return GetColorAtParam(_colors[i], _colors[i + 1], (value - _stops[i]) / (_stops[i + 1] - _stops[i]));
                    }
                }
                return _colors[_stops.Length - 1];
            }
        }

        private static Color GetColorAtParam(Color ca, Color cb, double param)
        {
            int a = (int)GetValueAtParam(ca.A, cb.A, param);
            int r = (int)GetValueAtParam(ca.R, cb.R, param);
            int g = (int)GetValueAtParam(ca.G, cb.G, param);
            int b = (int)GetValueAtParam(ca.B, cb.B, param);
            return Color.FromArgb(a, r, g, b);
        }

        private static double GetValueAtParam(double min, double max, double param)
        {
            return min + param * (max - min);
        }

        public double GetValueAtParam(double param)
        {
            return GetValueAtParam(_min, _max, param);
        }

        public void Render(System.IO.Stream stream, bool graded)
        {
            Bitmap bitmap = new Bitmap(_width, _height);
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    int n = i * _height + (_height - 1 - j);
                    bitmap.SetPixel(i, j, GetColorOfValue(_data[n], graded));
                }
            }
            bitmap.Save(stream, ImageFormat.Png);
        }
    }

    /// <summary>
    /// 纯计算版的网格分析
    /// </summary>
    public static class GridAnalysis
    {
        public const double CellSize = 100;
        public static List<Point2D> Grid { get; private set; }
        public static Func<Point2D, double> Function { get; set; }
        //public static Dictionary<Point2D, double> ValueCache = new Dictionary<Point2D, double>();
        public static List<double> ValueCache = new List<double>(); // 20130425 做地块计算时发现有的地块中心点重合，遂取消字典。
        public static List<GAFactor> Factors { get; private set; }
        public static Func<GAFactor, Func<Point2D, double>> Formula { get; set; }
        public static Map MapData { get; set; }

        public static void Init(Map map, List<Point2D> grid = null)
        {
            Function = p => 0;
            Factors = new List<GAFactor>();
            Formula = f => p => 0;

            MapData = map;
            DefaultFormula();
            DefaultFunction();
            if (grid == null)
            {
                BuildGrid(map.GetExtents());
            }
            else
            {
                Grid = grid;
            }
        }

        public static void DefaultFunction()
        {
            Function = p => Factors.Sum(f => Formula(f)(p));
        }

        public static void FactorLayers(params string[] layers)
        {
            Factors = layers.SelectMany(x => MapData.Layers[x].Features.Select(f =>
            {
                GAFactor factor = new GAFactor();
                factor.Type = f["ga-type"];
                factor.Radius = f["ga-radius"].TryParseToDouble();
                factor.Mass = f["ga-mass"].TryParseToDouble();
                if (factor.Type == "spot")
                {
                    factor.Spot = new Point2D(f.GeoData);
                }
                else if (factor.Type == "linear")
                {
                    factor.Linear = new Polyline(f.GeoData);
                }
                else if (factor.Type == "region") // newly 20130523
                {
                    factor.Linear = new Polyline(f.GeoData);
                    factor.Region = new Polygon(f.GeoData);
                }
                return factor;
            }).ToList()).ToList();
        }

        public static void DefaultFormula()
        {
            Formula = f => p =>
            {
                if (f.Type == "spot")
                {
                    double d = f.Spot.DistTo(p);
                    return (d > f.Radius) ? 0 : f.Mass * (1 - d / f.Radius);
                }
                else if (f.Type == "linear")
                {
                    if (f.Linear.Count == 0) // 数据容错
                    {
                        return 0;
                    }
                    double d = f.Linear.DistToPoint(p);
                    return (d > f.Radius) ? 0 : f.Mass * (1 - d / f.Radius);
                }
                else if (f.Type == "region") // newly 20130523
                {
                    if (f.Region.IsPointIn(p))
                    {
                        return f.Mass;
                    }
                    else
                    {
                        if (f.Linear.Count == 0) // 数据容错
                        {
                            return 0;
                        }
                        double d = f.Linear.DistToPoint(p);
                        return (d > f.Radius) ? 0 : f.Mass * (1 - d / f.Radius);
                    }
                }
                return 0;
            };
        }

        public static void AddProp(string layer, string prop, string defaultValue)
        {
            MapData.Layers[layer].Features.ForEach(f =>
            {
                f[prop] = defaultValue;
            });
        }

        public static void AddProp(string layer, string prop, Func<IFeature, object> valueSelector)
        {
            MapData.Layers[layer].Features.ForEach(f =>
            {
                f[prop] = valueSelector(f).ToString();
            });
        }

        public static void BuildGrid(Extent2D extents)
        {
            Grid = extents.GetGrid(CellSize);
        }

        public static void Calculate()
        {
            ValueCache.Clear();
            Grid.ForEach(p =>
            {
                ValueCache.Add(Function(p));
            });
        }

        public static string GetBakedResult()
        {
            return string.Join("|", ValueCache.Select(x => x.ToString("0.####")).ToArray());
        }

        public static string BakeAnalysis(string layer)
        {
            FactorLayers(layer);
            Calculate();
            return GetBakedResult();
        }
    }

    public class GAFactor
    {
        public string Type { get; set; }
        public Point2D Spot { get; set; }
        public Polyline Linear { get; set; }
        public Polygon Region { get; set; } // newly 20130523
        public double Radius { get; set; }
        public double Mass { get; set; }
    }

    [Obsolete]
    public class GABakedResult
    {
        public TongJi.Geometry.Extent2D Extents { get; set; }
        [XmlAttribute]
        public double CellSize { get; set; }
        [XmlAttribute]
        public int ColCount { get; set; }
        [XmlAttribute]
        public int RowCount { get; set; }
        public List<double> Values { get; set; }
    }

    public class GABakedResultStorage
    {
        public List<GAMapFile> Files { get; set; }

        public GABakedResultStorage()
        {
            Files = new List<GAMapFile>();
        }

        public void Save(string fileName)
        {
            Serialization.XmlSave(this, fileName);
        }

        public static GABakedResultStorage Load(string fileName)
        {
            return Serialization.XmlLoad<GABakedResultStorage>(fileName);
        }

        public static GABakedResultStorage LoadOrNew(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                return Load(fileName);
            }
            else
            {
                return new GABakedResultStorage();
            }
        }

        public string[] GetLayerNames(string file)
        {
            var mapFile = Files.FirstOrDefault(f => f.Name == file); // mod 20130528
            if (mapFile != null)
            {
                return mapFile.Layers.Select(x => x.Name).ToArray();
            }
            return new string[0];
        }

        public void Store(string file, string layer, string data, bool parcel = false)
        {
            if (!Files.Any(f => f.Name == file))
            {
                Files.Add(new GAMapFile { Name = file });
            }
            var mapFile = Files.First(f => f.Name == file);
            if (!mapFile.Layers.Any(l => l.Name == layer))
            {
                mapFile.Layers.Add(new GAMapLayer { Name = layer });
            }
            if (parcel)
            {
                mapFile.Layers.First(l => l.Name == layer).ParcelData = data;
            }
            else
            {
                mapFile.Layers.First(l => l.Name == layer).Data = data;
            }
        }

        public string Fetch(string file, string layer, bool parcel = false)
        {
            if (Files.Any(f => f.Name == file))
            {
                var mapFile = Files.First(f => f.Name == file);
                if (mapFile.Layers.Any(l => l.Name == layer))
                {
                    if (parcel)
                    {
                        return mapFile.Layers.First(l => l.Name == layer).ParcelData;
                    }
                    else
                    {
                        return mapFile.Layers.First(l => l.Name == layer).Data;
                    }
                }
            }
            return null;
        }
    }

    public class GAMapFile
    {
        [XmlAttribute]
        public string Name { get; set; }
        public List<GAMapLayer> Layers { get; set; }

        public GAMapFile()
        {
            Layers = new List<GAMapLayer>();
        }
    }

    public class GAMapLayer
    {
        [XmlAttribute]
        public string Name { get; set; }
        public string Data { get; set; }
        public string ParcelData { get; set; } // newly 20130425
    }
}
