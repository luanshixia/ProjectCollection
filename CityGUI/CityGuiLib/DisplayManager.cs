using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using TongJi.Geometry;

namespace TongJi.City
{
    public interface IColorMapper
    {
        //double MinValue { get; set; }
        //double MaxValue { get; set; }
        Color GetColorOfValue(double value);
    }

    public class BiColorGradientMapper : IColorMapper
    {
        public Color ColorA { get; set; }
        public Color ColorB { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        public BiColorGradientMapper()
        {
            MinValue = OptionsManager.Singleton.MinValue;
            MaxValue = OptionsManager.Singleton.MaxValue;
            ColorA = OptionsManager.Singleton.MinValueColor; //Color.FromArgb(192, 192, 0);
            ColorB = OptionsManager.Singleton.MaxValueColor; //Color.FromArgb(192, 0, 0);
        }

        public Color GetColorOfValue(double value)
        {
            if (value >= MaxValue)
            {
                return ColorB;
            }
            else if (value <= MinValue)
            {
                return ColorA;
            }
            else
            {
                double lambda = (value - MinValue) / (MaxValue - value);
                int r = (int)((ColorA.R + lambda * ColorB.R) / (1 + lambda));
                int g = (int)((ColorA.G + lambda * ColorB.G) / (1 + lambda));
                int b = (int)((ColorA.B + lambda * ColorB.B) / (1 + lambda));
                return Color.FromArgb(r, g, b);
            }
        }

        public Color GetColorOfValue(double value, int levels)
        {
            double levelIncrement = 1.0 / levels;
            double div = (value - MinValue) / (MaxValue - MinValue);
            div = Math.Floor(div / levelIncrement) * (1.0 / (levels - 1));
            if (div >= 1)
            {
                return ColorB;
            }
            else if (div <= 0)
            {
                return ColorA;
            }
            else
            {
                double lambda = div / (1 - div);
                int r = (int)((ColorA.R + lambda * ColorB.R) / (1 + lambda));
                int g = (int)((ColorA.G + lambda * ColorB.G) / (1 + lambda));
                int b = (int)((ColorA.B + lambda * ColorB.B) / (1 + lambda));
                return Color.FromArgb(r, g, b);
            }
        }
    }

    public class BiColorLevelsMapper : IColorMapper
    {
        public Color ColorA { get; set; }
        public Color ColorB { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int LevelCount { get; set; }

        public BiColorLevelsMapper(double min, double max, int levelCount)
        {
            MinValue = min;
            MaxValue = max;
            ColorA = OptionsManager.Singleton.MinValueColor; //Color.FromArgb(192, 192, 0);
            ColorB = OptionsManager.Singleton.MaxValueColor; //Color.FromArgb(192, 0, 0);
            LevelCount = levelCount;
        }

        public Color GetColorOfValue(double value)
        {
            double levelIncrement = 1.0 / LevelCount;
            double div = (value - MinValue) / (MaxValue - MinValue);
            div = Math.Floor(div / levelIncrement) * (1.0 / (LevelCount - 1));
            if (div >= 1)
            {
                return ColorB;
            }
            else if (div <= 0)
            {
                return ColorA;
            }
            else
            {
                double lambda = div / (1 - div);
                int r = (int)((ColorA.R + lambda * ColorB.R) / (1 + lambda));
                int g = (int)((ColorA.G + lambda * ColorB.G) / (1 + lambda));
                int b = (int)((ColorA.B + lambda * ColorB.B) / (1 + lambda));
                return Color.FromArgb(r, g, b);
            }
        }
    }

    public class LogicalColorMapper : IColorMapper
    {
        public Color ColorA { get; private set; }
        public Color ColorB { get; private set; }
        public Func<double, bool> Logic { get; private set; }

        public LogicalColorMapper(Func<double, bool> logic, Color trueColor, Color falseColor)
        {
            ColorA = trueColor;
            ColorB = falseColor;
            Logic = logic;
        }

        public Color GetColorOfValue(double value)
        {
            return Logic(value) ? ColorA : ColorB;
        }
    }

    public class MultiColorGradientMapper : IColorMapper
    {
        private SortedDictionary<double, Color> _stops = new SortedDictionary<double, Color>();
        public SortedDictionary<double, Color> Stops { get { return _stops; } }

        public void AddStop(double value, Color color)
        {
            _stops.Add(value, color);
        }

        public Color GetColorOfValue(double value)
        {
            double[] values = _stops.Keys.ToArray();
            if (value < values.First())
            {
                return _stops.First().Value;
            }
            else
            {
                for (int i = 0; i < values.Length - 1; i++)
                {
                    double a = values[i];
                    double b = values[i + 1];
                    if (value >= a && value < b)
                    {
                        BiColorGradientMapper bcgm = new BiColorGradientMapper { MinValue = a, MaxValue = b, ColorA = _stops[a], ColorB = _stops[b] };
                        return bcgm.GetColorOfValue(value);
                    }
                }
                return _stops.Last().Value;
            }
        }
    }

