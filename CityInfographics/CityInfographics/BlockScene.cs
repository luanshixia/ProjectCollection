using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

using Poly2Tri.Triangulation;
using Poly2Tri.Triangulation.Delaunay;
using Poly2Tri.Triangulation.Delaunay.Sweep;
using Poly2Tri.Triangulation.Sets;

using TongJi.Geometry;
using TongJi.Geometry3D;
using TongJi.Gis;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using MeshBuilder = HelixToolkit.Wpf.MeshBuilder;

namespace CityInfographics
{
    public class BlockScene
    {
        public HelixViewport3D Viewport { get; private set; }
        public List<IFeature> BasePaths { get; private set; }
        public List<ExtrudedVisual3D> Blocks { get; private set; }

        public const double FloorHeight = 3.0;

        public BlockScene(HelixViewport3D viewport, List<IFeature> basePaths)
        {
            Viewport = viewport;
            BasePaths = basePaths;
            Blocks = new List<ExtrudedVisual3D>();

            Init();
        }

        private void Init()
        {
            foreach (IFeature basePath in BasePaths)
            {
                double height = basePath["f"].TryParseToDouble() * FloorHeight;
                ExtrudedVisual3D block = BuildBlock(basePath, height);
                Blocks.Add(block);
                Viewport.Children.Add(block);
                //Viewport.Children.Add(Planar(basePath));
            }
            Viewport.ZoomExtents();
        }

        private static ExtrudedVisual3D BuildBlock(IFeature basePath, double height)
        {
            TongJi.Geometry.Polygon poly = new TongJi.Geometry.Polygon(basePath.GeoData);
            ExtrudedVisual3D result = new ExtrudedVisual3D { IsSectionClosed = true, IsPathClosed = false, UpVector = new Vector3D(0, 0, 1) };
            result.Section = new PointCollection(poly.Points.Select(p => new Point(p.x, p.y)));
            result.Path = new Point3DCollection(new Point3D[] { new Point3D(0, 0, 0), new Point3D(0, 0, height) });
            return result;
        }

        private static List<Triangle2> DelaunayOfPolygon(List<Point2D> points)
        {
            var pts = points.Select(p => new Poly2Tri.Triangulation.Polygon.PolygonPoint(p.x, p.y)).ToList();
            Poly2Tri.Triangulation.Polygon.Polygon set = new Poly2Tri.Triangulation.Polygon.Polygon(pts);

            DTSweepContext tcx = new DTSweepContext();
            tcx.PrepareTriangulation(set);
            DTSweep.Triangulate(tcx);

            var triangles = new List<Triangle2>();
            foreach (DelaunayTriangle triangle in set.Triangles)
            {
                List<Point2D> tri = new List<Point2D>();
                foreach (TriangulationPoint p in triangle.Points)
                {
                    tri.Add(new Point2D(p.X, p.Y));
                }
                triangles.Add(new Triangle2(tri[0], tri[1], tri[2]));
            }
            return triangles;
        }

        private static ModelVisual3D Planar(IFeature basePath)
        {
            TongJi.Geometry.Polyline curve = new TongJi.Geometry.Polyline(basePath.GeoData);
            if (curve.AlgebraicArea < 0)
            {
                curve = curve.ReversePoints();
            }
            var points = curve.Points.ToList();
            if (points.Last() == points.First())
            {
                points.RemoveAt(points.Count - 1);
            }
            var triangles = DelaunayOfPolygon(points);

            MeshBuilder mb = new MeshBuilder();
            foreach (var tri in triangles)
            {
                var a = new Point3D(tri.A.x, tri.A.y, 0);
                var b = new Point3D(tri.B.x, tri.B.y, 0);
                var c = new Point3D(tri.C.x, tri.C.y, 0);
                mb.AddTriangle(a, b, c);
            }
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = new GeometryModel3D(mb.ToMesh(), MaterialHelper.CreateMaterial(Colors.Blue));
            return visual;
        }
    }
}
