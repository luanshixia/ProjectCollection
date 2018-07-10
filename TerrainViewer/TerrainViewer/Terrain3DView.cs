using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using HelixToolkit.Wpf;

namespace TongJi.Drawing.Viewer3D
{
    public enum TextureType
    {
        Elevation,
        Slope,
    }

    public struct Road
    {
        public Point3D[] Points;
        public double Width;
    }

    public class RoadModel
    {
        public Point3D Offset { get; set; }
        public Color RoadColor { get; set; }

        public GeometryModel3D createRoadModel(Road road)
        {
            return createRoadModel(road.Points, road.Width);
        }

        public GeometryModel3D createRoadModel(Point3D[] points, double roadWidth)
        {
            // 转为相对坐标
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X -= Offset.X;
                points[i].Y -= Offset.Y;
                points[i].Z -= Offset.Z;
            }

            Point3D[,] RoadPts = createRoadMesh(points, roadWidth);
            // 构造道路面的mesh
            MeshBuilder mb = new MeshBuilder(false, false);
            mb.AddRectangularMesh(RoadPts, closed0: false, closed1: false);
            var mesh = mb.ToMesh();
            var model = new GeometryModel3D();
            model.Geometry = mesh;
            var material = MaterialHelper.CreateMaterial(RoadColor);
            model.Material = material;
            model.BackMaterial = material;
            return model;
        }

        private Point3D[,] createRoadMesh(Point3D[] points, double roadWidth)
        {
            if (points.Length <= 1)
            {
                throw new ArgumentException("道路中心线点数至少为2");
            }
            Point3D[,] retPts = new Point3D[3, points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                double offsetX, offsetY;  // 平行线的坐标偏移量

                if (i == 0)
                {
                    double a = getAngle(points[0].X, points[0].Y, points[1].X, points[1].Y);
                    offsetX = -roadWidth * Math.Sin(a);
                    offsetY = roadWidth * Math.Cos(a);
                }
                else if (i == points.Length - 1)
                {
                    double a = getAngle(points[i - 1].X, points[i - 1].Y, points[i].X, points[i].Y);
                    offsetX = -roadWidth * Math.Sin(a);
                    offsetY = roadWidth * Math.Cos(a);

                }
                else
                {
                    double a1 = getAngle(points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y);
                    double a2 = getAngle(points[i - 1].X, points[i - 1].Y, points[i].X, points[i].Y);
                    offsetX = roadWidth * (Math.Cos(a1) + Math.Cos(a2)) / (Math.Sin(a2 - a1));
                    offsetY = roadWidth * (Math.Sin(a1) + Math.Sin(a2)) / (Math.Sin(a2 - a1));
                }
                retPts[0, i].X = points[i].X + offsetX;
                retPts[0, i].Y = points[i].Y + offsetX;
                retPts[0, i].Z = points[i].Z;

                retPts[1, i] = points[i];

                retPts[2, i].X = points[i].X - offsetX;
                retPts[2, i].Y = points[i].Y - offsetX;
                retPts[2, i].Z = points[i].Z;
            }
            return retPts;
        }