    public class LeveledColorDecorator : IColorMapper
    {
        private IColorMapper _mapper;
        private double[] _levels;
        public IColorMapper Mapper { get { return _mapper; } }

        public LeveledColorDecorator(IColorMapper mapper, double[] levels)
        {
            _mapper = mapper;
            _levels = levels.OrderBy(x => x).ToArray();
        }

        public Color GetColorOfValue(double value)
        {
            if (value < _levels[0])
            {
                return _mapper.GetColorOfValue(_levels[0]);
            }
            else
            {
                for (int i = 0; i < _levels.Length - 1; i++)
                {
                    if (value >= _levels[i] && value < _levels[i + 1])
                    {
                        return _mapper.GetColorOfValue(_levels[i]);
                    }
                }
                return _mapper.GetColorOfValue(_levels.Last());
            }
        }
    }

    //public class MultiColorMapper : IColorMapper
    //{
    //}

    public class DisplayManager
    {
        public Graphics g;
        public int Width;
        public int Height;
        public const int SpotSize = 10;

        private AnalysisDistrict _cityModel;
        public AnalysisDistrict CityModel { get { return _cityModel; } } // mod 20110801

        private TongJi.Network.PathFinder _pathModel;
        public TongJi.Network.PathFinder PathModel { get { return _pathModel; } }

        public IColorMapper ColorMapper;

        private static DisplayManager _current = null;
        public static DisplayManager Current { get { return _current; } }

        public DisplayManager(CityDistrict cityModel, bool withoutFactors = false)
        {
            OptionsManager.Singleton.Load();

            _cityModel = new AnalysisDistrict(cityModel, withoutFactors);
            _pathModel = new TongJi.Network.PathFinder(_cityModel.Roads.Select(x => x.Alignment).ToList());

            _current = this;

            ColorMapper = new BiColorGradientMapper();
        }

        /// <summary>
        /// Length in city model represented by one pixel.
        /// </summary>
        public double Scale { get; set; }

        /// <summary>
        /// Point in city model represented by canvas center.
        /// </summary>
        public Point2D Center { get; set; }

        public Point2D CityCoordinate(int col, int row)
        {
            double x = Center.x + (col - Width / 2.0) * Scale;
            double y = Center.y - (row - Height / 2.0) * Scale;
            return new Point2D(x, y);
        }

        public Point CanvasCoordinate(Point2D cityPoint)
        {
            Vector2D v = (1.0 / Scale) * (cityPoint - Center);
            int col = (int)(Width / 2.0 + v.x);
            int row = (int)(Height / 2.0 - v.y);
            return new Point(col, row);
        }

        public PointF CanvasCoordinateF(Point2D cityPoint)
        {
            Vector2D v = (1.0 / Scale) * (cityPoint - Center);
            float col = (float)(Width / 2.0 + v.x);
            float row = (float)(Height / 2.0 - v.y);
            return new PointF(col, row);
        }

        internal void PaintRoad()
        {
            var pen = new Pen(OptionsManager.Singleton.RoadColor, 5);
            foreach (CityRoad road in _cityModel.Roads)
            {
                Point[] pts = road.Alignment.Points.Select(x => CanvasCoordinate(x)).ToArray();
                g.DrawLines(pen, pts);
            }
        }

        internal void PaintParcel()
        {
            var brush = new SolidBrush(OptionsManager.Singleton.ViewportLineColor);
            var pen = new Pen(OptionsManager.Singleton.ViewportLineColor);
            foreach (CityParcel parcel in _cityModel.Parcels)
            {
                Point[] pts = parcel.Domain.Points.Select(x => CanvasCoordinate(x)).ToArray();
                g.DrawPolygon(pen, pts);

                // Paint parcel centroid
                RectangleF rec = new RectangleF(CanvasCoordinate(parcel.Domain.Centroid), new Size(1, 1));
                g.FillRectangle(brush, rec);
            }
        }

        internal void PaintValue()
        {
            ValueBuffer.Units.ForEach(x => x.Paint(this.ColorMapper.GetColorOfValue(x.Value)));
        }

        internal void PaintSpot()
        {
            SolidBrush sb = new SolidBrush(Color.Tomato);
            Pen pn = new Pen(Color.Black, 2);
            foreach (var spot in _cityModel.CitySpots)
            {
                PointF center = CanvasCoordinate(spot.Position);
                RectangleF rect = new RectangleF(new PointF(center.X - SpotSize / 2, center.Y - SpotSize / 2), new Size(SpotSize, SpotSize));
                g.FillEllipse(sb, rect);
                g.DrawEllipse(pn, rect);
            }
        }

