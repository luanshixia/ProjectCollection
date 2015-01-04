using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using TongJi.Geometry;

namespace TongJi.City
{
    public interface IValueUnit
    {
        double Value { get; }
        //Point2D Position { get; }
        void UpdateValue();
        void Paint(Color color);
    }

    public class GridCell : IValueUnit
    {
        protected double _value;
        public double Value { get { return _value; } }

        protected Point2D _center;
        protected double _size;

        public Point2D Position { get { return _center; } }
        public int Row { get; protected set; }
        public int Col { get; protected set; }

        public GridCell(Point2D center, double size, int col, int row)
        {
            _center = center;
            _size = size;
            Col = col;
            Row = row;

            this.UpdateValue();
        }

        public virtual void UpdateValue()
        {
            _value = DisplayManager.Current.CityModel.GetValue(_center);
        }

        public void Paint(Color color)
        {
            Point2D pt1 = new Point2D(_center.x - _size / 2, _center.y + _size / 2);
            float side = (float)_size / (float)DisplayManager.Current.Scale;
            RectangleF rect = new RectangleF(DisplayManager.Current.CanvasCoordinateF(pt1), new SizeF(side, side));
            rect.Inflate(1, 1);
            if (TaskPanel.Current.cbShowGridLine.Checked)
            {
                rect.Inflate(-1.2f, -1.2f);
            }
            DisplayManager.Current.g.FillRectangle(new SolidBrush(color), rect);
        }
    }

    public class CoverRangeCell : GridCell
    {
        public CoverRangeCell(Point2D center, double size, int col, int row)
            : base(center, size, col, row)
        {
        }

        public override void UpdateValue()
        {
            double lower = Convert.ToDouble(TaskPanel.Current.txtMin.Text);
            double upper = Convert.ToDouble(TaskPanel.Current.txtMax.Text);
            double d = (upper - lower) / DisplayManager.Current.CityModel.CitySpots.Count;
            _value = lower + d * DisplayManager.Current.CityModel.GetSpotBelonging(_center);
        }
    }

    public class GridCellGroup : IValueUnit
    {
        // private fields
        private double _value;
        private List<GridCell> _cells;

        // accessors
        public List<GridCell> Cells { get { return _cells; } }

        // constructor
        public GridCellGroup(List<GridCell> cells, double value)
        {
            _cells = cells;
            _value = value;
        }

        // IValueUnit members
        public double Value { get { return _value; } }

        public void UpdateValue()
        {
        }

        public void Paint(Color color)
        {
            _cells.ForEach(x => x.Paint(color));
        }

        // additional functions

    }

    public class ParcelUnit : IValueUnit
    {
        protected double _value;
        public double Value { get { return _value; } }

        protected Point2D _center;
        protected CityParcel _parcel;

        public Point2D Position { get { return _center; } }

        public ParcelUnit(CityParcel parcel)
        {
            _parcel = parcel;
            _center = parcel.Domain.Centroid;

            this.UpdateValue();
        }

        public virtual void UpdateValue()
        {
            _value = DisplayManager.Current.CityModel.GetValue(_center);
        }

        public void Paint(Color color)
        {
            Point[] pts = _parcel.Domain.Points.Select(x => DisplayManager.Current.CanvasCoordinate(x)).ToArray();
            DisplayManager.Current.g.FillPolygon(new SolidBrush(color), pts);
        }
    }

    public class ParcelIndexUnit : ParcelUnit
    {
        private string _prop;

        public ParcelIndexUnit(CityParcel parcel, string prop)
            : base(parcel)
        {
            _prop = prop;
        }

        public override void UpdateValue()
        {
            try
            {
                _value = Convert.ToDouble(_parcel.Properties[_prop]);
            }
            catch
            {
                _value = 0;
            }
        }
    }

    public static class ValueBuffer
    {
        private static List<IValueUnit> _units = new List<IValueUnit>();
        public static List<IValueUnit> Units { get { return _units; } }

        // ---WARNING--- these 2 properties can only be called if you use grid cells
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        public static IValueUnit GetGridCell(int col, int row)
        {
            return _units[col * Height + row];
        }

        public static IValueUnit GetGridCell(List<IValueUnit> units, int col, int row)
        {
            return units[col * Height + row];
        }

        public static void BuildGrid()
        {
            _units = GetGrid();
        }

        public static List<IValueUnit> GetGrid()
        {
            List<IValueUnit> units = new List<IValueUnit>();

            float interval = Convert.ToSingle(TaskPanel.Current.numGridSize.Value);
            var pts = DisplayManager.Current.CityModel.Extents.GetGrid2d(interval);
            Width = pts.GetLength(0);
            Height = pts.GetLength(1);
            for (int i = 0; i < pts.GetLength(0); i++)
            {
                for (int j = 0; j < pts.GetLength(1); j++)
                {
                    units.Add(new GridCell(pts[i, j], interval, i, j));
                }
            }

            return units;
        }

        public static void BuildCoverRangeGrid()
        {
            _units = GetCoverRangeGrid();
        }

        public static List<IValueUnit> GetCoverRangeGrid()
        {
            List<IValueUnit> units = new List<IValueUnit>();

            float interval = Convert.ToSingle(TaskPanel.Current.numGridSize.Value);
            var pts = DisplayManager.Current.CityModel.Extents.GetGrid2d(interval);
            Width = pts.GetLength(0);
            Height = pts.GetLength(1);
            for (int i = 0; i < pts.GetLength(0); i++)
            {
                for (int j = 0; j < pts.GetLength(1); j++)
                {
                    units.Add(new CoverRangeCell(pts[i, j], interval, i, j));
                }
            }

            return units;
        }

        public static void BuildParcels()
        {
            _units.Clear();

            DisplayManager.Current.CityModel.Parcels.ForEach(x => _units.Add(new ParcelUnit(x)));
        }

        public static void BuildCoverRanges()
        {
            _units.Clear();

            Dictionary<SpotEntity, List<GridCell>> groups = new Dictionary<SpotEntity, List<GridCell>>();
            DisplayManager.Current.CityModel.CitySpots.ForEach(x => groups.Add(x, new List<GridCell>()));

            float interval = Convert.ToSingle(TaskPanel.Current.numGridSize.Value);
            List<Point2D> pts = DisplayManager.Current.CityModel.Extents.GetGrid(interval);
            foreach (var pt in pts)
            {
                double minDist = DisplayManager.Current.CityModel.CitySpots.Min(x => x.Position.DistTo(pt));
                var spot = DisplayManager.Current.CityModel.CitySpots.First(x => x.Position.DistTo(pt) == minDist);
                groups[spot].Add(new GridCell(pt, interval, 0, 0));
            }

            double lower = Convert.ToDouble(TaskPanel.Current.txtMin.Text);
            double upper = Convert.ToDouble(TaskPanel.Current.txtMax.Text);
            double d = (upper - lower) / groups.Count;
            foreach (var group in groups)
            {
                _units.Add(new GridCellGroup(group.Value, lower));
                lower += d;
            }
        }

        public static void Build()
        {
        }

        public static void UpdateValues()
        {
            _units.ForEach(x => x.UpdateValue());
        }

        public static List<Point> GetBinarizedPixels(List<GridCell> cells, Func<GridCell, bool> predicate)
        {
            return cells.Where(predicate).Select(x => new Point(x.Col, x.Row)).ToList();
        }
    }

    public static class ValueMode
    {
        public const int ParcelPrice = 0;
        public const int GridPrice = 1;
        public const int Voronoi = 2;
    }
}
