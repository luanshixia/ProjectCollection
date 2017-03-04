using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Geometry
{
    public static class Algorithms
    {
        public static double TriangleInterpolate(double x, double y, Vector a, Vector b, Vector c)
        {
            double x1 = a.X, x2 = b.X, x3 = c.X, y1 = a.Y, y2 = b.Y, y3 = c.Y, z1 = a.Z, z2 = b.Z, z3 = c.Z;
            return (x2 * y * z1 - x3 * y * z1 - x * y2 * z1 + x3 * y2 * z1 + x * y3 * z1 - x2 * y3 * z1 -
                    x1 * y * z2 + x3 * y * z2 + x * y1 * z2 - x3 * y1 * z2 - x * y3 * z2 + x1 * y3 * z2 +
                    x1 * y * z3 - x2 * y * z3 - x * y1 * z3 + x2 * y1 * z3 + x * y2 * z3 - x1 * y2 * z3)
                    / (x2 * y1 - x3 * y1 - x1 * y2 + x3 * y2 + x1 * y3 - x2 * y3);
        }

        public static PointString Offset(this PointString poly, double offset)
        {
            var points0 = poly.Points;
            var points1 = new List<Vector>();
            if (points0.Count <= 0)
            {
                //return null;
            }
            else if (points0.Count == 1)
            {
                var pt = new Vector(points0[0].X + offset, points0[0].Y);
                points1.Add(pt);
            }
            else if (points0.Count == 2)
            {
                points1 = GetOffsetSeg(points0[0], points0[1], offset);
            }
            else
            {
                var pt1 = GetOffsetSeg(points0[0], points0[1], offset)[0];
                points1.Add(pt1);

                for (int count = 1; count <= points0.Count - 1; count++)
                {
                    if (count == points0.Count - 1)
                    {
                        var pt = GetOffsetSeg(points0[count - 1], points0[count], offset)[1];
                        points1.Add(pt);
                    }
                    else
                    {
                        var p10 = GetOffsetSeg(points0[count - 1], points0[count], offset)[0];
                        var p11 = GetOffsetSeg(points0[count - 1], points0[count], offset)[1];
                        var p20 = GetOffsetSeg(points0[count], points0[count + 1], offset)[0];
                        var p21 = GetOffsetSeg(points0[count], points0[count + 1], offset)[1];

                        var d0 = new Vector(p11.X - p10.X, p11.Y - p10.Y);
                        var d1 = new Vector(p20.X - p21.X, p20.Y - p21.Y);

                        points1.Add(GetIntersection(p10, p21, d0, d1));
                    }
                }
            }
            return new PointString(points1);
        }

        private static Vector GetIntersection(Vector p0, Vector p1, Vector d0, Vector d1)
        {
            var p = new Vector(p1.X - p0.X, p1.Y - p0.Y);
            double kross = d0.X * d1.Y - d0.Y * d1.X;
            double sqrKross = kross * kross;
            double sqrLen0 = d0.X * d0.X + d0.Y * d0.Y;
            double sqrLen1 = d1.X * d1.X + d1.Y * d1.Y;

            //lines are not parallel
            if (sqrKross > 0.0001 * sqrLen0 * sqrLen1)
            {
                double s = (p.X * d1.Y - p.Y * d1.X) / kross;
                var pt = new Vector(p0.X + d0.X * s, p0.Y + d0.Y * s);
                return pt;
            }

            //lines are parallel
            double sqrLenE = p.X * p.X + p.Y * p.Y;
            kross = p.X * d0.Y - p.Y * d0.X;
            sqrKross = kross * kross;
            //lines are different
            if (sqrKross > 0.0001 * sqrLen0 * sqrLen1)
            {
                return new Vector();
            }
            //lines are the same
            else
            {
                return p0;
            }
        }

        private static List<Vector> GetOffsetSeg(Vector pt1, Vector pt2, double offset)
        {
            double offset_X = (-1) * (pt2.Y - pt1.Y) / (Math.Sqrt((pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y))) * offset;
            double offset_Y = (pt2.X - pt1.X) / (Math.Sqrt((pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y))) * offset;
            var ptFirst = new Vector(pt1.X + offset_X, pt1.Y + offset_Y);
            var ptSecond = new Vector(pt2.X + offset_X, pt2.Y + offset_Y);
            var listPt = new List<Vector>();
            listPt.Add(ptFirst);
            listPt.Add(ptSecond);
            return listPt;
        }

        public static void ReducePoints(this PointString poly, double epsilon)
        {
            var points = poly.Points;
            var cleanList = new List<int>();
            int j = 0;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].Dist(points[j]) < epsilon)
                {
                    cleanList.Add(i);
                }
                else
                {
                    j = i;
                }
            }
            cleanList.Reverse();
            cleanList.ForEach(x => points.RemoveAt(x));
        }

        public static List<Vector> GetConvexHull(this PointString poly)
        {
            var points = new List<Vector>();
            var collection = new List<Vector>();
            int num = 0;
            var source = poly.Points.ToList();
            source.Sort((p1, p2) => (p1.X - p2.X == 0) ? (int)(p1.Y - p2.Y) : (int)(p1.X - p2.X));

            points.Add(source[0]);
            points.Add(source[1]);
            for (num = 2; num <= (source.Count - 1); num++)
            {
                points.Add(source[num]);
                while ((points.Count >= 3) && !IsTurnRight(points[points.Count - 3], points[points.Count - 2], points[points.Count - 1]))
                {
                    points.RemoveAt(points.Count - 2);
                }
            }
            collection.Add(source[source.Count - 1]);
            collection.Add(source[source.Count - 2]);
            for (num = source.Count - 2; num >= 0; num--)
            {
                collection.Add(source[num]);
                while ((collection.Count >= 3) && !IsTurnRight(collection[collection.Count - 3], collection[collection.Count - 2], collection[collection.Count - 1]))
                {
                    collection.RemoveAt(collection.Count - 2);
                }
            }
            collection.RemoveAt(collection.Count - 1);
            collection.RemoveAt(0);
            points.AddRange(collection);
            return points;
        }

        private static bool IsTurnRight(Vector px, Vector py, Vector pz)
        {
            double num = 0;
            num = ((pz.Y - py.Y) * (py.X - px.X)) - ((py.Y - px.Y) * (pz.X - py.X));
            return num < 0d;
        }

        public static double DistToPoint(this PointString poly, Vector p, out Vector footPos)
        {
            var points = poly.Points;
            double minDist = points.Min(x => x.Dist(p));
            var minPoint = points.First(x => x.Dist(p) == minDist);
            if (minPoint.Equals(points.First()))
            {
                LineSeg seg1 = new LineSeg(points[0], points[1]);
                LineSegPointRelation relation1 = new LineSegPointRelation(seg1, p);
                if (relation1.Inner)
                {
                    footPos = relation1.Foot;
                    return relation1.Dist;
                }
                else
                {
                    footPos = minPoint;
                    return minDist;
                }
            }
            else if (minPoint.Equals(points.Last()))
            {
                int n = points.Count;
                LineSeg seg1 = new LineSeg(points[n - 2], points[n - 1]);
                LineSegPointRelation relation1 = new LineSegPointRelation(seg1, p);
                if (relation1.Inner)
                {
                    footPos = relation1.Foot;
                    return relation1.Dist;
                }
                else
                {
                    footPos = minPoint;
                    return minDist;
                }
            }
            else
            {
                int index = points.IndexOf(minPoint);
                LineSeg seg1 = new LineSeg(points[index - 1], points[index]);
                LineSeg seg2 = new LineSeg(points[index + 1], points[index]);
                LineSegPointRelation relation1 = new LineSegPointRelation(seg1, p);
                LineSegPointRelation relation2 = new LineSegPointRelation(seg2, p);
                if (relation1.Inner)
                {
                    footPos = relation1.Foot;
                    return relation1.Dist;
                }
                else if (relation2.Inner)
                {
                    footPos = relation1.Foot;
                    return relation2.Dist;
                }
                else
                {
                    footPos = minPoint;
                    return minDist;
                }
            }
        }

        public static double DistToPoint(this PointString poly, Vector p)
        {
            Vector foot;
            return DistToPoint(poly, p, out foot);
        }

        public static double DistToPoint(this PointString poly, Vector p, int divs) // newly 20130523 近似法
        {
            var points = poly.Points;
            double delta = (double)(points.Count - 1) / divs;
            var pts = Enumerable.Range(0, divs + 1).Select(i => poly.Lerp(i * delta)).ToList();
            return pts.Min(pt => pt.Dist(p));
        }

        public static PointString GetSubPoly(this PointString poly, double start, double end)
        {
            var points = poly.Points;
            start = (start < 0) ? 0 : start;
            end = (end > points.Count - 1) ? (points.Count - 1) : end;
            var pts = new List<Vector>();
            double a = Math.Ceiling(start);
            double b = Math.Floor(end);
            for (int i = (int)a; i <= (int)b; i++)
            {
                pts.Add(points[i]);
            }
            if (start < a)
            {
                pts.Insert(0, poly.Lerp(start));
            }
            if (end > b)
            {
                pts.Add(poly.Lerp(end));
            }
            return new PointString(pts);
        }

        public static LineSeg GetSegAt(this PointString poly, int i)
        {
            var points = poly.Points;
            if (i < 0)
            {
                i = 0;
            }
            else if (i > points.Count - 1)
            {
                i = points.Count - 1;
            }
            int j = i >= points.Count - 1 ? 0 : i + 1;
            return new LineSeg(points[i], points[j]);
        }

        public static List<Vector> GetGrid(this Extents extents, double interval)
        {
            var min = extents.Min.Value;
            var max = extents.Max.Value;
            var pts = new List<Vector>();
            for (double x = min.X + interval / 2; x < max.X; x += interval)
            {
                for (double y = min.Y + interval / 2; y < max.Y; y += interval)
                {
                    pts.Add(new Vector(x, y));
                }
            }
            return pts;
        }

        public static List<Vector> GetHexGrid(this Extents extents, double interval, double yScale = 1)
        {
            var min = extents.Min.Value;
            var max = extents.Max.Value;
            var pts = new List<Vector>();
            bool even = false;
            double yInterval = yScale * interval;
            for (double y = min.Y + yInterval / 2; y < max.Y; y += yInterval)
            {
                double x0 = even ? min.X : min.X + interval / 2;
                for (double x = x0; x < max.X; x += interval)
                {
                    pts.Add(new Vector(x, y));
                }
                even = !even;
            }
            return pts;
        }

        public static Vector[,] GetGrid2d(this Extents extents, double interval)
        {
            var min = extents.Min.Value;
            var max = extents.Max.Value;
            var xs = new List<double>();
            var ys = new List<double>();
            for (double x = min.X + interval / 2; x < max.X; x += interval)
            {
                xs.Add(x);
            }
            for (double y = min.Y + interval / 2; y < max.Y; y += interval)
            {
                ys.Add(y);
            }
            Vector[,] pts = new Vector[xs.Count, ys.Count];
            for (int i = 0; i < xs.Count; i++)
            {
                for (int j = 0; j < ys.Count; j++)
                {
                    pts[i, j] = new Vector(xs[i], ys[j]);
                }
            }
            return pts;
        }
    }

    public class LineSeg
    {
        public Vector A;
        public Vector B;

        public LineSeg(Vector a, Vector b)
        {
            A = a;
            B = b;
        }

        public double Length
        {
            get
            {
                return A.Dist(B);
            }
        }

        public Vector GetDivPoint(double lambda)
        {
            if (double.IsInfinity(lambda))
            {
                return B;
            }
            else
            {
                return (1 / (1 + lambda)) * (A + lambda * B);
            }
        }

        public Vector GetLerpPoint(double amt) // newly 20141107
        {
            return A + amt * (B - A);
        }

        public bool IsPointOn(Vector point)
        {
            if (Utils.FloatEquals((point - A).Kross(point - B), 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class LineSegIntersect
    {
        private Vector A1, B1, A2, B2;
        public double lambda, miu;

        public LineSegIntersect(LineSeg l1, LineSeg l2)
        {
            A1 = l1.A;
            B1 = l1.B;
            A2 = l2.A;
            B2 = l2.B;
        }

        public bool Intersect()
        {
            if ((A1 - B1).Kross(A2 - B2) == 0)
            {
                return false;
            }

            if (A2.X == B2.X)
            {
                lambda = (A2.X - A1.X) / (B1.X - A2.X);
                double y = (A1.Y + lambda * B1.Y) / (1 + lambda);
                miu = (y - A2.Y) / (B2.Y - y);
            }
            else if (A1.X == B1.X)
            {
                miu = (A1.X - A2.X) / (B2.X - A1.X);
                double y = (A2.Y + miu * B2.Y) / (1 + miu);
                lambda = (y - A1.Y) / (B1.Y - y);
            }
            else if (A2.Y == B2.Y)
            {
                lambda = (A2.Y - A1.Y) / (B1.Y - A2.Y);
                double x = (A1.X + lambda * B1.X) / (1 + lambda);
                miu = (x - A2.X) / (B2.X - x);
            }
            else if (A1.Y == B1.Y)
            {
                miu = (A1.Y - A2.Y) / (B2.Y - A1.Y);
                double x = (A2.X + miu * B2.X) / (1 + miu);
                lambda = (x - A1.X) / (B1.X - x);
            }
            else
            {
                lambda = (A2.X * A1.Y - B2.X * A1.Y - A1.X * A2.Y + B2.X * A2.Y + A1.X * B2.Y - A2.X * B2.Y) / (-A2.X * B1.Y + B2.X * B1.Y + B1.X * A2.Y - B2.X * A2.Y - B1.X * B2.Y + A2.X * B2.Y);
                miu = (-B1.X * A1.Y + A2.X * A1.Y + A1.X * B1.Y - A2.X * B1.Y - A1.X * A2.Y + B1.X * A2.Y) / (B1.X * A1.Y - B2.X * A1.Y - A1.X * B1.Y + B2.X * B1.Y + A1.X * B2.Y - B1.X * B2.Y); // from Mathematica
            }

            bool result = false;
            if (lambda >= 0 || double.IsInfinity(lambda))
            {
                if (miu >= 0 || double.IsInfinity(miu))
                {
                    result = true;
                }
            }
            return result;
        }

        public Vector Coincidence
        {
            get
            {
                return new LineSeg(A1, B1).GetDivPoint(lambda);
            }
        }

        public double RatioAB
        {
            get
            {
                if (double.IsInfinity(lambda))
                {
                    return 1;
                }
                else
                {
                    return lambda / (1 + lambda);
                }
            }
        }
    }

    public class LineSegPointRelation
    {
        private Vector A, B, C, D;
        private double lambda;

        public LineSegPointRelation(LineSeg line, Vector point)
        {
            A = line.A;
            B = line.B;
            C = point;
            if (line.IsPointOn(point))
            {
                D = C;
                lambda = (A.X - C.X) / (C.X - B.X);
                if (double.IsNaN(lambda))
                {
                    lambda = (A.Y - C.Y) / (C.Y - B.Y);
                }
            }
            else
            {
                lambda = (-A.X * A.X + A.X * B.X + A.X * C.X - B.X * C.X - A.Y * A.Y + A.Y * B.Y + A.Y * C.Y - B.Y * C.Y) / (A.X * B.X - B.X * B.X - A.X * C.X + B.X * C.X + A.Y * B.Y - B.Y * B.Y - A.Y * C.Y + B.Y * C.Y); // from Mathematica
                D = line.GetDivPoint(lambda);
            }
        }

        public bool Inner
        {
            get
            {
                return (lambda >= 0 || double.IsInfinity(lambda));
            }
        }

        public double Dist
        {
            get
            {
                return C.Dist(D);
            }
        }

        public Vector Foot
        {
            get
            {
                return D;
            }
        }
    }

    public class CoordinateMapping
    {
        public double X21;
        public double Y21;
        public double ScaleX21;
        public double ScaleY21;

        public CoordinateMapping(double x0, double y0, double scaleX, double scaleY)
        {
            X21 = x0;
            Y21 = y0;
            ScaleX21 = scaleX;
            ScaleY21 = scaleY;
        }

        public Vector Transform(Vector p)
        {
            return new Vector((p.X - X21) * ScaleX21, (p.Y - Y21) * ScaleY21);
        }

        public Vector TransformBack(Vector p)
        {
            return new Vector(p.X / ScaleX21 + X21, p.Y / ScaleY21 + Y21);
        }
    }

    /// <summary>
    /// parameters settings
    /// </summary>
    public static class Parameters
    {
        /// <summary>
        /// get or set the length of decimal places when building the string notation
        /// </summary>
        public static int DecimalPlaces { get; set; }

        /// <summary>
        /// epsilon when determine equality
        /// </summary>
        public const double Epsilon = 0.001;

        static Parameters()
        {
            DecimalPlaces = 4;
        }

        /// <summary>
        /// get the string form of a number with given decimal places
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ToDecimalString(this double num)
        {
            string str = num.ToString();
            int i = str.IndexOf('.');
            int j = i + DecimalPlaces + 1;
            if (i > -1 && j < str.Length)
            {
                return str.Remove(j);
            }
            else
            {
                return str;
            }
        }
    }
}