        internal void PaintSSet()
        {
            SolidBrush sb = new SolidBrush(Color.Yellow);
            Pen pn = new Pen(Color.Yellow, 2);
            Pen pen = new Pen(Color.Yellow, 5);
            foreach (var entity in PyCmd.sset)
            {
                if (entity is SpotEntity)
                {
                    PointF center = CanvasCoordinate((entity as SpotEntity).Position);
                    RectangleF rect = new RectangleF(new PointF(center.X - SpotSize / 2, center.Y - SpotSize / 2), new Size(SpotSize, SpotSize));
                    g.FillEllipse(sb, rect);
                    g.DrawEllipse(pn, rect);
                }
                else if (entity is LinearEntity)
                {
                    var poly = (entity as LinearEntity).Alignment;
                    Point[] pts = poly.Points.Select(x => DisplayManager.Current.CanvasCoordinate(x)).ToArray();
                    DisplayManager.Current.g.DrawLines(pen, pts);
                }
                else if (entity is RegionEntity)
                {
                    var poly = (entity as RegionEntity).Domain;
                    Point[] pts = poly.Points.Select(x => DisplayManager.Current.CanvasCoordinate(x)).ToArray();
                    DisplayManager.Current.g.DrawPolygon(pen, pts);
                }
            }
        }

        private List<Action<Graphics>> _additionalPaintings = new List<Action<Graphics>>();
        public List<Action<Graphics>> AdditionalPaintings { get { return _additionalPaintings; } }

        internal void PaintAdditional()
        {
            AdditionalPaintings.ForEach(x => x(g));
        }

        public SpotEntity DetectSpotHover(int col, int row)
        {
            foreach (var spot in _cityModel.CitySpots)
            {
                PointF center = CanvasCoordinate(spot.Position);
                if ((center.X - col) * (center.X - col) + (center.Y - row) * (center.Y - row) < SpotSize * SpotSize)
                {
                    return spot;
                }
            }
            return null;
        }

        public CityParcel DetectParcelHover(int col, int row)
        {
            List<CityParcel> temp = new List<CityParcel>();
            Point2D p = CityCoordinate(col, row);
            foreach (var parcel in _cityModel.Parcels)
            {
                if (parcel.Domain.IsPointIn(p))
                {
                    temp.Add(parcel);
                }
            }
            if (temp.Count > 0)
            {
                double minArea = temp.Min(x => x.Domain.Area);
                return temp.First(x => x.Domain.Area == minArea);
            }
            return null;
        }

        public void AddSpot(CityEntityType type, Point2D pos)
        {
            SpotEntity spot = new SpotEntity(type);
            spot.Position = pos;
            spot.ServingRadius = 600.0;
            spot.Coefficient = 1200.0;

            _cityModel.CitySpots.Add(spot);
            _cityModel.Factors.Add(spot);
        }

        public void AddLinear(Polyline alignment)
        {
            LinearEntity linear = new LinearEntity();
            linear.Alignment = alignment;
            linear.BufferSize = 600.0;
            linear.Coefficient = 1200.0;

            _cityModel.CityLinears.Add(linear);
            _cityModel.Factors.Add(linear);
        }

        public void RemoveSpot(SpotEntity spot)
        {
            _cityModel.CitySpots.Remove(spot);
            _cityModel.Factors.Remove(_cityModel.Factors.Single(x => (x as SpotEntity).Properties == spot.Properties)); // 注意：这只是一个引用比较 mod 20110801
        }

        public Polyline DetectRoadDiv(int col, int row, out double dist, out Point2D foot)
        {
            Point2D p = CityCoordinate(col, row);
            var segList = PathModel.divPolys.SelectMany(x => x.GetAllSegs()).Select(x => new LineSegPointRelation(x, p)).Where(x => x.Inner);
            if (segList.Count() == 0)
            {
                dist = 0;
                foot = new Point2D(0, 0);
                return null;
            }
            double minDist = segList.Min(x => x.Dist);
            dist = minDist;
            foot = segList.First(x => x.Dist == minDist).Foot;
            return PathModel.divPolys.First(x => x.GetAllSegs().Where(y => new LineSegPointRelation(y, p).Inner).Select(y => new LineSegPointRelation(y, p).Dist).Contains(minDist));
        }

        public Polyline DetectRoadDiv(int col, int row)
        {
            if (PathModel.divPolys.Count == 0) // mod 20110802
            {
                return null;
            }

            Point2D p = CityCoordinate(col, row);
            double minDist = PathModel.divPolys.Min(x => x.DistToPoint(p));
            if (minDist < Scale * 5)
            {
                return PathModel.divPolys.First(x => x.DistToPoint(p) == minDist);
            }
            else
            {
                return null;
            }
        }

        public CityRoad DetectRoad(int col, int row)
        {
            Polyline roadDiv = DetectRoadDiv(col, row);
            if (roadDiv != null)
            {
                foreach (var road in _cityModel.Roads)
                {
                    if (roadDiv.IsSubOf(road.Alignment))
                    {
                        return road;
                    }
                }
            }
            return null;
        }
    }
}
