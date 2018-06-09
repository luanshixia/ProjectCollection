using Dreambuild.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Geometry3D
{
    public class Mesh
    {
        // Properties with storage

        public List<Vector> Vertices { get; private set; }
        public List<Vector> Normals { get; private set; }
        public List<Vector> TextureCoordinates { get; private set; }
        public List<Triangle> Triangles { get; private set; }

        // Constructor

        public Mesh()
        {
            this.Vertices = new List<Vector>();
            this.Normals = new List<Vector>();
            this.TextureCoordinates = new List<Vector>();
            this.Triangles = new List<Triangle>();
        }

        // Methods

        public Extents GetBounds()
        {
            var rect = Extents.Empty;
            this.Vertices.ForEach(x => rect.AddPoint(x));
            return rect;
        }

        public Mesh Transform(Matrix4 matrix)
        {
            for (int i = 0; i < this.Vertices.Count; i++)
            {
                this.Vertices[i] = this.Vertices[i].Transform(matrix);
            }
            var rotation = matrix.ExtractRotation();
            for (int i = 0; i < this.Normals.Count; i++)
            {
                this.Normals[i] = this.Normals[i].Transform(rotation);
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
            this.V0 = v0;
            this.V1 = v1;
            this.V2 = v2;
        }
    }

    public class Triangle2
    {
        public Vector A;
        public Vector B;
        public Vector C;

        public Triangle2(Vector a, Vector b, Vector c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public double Area
        {
            get
            {
                return Math.Abs((B - A).Kross(C - A)) / 2;
            }
        }

        public bool IsPointIn(Vector p)
        {
            var ta = new Triangle2(p, B, C);
            var tb = new Triangle2(p, A, C);
            var tc = new Triangle2(p, A, B);
            return Math.Abs(ta.Area + tb.Area + tc.Area - Area) < 1e-6;
        }

        public IEnumerable<Vector> P
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
        public Vector A;
        public Vector B;
        public Vector C;

        public Triangle3(Vector a, Vector b, Vector c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }
    }

    public class MeshBuilder
    {
        public List<Vector> Vertices = new List<Vector>();
        public List<Triangle> Faces = new List<Triangle>();
        public List<Vector> Normals = new List<Vector>();
        public List<Vector> Texcoords = new List<Vector>();

        private MeshBuilder()
        {
        }

        public Mesh ToMesh()
        {
            var mesh = new Mesh();
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
                var temp = face.V0;
                face.V0 = face.V1;
                face.V1 = temp;
            }
        }

        private void AppendTriangle(int v0, int v1, int v2)
        {
            this.Faces.Add(new Triangle(v0, v1, v2));
        }

        private void AppendQuad(int v0, int v1, int v2, int v3)
        {
            this.Faces.Add(new Triangle(v0, v1, v2));
            this.Faces.Add(new Triangle(v0, v2, v3));
        }

        public void AddTriangle(Vector p0, Vector p1, Vector p2)
        {
            var i0 = Vertices.Count();

            this.Vertices.Add(p0);
            this.Vertices.Add(p1);
            this.Vertices.Add(p2);

            this.AppendTriangle(i0, i0 + 1, i0 + 2);
        }

        public void AddQuad(Vector p0, Vector p1, Vector p2, Vector p3)
        {
            var i0 = Vertices.Count();

            this.Vertices.Add(p0);
            this.Vertices.Add(p1);
            this.Vertices.Add(p2);
            this.Vertices.Add(p3);

            this.AppendQuad(i0, i0 + 1, i0 + 2, i0 + 3);
        }

        public void Append(MeshBuilder mb)
        {
            var i0 = this.Vertices.Count();
            foreach (var p in mb.Vertices)
            {
                this.Vertices.Add(p);
            }
            foreach (var f in mb.Faces)
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
            var mb = new MeshBuilder();
            mb.Vertices.AddRange(mesh.Vertices);
            mb.Faces.AddRange(mesh.Triangles);
            mb.Normals.AddRange(mesh.Normals);
            mb.Texcoords.AddRange(mesh.TextureCoordinates);
            return mb;
        }

        public static MeshBuilder RectGrid(Vector[,] points, bool closed0 = false, bool closed1 = false)
        {
            var mb = new MeshBuilder();
            var rows = points.GetUpperBound(0) + 1;
            var columns = points.GetUpperBound(1) + 1;
            var index0 = 0;
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
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

        public static MeshBuilder ExtrudeSmooth(PointString curve, double height)
        {
            var points0 = curve.Points.Select(p => new Vector(p.X, p.Y, 0)).ToList();
            var points1 = curve.Points.Select(p => new Vector(p.X, p.Y, height)).ToList();
            var pointCount = curve.Points.Count;

            var points = new Vector[pointCount, 2];
            Enumerable.Range(0, pointCount).ToList().ForEach(i => points[i, 0] = points0[i]);
            Enumerable.Range(0, pointCount).ToList().ForEach(i => points[i, 1] = points1[i]);

            return MeshBuilder.RectGrid(points);
        }

        public static MeshBuilder Extrude(PointString curve, double height)
        {
            if (curve.AlgebraicArea() < 0)
            {
                curve.Points.Reverse();
            }
            var points0 = curve.Points.Select(p => new Vector(p.X, p.Y, 0)).ToList();
            var points1 = curve.Points.Select(p => new Vector(p.X, p.Y, height)).ToList();
            var pointCount = curve.Points.Count;

            var mb = new MeshBuilder();
            for (int i = 0; i < pointCount - 1; i++)
            {
                mb.AddQuad(points0[i], points0[i + 1], points1[i + 1], points1[i]);
            }
            return mb;
        }

        public static MeshBuilder ExtrudeWithCaps(PointString curve, double height)
        {
            var mb = MeshBuilder.Extrude(curve, height);
            var cap0 = MeshBuilder.Planar(curve);
            var cap1 = MeshBuilder.Planar(curve);
            if (cap0 != null)
            {
                cap0.FlipFaces();
                cap1.Transform(Matrix4.Translation(0, 0, height));
                mb.Append(cap0);
                mb.Append(cap1);
            }
            return mb;
        }

        public static MeshBuilder Planar(PointString curve)
        {
            const double epsilon = 0.001;
            curve.ReducePoints(epsilon);
            var points = curve.Points.ToList();
            if (points.Last().Dist(points.First()) < epsilon)
            {
                points.RemoveAt(points.Count - 1);
            }
            var triIndices = CuttingEarsTriangulator.Triangulate(points);
            if (triIndices == null)
            {
                return null;
            }
            var mb = new MeshBuilder();
            for (int i = 0; i < triIndices.Count; i += 3)
            {
                var a = new Vector(points[triIndices[i]].X, points[triIndices[i]].Y, 0);
                var b = new Vector(points[triIndices[i + 1]].X, points[triIndices[i + 1]].Y, 0);
                var c = new Vector(points[triIndices[i + 2]].X, points[triIndices[i + 2]].Y, 0);
                mb.AddTriangle(a, b, c);
            }
            return mb;
        }

        private void MakeRectGridFaces(int index0, int rows, int columns, bool rowsClosed, bool columnsClosed)
        {
            var m2 = rows - 1;
            var n2 = columns - 1;
            if (columnsClosed) m2++;
            if (rowsClosed) n2++;

            for (var i = 0; i < m2; i++)
            {
                for (var j = 0; j < n2; j++)
                {
                    var ij00 = index0 + i * columns + j;
                    var ij01 = index0 + i * columns + (j + 1) % columns;
                    var ij10 = index0 + ((i + 1) % rows) * columns + j;
                    var ij11 = index0 + ((i + 1) % rows) * columns + (j + 1) % columns;

                    this.AppendQuad(ij00, ij01, ij11, ij10);
                }
            }
        }

        public void Transform(Matrix4 matrix)
        {
            for (var i = 0; i < this.Vertices.Count; i++)
            {
                this.Vertices[i] = this.Vertices[i].Transform(matrix);
            }
            Matrix4 rotation = matrix.ExtractRotation();
            for (var i = 0; i < this.Normals.Count; i++)
            {
                this.Normals[i] = this.Normals[i].Transform(rotation);
            }
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
            this.X = (u, v) => u;
            this.Y = (u, v) => v;
            this.Z = (u, v) => 0;
        }

        public SurfaceParametricEquation(UvExpression x, UvExpression y, UvExpression z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector Evaluate(double u, double v)
        {
            return new Vector(X(u, v), Y(u, v), Z(u, v));
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
            var div = this.Length / (segs - 1);
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
            this.UDomain = uDomain;
            this.VDomain = vDomain;
            this.UCount = uCount;
            this.VCount = vCount;
        }

        public Vector[,] GetSamples()
        {
            var uSamples = UDomain.Sample(UCount);
            var vSamples = VDomain.Sample(VCount);

            var samples = new Vector[UCount, VCount];
            for (var i = 0; i < UCount; i++)
            {
                for (var j = 0; j < VCount; j++)
                {
                    samples[i, j] = new Vector(uSamples[i], vSamples[j]);
                }
            }

            return samples;
        }

        public Vector[,] EvaluateSurface(SurfaceParametricEquation surface)
        {
            var samples = GetSamples();
            var points = new Vector[UCount, VCount];
            for (var i = 0; i < UCount; i++)
            {
                for (var j = 0; j < VCount; j++)
                {
                    var sample = samples[i, j];
                    var u = sample.X;
                    var v = sample.Y;
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
            var pts = new List<Vector>
            {
                new Vector(a / 2, b / 2),
                new Vector(-a / 2, b / 2),
                new Vector(-a / 2, -b / 2),
                new Vector(a / 2, -b / 2),
                new Vector(a / 2, b / 2)
            };
            var poly = new PointString(pts);
            var mb = MeshBuilder.ExtrudeWithCaps(poly, (float)c);
            return mb.ToMesh();
        }

        public static Mesh Parametric(SurfaceParametricEquation surface, UvSampler vertices, bool closed0 = false, bool closed1 = false)
        {
            var mb = MeshBuilder.ParametricEquation(surface, vertices, closed0, closed1);
            return mb.ToMesh();
        }

        public static Mesh Sphere(double radius)
        {
            var surface = new SurfaceParametricEquation(
                (u, v) => radius * Math.Cos(v) * Math.Cos(u),
                (u, v) => radius * Math.Cos(v) * Math.Sin(u),
                (u, v) => radius * Math.Sin(v)
            );
            var vertices = new UvSampler(new Interval(0, 2 * Math.PI), new Interval(-0.5 * Math.PI, 0.5 * Math.PI), 25, 31);
            return Parametric(surface, vertices, true, true);
        }

        public static Mesh HorizontalPlane(UvSampler vertices)
        {
            var surface = new SurfaceParametricEquation();
            return Parametric(surface, vertices);
        }

        public static Mesh ZFunction(Func<double, double, double> z, UvSampler vertices)
        {
            var surface = new SurfaceParametricEquation(
                (u, v) => u,
                (u, v) => v,
                (u, v) => z(u, v)
            );
            return Parametric(surface, vertices);
        }
    }

    public static class SurfaceExamples
    {
        public static Mesh HorizontalPlane
        {
            get
            {
                var vertices = new UvSampler(new Interval(-1, 1), new Interval(-1, 1), 21, 21);
                return MeshHelpers.HorizontalPlane(vertices);
            }
        }

        public static Mesh ZFunction(Func<double, double, double> z)
        {
            var vertices = new UvSampler(new Interval(-5, 5), new Interval(-5, 5), 101, 101);
            return MeshHelpers.ZFunction(z, vertices);
        }

        public static Mesh Planar()
        {
            var mb = MeshBuilder.Planar(PointString.Parse("0,0|1,0|2,1|2,2"));
            return mb.ToMesh();
        }

        public static Mesh Extrude()
        {
            var mb = MeshBuilder.ExtrudeSmooth(PointString.Parse("0,0|1,0|2,1|2,2|0,0"), 2);
            return mb.ToMesh();
        }

        public static Mesh ExtrudeWithCaps()
        {
            var mb = MeshBuilder.ExtrudeWithCaps(PointString.Parse("0,0|1,0|2,1|2,2|0,0"), 2);
            return mb.ToMesh();
        }

        public static Mesh NonConvexPlanar()
        {
            var mb = MeshBuilder.Planar(PointString.Parse("0,0|9,0|9,9|6,9|6,6|3,6|3,9|0,9"));
            return mb.ToMesh();
        }

        //public static Mesh NonConvexExtrude()
        //{

        //}
    }
}