        /// <summary>
        /// 求两点连线的方位角
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private double getAngle(double x1, double y1, double x2, double y2)
        {
            double a = Math.Atan2(y2 - y1, x2 - x1);
            if (a < 0)
            {
                a = a + 2 * Math.PI;
            }
            return a;
        }
    }

    public class Terrain3DView : TerrainVisual3D
    {
        private TerrainModel _model;
        private List<Road> _roads = new List<Road>();
        private List<ModelVisual3D> _roadChilds = new List<ModelVisual3D>();
        private readonly ModelVisual3D _visualChild;
        private double[] _data;

        public LinearGradientBrush ElevationBrush { get; set; }
        public LinearGradientBrush SlopeBrush { get; set; }
        public double VerticalMagnification { get; set; }
        public TextureType TextureType { get; set; }
        public Color RoadColor { get; set; }

        public bool HasData
        {
            get
            {
                if (_data == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public List<ModelVisual3D> RoadChilds
        {
            get
            {
                return _roadChilds;
            }
        }

        public Terrain3DView()
        {
            _visualChild = new ModelVisual3D();
            Children.Add(_visualChild);
            VerticalMagnification = 1;
            RoadColor = Colors.Green;
        }

        public void LoadData(double[] elevations, int width, double left, double right, double top, double bottom)
        {
            _model = new TerrainModel();
            _data = elevations.ToArray();
            _model.Width = width;
            _model.Height = elevations.Length / _model.Width;
            _model.Left = left;
            _model.Right = right;
            _model.Top = top;
            _model.Bottom = bottom;
        }

        public void AddRoad(Point3D[] points, double roadWidth)
        {
            Road road = new Road();
            road.Points = (Point3D[])points.Clone();
            road.Width = roadWidth;
            _roads.Add(road);
            ModelVisual3D roadChild = new ModelVisual3D();
            _roadChilds.Add(roadChild);
            Children.Add(roadChild);
            //ModelVisual3D roadChild = new ModelVisual3D();
            ////_roadChilds.Add(roadChild);
            //Children.Add(roadChild);
            //roadChild.Content = createRoadModel(points, roadWidth);
        }

        public void Update()
        {
            // 更新地形
            if (_data == null)
            {
                return;
            }
            _model.Data = new double[_data.Length];
            for (int i = 0; i < _model.Height; i++)
            {
                for (int j = 0; j < _model.Width; j++)
                {
                    _model.Data[i * _model.Width + j] = _data[(_model.Height - i - 1) * _model.Width + j] * VerticalMagnification;
                }
            }
            _model.MinimumZ = _model.Data.Min();
            _model.MaximumZ = _model.Data.Max();
            switch (TextureType)
            {
                case TextureType.Elevation:
                    var et = new ElevationTexture();
                    if (ElevationBrush != null)
                    {
                        et.Brush = ElevationBrush;
                    }
                    _model.Texture = et;
                    break;
                case TextureType.Slope:
                    var st = new SlopeTexture(18);
                    if (SlopeBrush != null)
                    {
                        st.Brush = SlopeBrush;
                    }
                    _model.Texture = st;
                    break;
            }
            _visualChild.Content = _model.CreateModel(2);

            // 更新道路
            for (int i = 0; i < _roads.Count; i++)
            {
                var roadModel = new RoadModel
                {
                    RoadColor = RoadColor,
                    Offset = _model.Offset
                };

                var road = new Road
                {
                    Width = _roads[i].Width,
                    Points = new Point3D[_roads[i].Points.Length]
                };

                for (int j = 0; j < _roads[i].Points.Length; j++)
                {
                    road.Points[j] = _roads[i].Points[j];
                    road.Points[j].Z = _roads[i].Points[j].Z * VerticalMagnification;
                }
                _roadChilds[i].Content = roadModel.createRoadModel(road);
            }
        }
    }

    /// <summary>
    /// Texture by the elevation of the terrain.
    /// </summary>
    public class ElevationTexture : TerrainTexture
    {
        public Brush Brush { get; set; }

        public ElevationTexture()
        {
            this.Brush = GradientBrushes.Rainbow;
        }

        public ElevationTexture(int gradientSteps)
        {
            if (gradientSteps > 0)
            {
                this.Brush = BrushHelper.CreateSteppedGradientBrush(GradientBrushes.Rainbow, gradientSteps);
            }
            else
            {
                this.Brush = GradientBrushes.Rainbow;
            }
        }

        public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
            base.TextureCoordinates = new PointCollection(mesh.Positions
                .Select(p =>
                {
                    double x = p.X + model.Offset.X;
                    double y = p.Y + model.Offset.Y;
                    double u = (GetElevation(model, x, y) - model.MinimumZ) / (model.MaximumZ - model.MinimumZ);
                    u = u > 1 ? 1 : u < 0 ? 0 : u;
                    return new Point(u, u);
                }));

            base.Material = MaterialHelper.CreateMaterial(Brush);
        }

        private static double GetElevation(TerrainModel model, double x, double y)
        {
            double colWidth = (model.Right - model.Left) / (model.Width - 1);
            int col = (int)((x - model.Left) / colWidth);
            double rowWidth = (model.Top - model.Bottom) / (model.Height - 1);
            int row = (int)(model.Height - 1 - (y - model.Bottom) / rowWidth);
            if (row == model.Height - 1)
            {
                row--;
            }
            if (col == model.Width - 1)
            {
                col--;
            }

            double lbElevation = model.Data[model.Width * row + col];
            double ltElevation = model.Data[model.Width * (row + 1) + col];
            double rtElevation = model.Data[model.Width * (row + 1) + col + 1];
            double rbElevation = model.Data[model.Width * row + col + 1];
            double u = (x - model.Left) / colWidth - col;
            double v = model.Height - 1 - (y - model.Bottom) / rowWidth - row;
            return (1 - u) * (1 - v) * lbElevation + (1 - u) * v * ltElevation + u * (1 - v) * rbElevation + u * v * rtElevation;
        }
    }
}
