using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

using TongJi.Geometry;
using Point3D = System.Windows.Media.Media3D.Point3D;

namespace CityInfographics
{
    public class ChartScene
    {
        public HelixViewport3D Viewport { get; private set; }
        public int M { get; private set; }
        public int N { get; private set; }
        public List<BoxVisual3D> Boxes { get; private set; }

        public const double BoxSize = 10;
        public const double GridSize = 20;
        public const double BoxInitHeight = 50;

        public ChartScene(HelixViewport3D viewport, int m, int n)
        {
            Viewport = viewport;
            M = m;
            N = n;
            Boxes = new List<BoxVisual3D>();

            Init();
        }

        private void Init()
        {
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    BoxVisual3D box = new BoxVisual3D { Center = new Point3D(i * GridSize, j * GridSize, BoxInitHeight / 2), Width = BoxSize, Length = BoxSize, Height = BoxInitHeight };
                    Boxes.Add(box);
                    Viewport.Children.Add(box);
                }
            }
            Viewport.ZoomExtents(1000);
        }

        public void UpdateBoxes(double[] heights)
        {
            for (int i = 0; i < Boxes.Count; i++)
            {
                BoxVisual3D box = Boxes[i];
                Point3D center = box.Center;
                center.Z = heights[i] / 2;
                box.Center = center;
                box.Height = heights[i];
            }
        }
    }

    public class ChartScene2
    {
        public HelixViewport3D Viewport { get; private set; }
        public List<Point2D> Points { get; private set; }
        public List<BoxVisual3D> Boxes { get; private set; }

        private TongJi.Gis.Display.MultiColorGradientTheme _theme = new TongJi.Gis.Display.MultiColorGradientTheme("", 0, 500);

        public double GridSize { get; set; }
        public const double BoxSizeRatio = 0.6;
        public double BoxSize
        {
            get
            {
                return BoxSizeRatio * GridSize;
            }
        }
        public const double BoxInitHeight = 50;

        public ChartScene2(HelixViewport3D viewport, List<Point2D> points, double gridSize)
        {
            Viewport = viewport;
            Points = points;
            Boxes = new List<BoxVisual3D>();
            GridSize = gridSize;

            _theme.Stops.Clear();
            _theme.AddStop(0, System.Windows.Media.Colors.Cyan);
            _theme.AddStop(200, System.Windows.Media.Colors.Yellow);
            _theme.AddStop(500, System.Windows.Media.Colors.Red);

            Init();
        }

        private void Init()
        {
            foreach (Point2D p in Points)
            {
                BoxVisual3D box = new BoxVisual3D { Center = new Point3D(p.x, p.y, BoxInitHeight / 2), Width = BoxSize, Length = BoxSize, Height = BoxInitHeight, Fill = System.Windows.Media.Brushes.Cyan };
                Boxes.Add(box);
                Viewport.Children.Add(box);
            }
            var center = new TongJi.Geometry.Extent2D(Points).Center;
            Viewport.LookAt(new System.Windows.Media.Media3D.Point3D(center.x, center.y, 0), new System.Windows.Media.Media3D.Vector3D(0, 1, -0.8), 500);
            Viewport.ZoomExtents(1000);
        }

        public void UpdateBoxes(double[] heights)
        {
            double maxHeight = 400;
            for (int i = 0; i < Boxes.Count; i++)
            {
                BoxVisual3D box = Boxes[i];
                Point3D center = box.Center;
                double height = heights[i] > maxHeight ? maxHeight : heights[i];
                center.Z = height / 2;
                box.Center = center;
                box.Height = height;
                box.Fill = new System.Windows.Media.SolidColorBrush(_theme.GetColorByValue(height));
            }
        }
    }

    public class EnergyVisualizationModel
    {
    }
}
