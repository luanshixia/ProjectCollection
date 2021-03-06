﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Geometry;

namespace TongJi.Geometry3D
{
    public class Mesh
    {
        // Properties with storage

        public List<Point3D> Vertices { get; private set; }
        public List<Vector3D> Normals { get; private set; }
        public List<Point2D> TextureCoordinates { get; private set; }
        public List<Triangle> Triangles { get; private set; }

        // Constructor

        public Mesh()
        {
            Vertices = new List<Point3D>();
            Normals = new List<Vector3D>();
            TextureCoordinates = new List<Point2D>();
            Triangles = new List<Triangle>();
        }

        // Helper properties

        public Rect3D Bounds
        {
            get
            {
                Rect3D rect = Rect3D.Null;
                Vertices.ForEach(x => rect.AddPoint(x));
                return rect;
            }
        }

        // Methods

        public Mesh Transform(Matrix4 matrix)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = Vertices[i].Transform(matrix);
            }
            Matrix4 rotation = matrix.ExtractRotation();
            for (int i = 0; i < Normals.Count; i++)
            {
                Normals[i] = Normals[i].Transform(rotation);
            }
            return this;
        }
    }

    public class Triangle
    {
        public int V0 { get; set; }
        public int V1 { get; set; }
        public int V2 { get; set; }

        public Triangle(int v0, int v1, int v2)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
        }
    }

    public class Triangle2
    {
        public Point2D A;
        public Point2D B;
        public Point2D C;

        public Triangle2(Point2D a, Point2D b, Point2D c)
        {
            A = a;
            B = b;
            C = c;
        }

        public double Area
        {
            get
            {
                return Math.Abs((B - A).ScalarCross(C - A)) / 2;
            }
        }

        public bool IsPointIn(Point2D p)
        {
            Triangle2 ta = new Triangle2(p, B, C);
            Triangle2 tb = new Triangle2(p, A, C);
            Triangle2 tc = new Triangle2(p, A, B);
            return Math.Abs(ta.Area + tb.Area + tc.Area - Area) < 1e-6;
        }

        public IEnumerable<Point2D> P
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
            }
        }
    }

    public class Triangle3
    {
        public Point3D A;
        public Point3D B;
        public Point3D C;

        public Triangle3(Point3D a, Point3D b, Point3D c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Triangle2 ToTriangle2()
        {
            return new Triangle2(A.ToPoint2D(), B.ToPoint2D(), C.ToPoint2D());
        }
    }

    public class MeshBuilder
    {
        public List<Point3D> Vertices = new List<Point3D>();
        public List<Triangle> Faces = new List<Triangle>();
        public List<Vector3D> Normals = new List<Vector3D>();
        public List<Point2D> Texcoords = new List<Point2D>();

        private MeshBuilder()
        {
        }

        public Mesh ToMesh()
        {
            Mesh mesh = new Mesh();
            mesh.Vertices.AddRange(Vertices);
            mesh.Triangles.AddRange(Faces);
            mesh.Normals.AddRange(Normals);
            mesh.TextureCoordinates.AddRange(Texcoords);

            //mesh.CombineIdenticalVertices();
            return mesh;
        }

        private void FlipFaces()
        {
            foreach (var face in Faces)
            {
                int temp = face.V0;
                face.V0 = face.V1;
                face.V1 = temp;
            }
        }

        private void AppendTriangle(int v0, int v1, int v2)
        {
            Faces.Add(new Triangle(v0, v1, v2));
        }

        private void AppendQuad(int v0, int v1, int v2, int v3)
        {
            Faces.Add(new Triangle(v0, v1, v2));
            Faces.Add(new Triangle(v0, v2, v3));
        }

        public void AddTriangle(Point3D p0, Point3D p1, Point3D p2)
        {
            int i0 = Vertices.Count();

            Vertices.Add(p0);
            Vertices.Add(p1);
            Vertices.Add(p2);

            this.AppendTriangle(i0, i0 + 1, i0 + 2);
        }

        public void AddQuad(Point3D p0, Point3D p1, Point3D p2, Point3D p3)
        {
            int i0 = Vertices.Count();

            Vertices.Add(p0);
            Vertices.Add(p1);
            Vertices.Add(p2);
            Vertices.Add(p3);

            this.AppendQuad(i0, i0 + 1, i0 + 2, i0 + 3);
        }

        public void Append(MeshBuilder mb)
        {
            int i0 = this.Vertices.Count();
            foreach (Point3D p in mb.Vertices)
            {
                this.Vertices.Add(p);
            }
            foreach (Triangle f in mb.Faces)
            {
                this.AppendTriangle(f.V0 + i0, f.V1 + i0, f.V2 + i0);
            }
            if (mb.Normals.Count() > 0)
            {
                foreach (var n in mb.Normals)
                {
                    this.Normals.Add(n);
                }
            }
            if (mb.Texcoords.Count() > 0)
            {
                foreach (var t in mb.Texcoords)
                {
                    this.Texcoords.Add(t);
                }
            }
        }

        public static MeshBuilder FromMesh(Mesh mesh)
        {
            MeshBuilder mb = new MeshBuilder();
            mb.Vertices.AddRange(mesh.Vertices);
            mb.Faces.AddRange(mesh.Triangles);
            mb.Normals.AddRange(mesh.Normals);
            mb.Texcoords.AddRange(mesh.TextureCoordinates);
            return mb;
        }

        public static MeshBuilder RectGrid(Point3D[,] points, bool closed0 = false, bool closed1 = false)
        {
            MeshBuilder mb = new MeshBuilder();
            int rows = points.GetUpperBound(0) + 1;
            int columns = points.GetUpperBound(1) + 1;
            int index0 = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    mb.Vertices.Add(points[i, j]);
                }
            }

            mb.MakeRectGridFaces(index0, rows, columns, closed0, closed1);
            return mb;
        }

        public static MeshBuilder ParametricEquation(SurfaceParametricEquation surface, UvSampler vertices, bool closed0 = false, bool closed1 = false)
        {
            var points = vertices.EvaluateSurface(surface);
            return RectGrid(points, closed0, closed1);
        }

        public static MeshBuilder ExtrudeSmooth(TongJi.Geometry.Polyline curve, double height)
        {
            var points0 = curve.Points.Select(p => new Point3D(p.x, p.y, 0)).ToList();
            var points1 = curve.Points.Select(p => new Point3D(p.x, p.y, height)).ToList();
            int pointCount = curve.Count;

            Point3D[,] points = new Point3D[pointCount, 2];
            Enumerable.Range(0, pointCount).ToList().ForEach(i => points[i, 0] = points0[i]);
            Enumerable.Range(0, pointCount).ToList().ForEach(i => points[i, 1] = points1[i]);

            return MeshBuilder.RectGrid(points);
        }

        public static MeshBuilder Extrude(TongJi.Geometry.Polyline curve, double height)
        {
            if (curve.AlgebraicArea < 0)
            {
                curve.Reverse();
            }
            var points0 = curve.Points.Select(p => new Point3D(p.x, p.y, 0)).ToList();
            var points1 = curve.Points.Select(p => new Point3D(p.x, p.y, height)).ToList();
            int pointCount = curve.Count;

            MeshBuilder mb = new MeshBuilder();
            for (int i = 0; i < pointCount - 1; i++)
            {
                mb.AddQuad(points0[i], points0[i + 1], points1[i + 1], points1[i]);
            }
            return mb;
        }

        public static MeshBuilder ExtrudeWithCaps(TongJi.Geometry.Polyline curve, double height)
        {
            MeshBuilder mb = MeshBuilder.Extrude(curve, height);
            MeshBuilder cap0 = MeshBuilder.Planar(curve);
            MeshBuilder cap1 = MeshBuilder.Planar(curve);
            if (cap0 != null)
            {
                cap0.FlipFaces();
                cap1.Transform(Matrix4.Translation(0, 0, height));
                mb.Append(cap0);
                mb.Append(cap1);
            }
            return mb;
        }

        public static MeshBuilder Planar(TongJi.Geometry.Polyline curve)
        {
            const double epsilon = 0.001;
            curve.ReducePoints(epsilon);
            var points = curve.Points.ToList();
            if (points.Last().DistTo(points.First()) < epsilon)
            {
                points.RemoveAt(points.Count - 1);
            }
            var triIndices = CuttingEarsTriangulator.Triangulate(points.Select(p => new System.Windows.Point(p.x, p.y)).ToList());
            if (triIndices == null)
            {
                return null;
            }
            MeshBuilder mb = new MeshBuilder();
            for (int i = 0; i < triIndices.Count; i += 3)
            {
                var a = new Point3D(points[triIndices[i]].x, points[triIndices[i]].y, 0);
                var b = new Point3D(points[triIndices[i + 1]].x, points[triIndices[i + 1]].y, 0);
                var c = new Point3D(points[triIndices[i + 2]].x, points[triIndices[i + 2]].y, 0);
                mb.AddTriangle(a, b, c);
            }
            return mb;
        }

        private void MakeRectGridFaces(int index0, int rows, int columns, bool rowsClosed, bool columnsClosed)
        {
            int m2 = rows - 1;
            int n2 = columns - 1;
            if (columnsClosed) m2++;
            if (rowsClosed) n2++;

            for (int i = 0; i < m2; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    int ij00 = index0 + i * columns + j;
                    int ij01 = index0 + i * columns + (j + 1) % columns;
                    int ij10 = index0 + ((i + 1) % rows) * columns + j;
                    int ij11 = index0 + ((i + 1) % rows) * columns + (j + 1) % columns;

                    this.AppendQuad(ij00, ij01, ij11, ij10);
                }
            }
        }

        public void Transform(Matrix4 matrix)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = Vertices[i].Transform(matrix);
            }
            Matrix4 rotation = matrix.ExtractRotation();
            for (int i = 0; i < Normals.Count; i++)
            {
                Normals[i] = Normals[i].Transform(rotation);
            }
        }
    }

    public class Rect3D
    {
        public Point3D min;
        public Point3D max;

        public double XRange { get { return max.x - min.x; } }
        public double YRange { get { return max.y - min.y; } }
        public double ZRange { get { return max.z - min.z; } }
        public double Volume { get { return XRange * YRange * ZRange; } }
        public Point3D Center { get { return min.Move(0.5 * new Vector3D(XRange, YRange, ZRange)); } }

        public bool IsNull { get; private set; }
        public static Rect3D Null { get { return new Rect3D(); } }

        private Rect3D()
        {
            IsNull = true;
        }

        public Rect3D(Point3D p1, Point3D p2)
        {
            min = new Point3D(Math.Min(p1.x, p2.x), Math.Min(p1.y, p2.y), Math.Min(p1.z, p2.z));
            max = new Point3D(Math.Max(p1.x, p2.x), Math.Max(p1.y, p2.y), Math.Max(p1.z, p2.z));
            IsNull = false;
        }

        public static Rect3D operator +(Rect3D e1, Rect3D e2)
        {
            if (e1.IsNull)
            {
                return e2;
            }
            else if (e2.IsNull)
            {
                return e1;
            }
            double minx = Math.Min(e1.min.x, e2.min.x);
            double miny = Math.Min(e1.min.y, e2.min.y);
            double minz = Math.Min(e1.min.z, e2.min.z);
            double maxx = Math.Max(e1.max.x, e2.max.x);
            double maxy = Math.Max(e1.max.y, e2.max.y);
            double maxz = Math.Max(e1.max.z, e2.max.z);
            return new Rect3D(new Point3D(minx, miny, minz), new Point3D(maxx, maxy, maxz));
        }

        // 20111014
        public void AddPoint(Point3D point)
        {
            if (this.IsNull)
            {
                min = point;
                max = point;
                IsNull = false;
            }
            else
            {
                min = new Point3D(Math.Min(min.x, point.x), Math.Min(min.y, point.y), Math.Min(min.z, point.z));
                max = new Point3D(Math.Max(max.x, point.x), Math.Max(max.y, point.y), Math.Max(max.z, point.z));
            }
        }

        // 20111014
        public void Add(Rect3D ext)
        {
            if (this.IsNull)
            {
                min = ext.min;
                max = ext.max;
                IsNull = ext.IsNull;
            }
            else
            {
                if (ext.IsNull)
                {
                    return;
                }
                min = new Point3D(Math.Min(min.x, ext.min.x), Math.Min(min.y, ext.min.y), Math.Min(min.z, ext.min.z));
                max = new Point3D(Math.Max(max.x, ext.max.x), Math.Max(max.y, ext.max.y), Math.Max(max.z, ext.max.z));
            }
        }

        public override string ToString()
        {
            return min.ToString() + "|" + max.ToString();
        }

        public bool IsPointIn(Point3D point)
        {
            return point.x >= min.x && point.x <= max.x && point.y >= min.y && point.y <= max.y && point.z >= min.z && point.z <= max.z;
        }

        public bool IsExtentsIn(Rect3D extents)
        {
            return extents.min.x >= min.x && extents.min.y >= min.y && extents.max.x <= max.x && extents.max.y <= max.y && extents.min.z >= min.z && extents.max.z <= max.z;
        }

        public bool IsExtentsCross(Rect3D extents)
        {
            Rect3D sum = this + extents;
            return (sum.XRange <= this.XRange + extents.XRange) && (sum.YRange <= this.YRange + extents.YRange) && (sum.ZRange <= this.ZRange + extents.ZRange);
        }
    }

    public delegate double UvExpression(double u, double v);

    public class SurfaceParametricEquation
    {
        public UvExpression X { get; private set; }
        public UvExpression Y { get; private set; }
        public UvExpression Z { get; private set; }

        public SurfaceParametricEquation()
        {
            X = (u, v) => u;
            Y = (u, v) => v;
            Z = (u, v) => 0;
        }

        public SurfaceParametricEquation(UvExpression x, UvExpression y, UvExpression z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D Evaluate(double u, double v)
        {
            return new Point3D(X(u, v), Y(u, v), Z(u, v));
        }
    }

    public class Interval
    {
        public double min;
        public double max;
        public double Length { get { return max - min; } }

        public Interval(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        public double[] Sample(int segs)
        {
            double div = this.Length / (segs - 1);
            return Enumerable.Range(0, segs).Select(x => this.min + x * div).ToArray();
        }
    }

    public class UvSampler
    {
        public Interval UDomain { get; private set; }
        public Interval VDomain { get; private set; }
        public int UCount { get; private set; }
        public int VCount { get; private set; }

        public UvSampler(Interval uDomain, Interval vDomain, int uCount, int vCount)
        {
            UDomain = uDomain;
            VDomain = vDomain;
            UCount = uCount;
            VCount = vCount;
        }

        public Point2D[,] GetSamples()
        {
            var uSamples = UDomain.Sample(UCount);
            var vSamples = VDomain.Sample(VCount);

            Point2D[,] samples = new Point2D[UCount, VCount];
            for (int i = 0; i < UCount; i++)
            {
                for (int j = 0; j < VCount; j++)
                {
                    samples[i, j] = new Point2D(uSamples[i], vSamples[j]);
                }
            }

            return samples;
        }

        public Point3D[,] EvaluateSurface(SurfaceParametricEquation surface)
        {
            var samples = GetSamples();
            Point3D[,] points = new Point3D[UCount, VCount];
            for (int i = 0; i < UCount; i++)
            {
                for (int j = 0; j < VCount; j++)
                {
                    var sample = samples[i, j];
                    double u = sample.x;
                    double v = sample.y;
                    points[i, j] = surface.Evaluate(u, v);
                }
            }

            return points;
        }
    }

    public static class MeshHelpers
    {
        public static Mesh Box(double a, double b, double c)
        {
            List<Point2D> pts = new List<Point2D>();
            pts.Add(new Point2D(a / 2, b / 2));
            pts.Add(new Point2D(-a / 2, b / 2));
            pts.Add(new Point2D(-a / 2, -b / 2));
            pts.Add(new Point2D(a / 2, -b / 2));
            pts.Add(new Point2D(a / 2, b / 2));
            Polyline poly = new Polyline(pts);
            MeshBuilder mb = MeshBuilder.ExtrudeWithCaps(poly, (float)c);
            return mb.ToMesh();
        }

        public static Mesh Parametric(SurfaceParametricEquation surface, UvSampler vertices, bool closed0 = false, bool closed1 = false)
        {
            MeshBuilder mb = MeshBuilder.ParametricEquation(surface, vertices, closed0, closed1);
            return mb.ToMesh();
        }

        public static Mesh Sphere(double radius)
        {
            SurfaceParametricEquation surface = new SurfaceParametricEquation(
                (u, v) => radius * Math.Cos(v) * Math.Cos(u),
                (u, v) => radius * Math.Cos(v) * Math.Sin(u),
                (u, v) => radius * Math.Sin(v)
            );
            UvSampler vertices = new UvSampler(new Interval(0, 2 * Math.PI), new Interval(-0.5 * Math.PI, 0.5 * Math.PI), 25, 31);
            return Parametric(surface, vertices, true, true);
        }

        public static Mesh HorizontalPlane(UvSampler vertices)
        {
            SurfaceParametricEquation surface = new SurfaceParametricEquation();
            return Parametric(surface, vertices);
        }

        public static Mesh ZFunction(Func<double, double, double> z, UvSampler vertices)
        {
            SurfaceParametricEquation surface = new SurfaceParametricEquation(
                (u, v) => u,
                (u, v) => v,
                (u, v) => z(u, v)
            );
            return Parametric(surface, vertices);
        }
    }

    public class SurfaceExamples
    {
        public static Mesh HorizontalPlane
        {
            get
            {
                UvSampler vertices = new UvSampler(new Interval(-1, 1), new Interval(-1, 1), 21, 21);
                return MeshHelpers.HorizontalPlane(vertices);
            }
        }

        public static Mesh ZFunction(Func<double, double, double> z)
        {
            UvSampler vertices = new UvSampler(new Interval(-5, 5), new Interval(-5, 5), 101, 101);
            return MeshHelpers.ZFunction(z, vertices);
        }

        public static Mesh Planar()
        {
            MeshBuilder mb = MeshBuilder.Planar(new Geometry.Polyline("0,0|1,0|2,1|2,2"));
            return mb.ToMesh();
        }

        public static Mesh Extrude()
        {
            MeshBuilder mb = MeshBuilder.ExtrudeSmooth(new Geometry.Polyline("0,0|1,0|2,1|2,2|0,0"), 2);
            return mb.ToMesh();
        }

        public static Mesh ExtrudeWithCaps()
        {
            MeshBuilder mb = MeshBuilder.ExtrudeWithCaps(new Geometry.Polyline("0,0|1,0|2,1|2,2|0,0"), 2);
            return mb.ToMesh();
        }

        public static Mesh NonConvexPlanar()
        {
            MeshBuilder mb = MeshBuilder.Planar(new Geometry.Polyline("0,0|9,0|9,9|6,9|6,6|3,6|3,9|0,9"));
            return mb.ToMesh();
        }

        //public static Mesh NonConvexExtrude()
        //{

        //}
    }
}
