using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Geometry;
using TongJi.Geometry3D;

using Poly2Tri.Triangulation;
using Poly2Tri.Triangulation.Delaunay;
using Poly2Tri.Triangulation.Delaunay.Sweep;
using Poly2Tri.Triangulation.Sets;

namespace TongJi.Gis.Topography
{
    public class TIN
    {
        public Dictionary<Point2D, double> Points { get; private set; }
        public List<Triangle3> Triangles { get; private set; }

        public TIN(List<Point3D> points)
        {
            Points = new Dictionary<Point2D, double>();
            points.ForEach(p => Points[p.ToPoint2D()] = p.z);
            Triangles = new List<Triangle3>();

            Delaunay();
        }

        private void Delaunay()
        {
            var pts = Points.Select(p => new TriangulationPoint(p.Key.x, p.Key.y)).ToList();
            PointSet set = new PointSet(pts);

            DTSweepContext tcx = new DTSweepContext();
            tcx.PrepareTriangulation(set);
            DTSweep.Triangulate(tcx);

            Triangles.Clear();
            foreach (DelaunayTriangle triangle in set.Triangles)
            {
                List<Point3D> tri = new List<Point3D>();
                foreach (TriangulationPoint p in triangle.Points)
                {
                    Point2D p2d = new Point2D(p.X, p.Y);
                    tri.Add(new Point3D(p.X, p.Y, Points[p2d]));
                }
                Triangles.Add(new Triangle3(tri[0], tri[1], tri[2]));
            }
        }

        public double InterpolateZ(Point2D p)
        {
            foreach (var tri in Triangles)
            {
                if (tri.ToTriangle2().IsPointIn(p))
                {
                    return Algorithms.TriangleInterpolate(p.x, p.y, tri.A, tri.B, tri.C);
                }
            }
            return 0;
        }

        public List<Point3D> GetDEMPoints(double gridSize)
        {
            var extents = new Extent2D(Points.Keys);
            var demPoints = new Dictionary<Point2D, double>();
            var sampleGrid = new SampleGrid(extents, gridSize);
            for (double x = extents.min.x; x < extents.max.x; x += gridSize)
            {
                for (double y = extents.min.y; y < extents.max.y; y += gridSize)
                {
                    demPoints[new Point2D(x, y)] = 0;
                }
            }
            //var p2ds = demPoints.Keys.ToList();
            foreach (var tri in Triangles)
            {
                var tri2 = tri.ToTriangle2();
                var pts = sampleGrid.GetPointsInExtents(new Extent2D(tri2.P));
                foreach (var p in pts)
                {
                    if (tri2.IsPointIn(p))
                    {
                        demPoints[p] = Algorithms.TriangleInterpolate(p.x, p.y, tri.A, tri.B, tri.C);
                    }
                }
            }
            return demPoints.Select(p => new Point3D(p.Key.x, p.Key.y, p.Value)).ToList();
        }

        private static List<Point2D> GetPointsInExtents(IEnumerable<Point2D> points, Extent2D extents)
        {
            return points.Where(p => extents.IsPointIn(p)).ToList();
        }
    }

    public class SampleGrid
    {
        public List<double> XS { get; private set; }
        public List<double> YS { get; private set; }
        public Extent2D Extents { get; private set; }

        public SampleGrid(Extent2D extents, double cellSize)
        {
            XS = new List<double>();
            YS = new List<double>();
            for (double x = extents.min.x; x < extents.max.x; x += cellSize)
            {
                XS.Add(x);
            }
            for (double y = extents.min.y; y < extents.max.y; y += cellSize)
            {
                YS.Add(y);
            }
            Extents = new Extent2D(extents.min, new Point2D(XS.Last(), YS.Last()));
        }

        public List<Point2D> GetPointsInExtents(Extent2D extents)
        {
            var xs = XS.Where(x => x >= extents.min.x && x <= extents.max.x).ToList();
            var ys = YS.Where(y => y >= extents.min.y && y <= extents.max.y).ToList();
            return BuildGrid(xs, ys);
        }

        public static List<Point2D> BuildGrid(List<double> xs, List<double> ys)
        {
            return xs.SelectMany(x => ys.Select(y => new Point2D(x, y)).ToList()).ToList();
        }
    }
}
