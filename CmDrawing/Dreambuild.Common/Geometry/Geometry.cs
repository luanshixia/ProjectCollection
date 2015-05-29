using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dreambuild.Geometry
{
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
            DecimalPlaces = 2;
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

    

    

    

    

    public class Point2DString
    {
        protected List<Point2D> points;

        protected bool _isClosed = false;
        public bool IsClosed { get { return _isClosed; } }

        public int Count { get { return points.Count; } }
        public List<Point2D> Points { get { return points; } }

        public Point2DString(List<Point2D> pts)
        {
            points = pts;
        }

        public Point2DString(string ptStr)
        {
            if (!string.IsNullOrEmpty(ptStr))
            {
                string[] pts = ptStr.Split('|');
                points = pts.Select(x => new Point2D(x)).ToList();
            }
            else
            {
                points = new List<Point2D>();
            }
        }

        protected Point2DString()
        {
        }

        public Point2D this[int i]
        {
            get
            {
                return points[i];
            }
        }

        public Point2D this[double d]
        {
            get
            {
                int i = (int)Math.Floor(d);
                int j = i + 1;
                double lambda = (d - i) / (j - d);
                return GetSegAt(i).GetDivPoint(lambda);
            }
        }

        // 20111014
        public Extent2D GetExtent()
        {
            Extent2D result = Extent2D.Null;
            points.ForEach(p => result.AddPoint(p));
            return result;
        }

        public LineSeg GetSegAt(int i)
        {
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

        public List<LineSeg> GetAllSegs()
        {
            return Enumerable.Range(0, Count - 1).Select(x => GetSegAt(x)).ToList();
        }

        public void CloseShape()
        {
            _isClosed = true;
        }

        public void Insert(int index, Point2D p)
        {
            points.Insert(index, p);
        }

        public void Remove(int index)
        {
            points.RemoveAt(index);
        }

        public double Perimeter
        {
            get
            {
                double temp = 0;
                for (int i = 0; i < Count; i++)
                {
                    int j = (i < Count - 1) ? (i + 1) : 0;
                    temp += points[i].DistTo(points[j]);
                }
                return temp;
            }
        }

        public Point2D Average
        {
            get
            {
                Vector2D temp = Vector2D.Zero;
                for (int i = 0; i < Count; i++)
                {
                    temp += points[i] - new Point2D(0, 0);
                }
                return ((1.0 / Count) * temp).ToPoint();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in points)
            {
                sb.AppendFormat("{0}|", p);
            }
            string s = sb.ToString();
            return s.Remove(s.Length - 1);
        }

        [Obsolete]
        public double GetFirstDerivative(double dist)
        {
            if (points.Count <= 1 || dist < 0)
            {
                return double.NegativeInfinity;
            }

            bool isValued = false;
            int count = points.Count;
            double Derivative = 0;
            double tempDist = 0;
            for (int i = 0; i < count - 1; i++)
            {
                double distance = Math.Sqrt((points[i].X - points[i + 1].X) * (points[i].X - points[i + 1].X) + (points[i].Y - points[i + 1].Y) * (points[i].Y - points[i + 1].Y));
                if (tempDist + distance < dist)
                {
                    tempDist = tempDist + distance;
                    continue;
                }
                else
                {
                    isValued = true;
                    if (points[i + 1].X == points[i].X)
                    {
                        Derivative = double.PositiveInfinity;
                    }
                    else
                    {
                        Derivative = (points[i + 1].Y - points[i].Y) / (points[i + 1].X - points[i].X);
                    }
                    break;
                }
            }

            if (!isValued)
            {
                Derivative = double.NegativeInfinity;
            }
            return Derivative;
        }

        [Obsolete]
        public double GetDistAtPoint(Point2D p)
        {
            double distance = 0;
            bool isOnLine = false;

            if (points.Count == 0)
            {
                return -1;
            }
            else if (points.Count == 1)
            {
                return Math.Sqrt((p.X - points[0].X) * (p.X - points[0].X) + (p.Y - points[0].Y) * (p.Y - points[0].Y));
            }
            else
            {
                int count = points.Count;
                for (int i = 0; i < count - 1; i++)
                {
                    if (points[i].X == points[i + 1].X && p.X == points[i].X)
                    {
                        if ((p.Y - points[i].Y) * (p.Y - points[i + 1].Y) <= 0)
                        {
                            distance = distance + Math.Sqrt((p.X - points[i].X) * (p.X - points[i].X) + (p.Y - points[i].Y) * (p.Y - points[i].Y));
                            isOnLine = true;
                            break;
                        }
                        else
                        {
                            isOnLine = false;
                            break;
                        }
                    }

                    else if (points[i].Y == points[i + 1].Y && p.Y == points[i].Y)
                    {
                        if ((p.X - points[i].X) * (p.X - points[i + 1].X) <= 0)
                        {
                            distance = distance + Math.Sqrt((p.X - points[i].X) * (p.X - points[i].X) + (p.Y - points[i].Y) * (p.Y - points[i].Y));
                            isOnLine = true;
                            break;
                        }
                        else
                        {
                            isOnLine = false;
                            break;
                        }
                    }

                    else
                    {
                        if ((p.Y - points[i].Y) / (p.X - points[i].X) == (p.Y - points[i + 1].Y) / (p.X - points[i + 1].X) && (p.Y - points[i].Y != 0) && (points[i + 1].X - points[i].X) * (p.X - points[i].X) >= 0)
                        {
                            distance = distance + Math.Sqrt((p.X - points[i].X) * (p.X - points[i].X) + (p.Y - points[i].Y) * (p.Y - points[i].Y));
                            isOnLine = true;
                            break;
                        }
                        else
                        {
                            distance = distance + Math.Sqrt((points[i + 1].X - points[i].X) * (points[i + 1].X - points[i].X) + (points[i + 1].Y - points[i].Y) * (points[i + 1].Y - points[i].Y));
                        }
                    }
                }
            }

            if (!isOnLine)
            {
                distance = -1;
            }

            return distance;
        }

        public Point2D GetPointAtDist(double dist)
        {
            if (points.Count <= 1 || dist < 0)
            {
                return points[0];
            }

            Point2D pt = new Point2D(-1, -1);
            int count = points.Count;
            double tempDist = 0;
            for (int i = 0; i < count - 1; i++)
            {
                double distance = Math.Sqrt((points[i].X - points[i + 1].X) * (points[i].X - points[i + 1].X) + (points[i].Y - points[i + 1].Y) * (points[i].Y - points[i + 1].Y));
                if (tempDist + distance == dist)
                {
                    pt = points[i + 1];
                    break;
                }
                else if (tempDist + distance < dist)
                {
                    tempDist = tempDist + distance;
                    continue;
                }
                else
                {
                    double length = points[i].DistTo(points[i + 1]);
                    double proportion = (dist - tempDist) / length;
                    if (points[i].X == points[i + 1].X)
                    {
                        pt.X = points[i].X;
                    }
                    else
                    {
                        pt.X = (points[i + 1].X - points[i].X) * proportion + points[i].X;
                    }

                    if (points[i].Y == points[i + 1].Y)
                    {
                        pt.Y = points[i].Y;
                    }
                    else
                    {
                        pt.Y = (points[i + 1].Y - points[i].Y) * proportion + points[i].Y;
                    }
                    break;
                }
            }

            if (pt.X == -1 && pt.Y == -1)
            {
                double total = GetDistAtPoint(points.ElementAt(count - 1));
                pt = GetPointAtDist(total / 2);
            }

            return pt;
        }

        public Point2DString GetOffset(double offset)
        {
            List<Point2D> listPt = new List<Point2D>();
            if (points.Count <= 0)
            {
                //return null;
            }
            else if (points.Count == 1)
            {
                Point2D pt = new Point2D(points[0].X + offset, points[0].Y);
                listPt.Add(pt);
            }
            else if (points.Count == 2)
            {
                listPt = GetOffsetSeg(points[0], points[1], offset);
            }
            else
            {
                Point2D pt1 = GetOffsetSeg(points[0], points[1], offset)[0];
                listPt.Add(pt1);

                for (int count = 1; count <= points.Count - 1; count++)
                {
                    if (count == points.Count - 1)
                    {
                        Point2D pt = GetOffsetSeg(points[count - 1], points[count], offset)[1];
                        listPt.Add(pt);
                    }
                    else
                    {
                        Point2D p10 = GetOffsetSeg(points[count - 1], points[count], offset)[0];
                        Point2D p11 = GetOffsetSeg(points[count - 1], points[count], offset)[1];
                        Point2D p20 = GetOffsetSeg(points[count], points[count + 1], offset)[0];
                        Point2D p21 = GetOffsetSeg(points[count], points[count + 1], offset)[1];

                        Point2D d0 = new Point2D(p11.X - p10.X, p11.Y - p10.Y);
                        Point2D d1 = new Point2D(p20.X - p21.X, p20.Y - p21.Y);

                        listPt.Add(GetIntersection(p10, p21, d0, d1));
                    }
                }
            }
            Point2DString ptSet = new Point2DString(listPt);
            return ptSet;
        }

        private Point2D GetIntersection(Point2D p0, Point2D p1, Point2D d0, Point2D d1)
        {
            Point2D p = new Point2D(p1.X - p0.X, p1.Y - p0.Y);
            double kross = d0.X * d1.Y - d0.Y * d1.X;
            double sqrKross = kross * kross;
            double sqrLen0 = d0.X * d0.X + d0.Y * d0.Y;
            double sqrLen1 = d1.X * d1.X + d1.Y * d1.Y;

            //lines are not parallel
            if (sqrKross > 0.0001 * sqrLen0 * sqrLen1)
            {
                double s = (p.X * d1.Y - p.Y * d1.X) / kross;
                Point2D pt = new Point2D(p0.X + d0.X * s, p0.Y + d0.Y * s);
                return pt;
            }

            //lines are parallel
            double sqrLenE = p.X * p.X + p.Y * p.Y;
            kross = p.X * d0.Y - p.Y * d0.X;
            sqrKross = kross * kross;
            //lines are different
            if (sqrKross > 0.0001 * sqrLen0 * sqrLen1)
            {
                return new Point2D();
            }
            //lines are the same
            else
            {
                return p0;
            }
        }

        private List<Point2D> GetOffsetSeg(Point2D pt1, Point2D pt2, double offset)
        {
            double offset_X = (-1) * (pt2.Y - pt1.Y) / (Math.Sqrt((pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y))) * offset;
            double offset_Y = (pt2.X - pt1.X) / (Math.Sqrt((pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y))) * offset;
            Point2D ptFirst = new Point2D(pt1.X + offset_X, pt1.Y + offset_Y);
            Point2D ptSecond = new Point2D(pt2.X + offset_X, pt2.Y + offset_Y);
            List<Point2D> listPt = new List<Point2D>();
            listPt.Add(ptFirst);
            listPt.Add(ptSecond);
            return listPt;
        }

        public void ReducePoints(double epsilon)
        {
            List<int> cleanList = new List<int>();
            int j = 0;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].DistTo(points[j]) < epsilon)
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

        public List<Point2D> GetConvexHull()
        {
            List<Point2D> points = new List<Point2D>();
            List<Point2D> collection = new List<Point2D>();
            int num = 0;
            var source = Points.ToList();
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

        private static bool IsTurnRight(Point2D px, Point2D py, Point2D pz)
        {
            double num = 0;
            num = ((pz.Y - py.Y) * (py.X - px.X)) - ((py.Y - px.Y) * (pz.X - py.X));
            return (num < 0f);
        }

        public double Area
        {
            get
            {
                return Math.Abs(AlgebraicArea);
            }
        }

        public double AlgebraicArea
        {
            get
            {
                double temp = 0;
                for (int i = 0; i < Count; i++)
                {
                    int j = (i < Count - 1) ? (i + 1) : 0;
                    Vector2D v1 = points[i] - new Point2D(0, 0);
                    Vector2D v2 = points[j] - new Point2D(0, 0);
                    temp += v1.Kross(v2);
                }
                return 0.5 * temp;
            }
        }

        public void Reverse()
        {
            points = points.Reverse<Point2D>().ToList();
        }
    }

    public class Polyline : Point2DString
    {
        public Polyline(List<Point2D> pts)
            : base(pts)
        {
        }

        public Polyline(string ptStr)
            : base(ptStr)
        {
        }

        protected Polyline()
        {
        }

        public double Length
        {
            get
            {
                double temp = 0;
                for (int i = 0; i < Count - 1; i++)
                {
                    int j = i + 1;
                    temp += points[i].DistTo(points[j]);
                }
                return temp;
            }
        }

        public Polyline GetSubPoly(double start, double end)
        {
            start = (start < 0) ? 0 : start;
            end = (end > points.Count - 1) ? (points.Count - 1) : end;
            List<Point2D> pts = new List<Point2D>();
            double a = Math.Ceiling(start);
            double b = Math.Floor(end);
            for (int i = (int)a; i <= (int)b; i++)
            {
                pts.Add(points[i]);
            }
            if (start < a)
            {
                pts.Insert(0, this[start]);
            }
            if (end > b)
            {
                pts.Add(this[end]);
            }
            return new Polyline(pts);
        }

        public double DistToPoint(Point2D p, out Point2D footPos)
        {
            double minDist = this.points.Min(x => x.DistTo(p));
            Point2D minPoint = this.points.First(x => x.DistTo(p) == minDist);
            if (minPoint == this.points.First())
            {
                LineSeg seg1 = new LineSeg(this.points[0], this.points[1]);
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
            else if (minPoint == this.points.Last())
            {
                int n = this.points.Count;
                LineSeg seg1 = new LineSeg(this.points[n - 2], this.points[n - 1]);
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
                int index = this.points.IndexOf(minPoint);
                LineSeg seg1 = new LineSeg(this.points[index - 1], this.points[index]);
                LineSeg seg2 = new LineSeg(this.points[index + 1], this.points[index]);
                LineSegPointRelation relation1 = new LineSegPointRelation(seg1, p);
                LineSegPointRelation relation2 = new LineSegPointRelation(seg2, p);
                if (relation1.Inner)
                {
                    footPos = relation1.Foot;
                    return relation1.Dist;
                }
                else if (relation2.Inner)
                {
                    footPos = relation2.Foot;
                    return relation2.Dist;
                }
                else
                {
                    footPos = minPoint;
                    return minDist;
                }
            }
        }

        public double DistToPoint(Point2D p)
        {
            Point2D foot;
            return DistToPoint(p, out foot);
        }

        public double DistToPoint(Point2D p, int divs) // newly 20130523 近似法
        {
            double delta = (double)(this.points.Count - 1) / divs;
            var pts = Enumerable.Range(0, divs + 1).Select(i => this[i * delta]).ToList();
            return pts.Min(pt => pt.DistTo(p));
            //double minDist = double.MaxValue;
            //foreach (var pt in pts)
            //{
            //    double dist = pt.DistTo(p);
            //    if (dist < minDist)
            //    {
            //        minDist = dist;
            //    }
            //}
            //return minDist;
        }

        public bool IsSubOf(Polyline poly)
        {
            return points.All(x => poly.GetAllSegs().Any(y => y.IsPointOn(x)));
        }

        public Polyline ReversePoints()
        {
            return new Polyline(points.Reverse<Point2D>().ToList());
        }
    }

    public class Polygon : Point2DString
    {
        public Polygon(List<Point2D> pts)
            : base(pts)
        {
        }

        public Polygon(string ptStr)
            : base(ptStr)
        {
        }

        public Point2D Centroid // todo: 兼容退化成点的情况
        {
            get
            {
                if (points.Count == 1)
                {
                    return points[0];
                }
                else
                {
                    Vector2D temp = Vector2D.Zero;
                    double areaTwice = 0;
                    for (int i = 0; i < Count; i++)
                    {
                        int j = (i < Count - 1) ? (i + 1) : 0;
                        Vector2D v1 = points[i] - new Point2D(0, 0);
                        Vector2D v2 = points[j] - new Point2D(0, 0);
                        temp += v1.Kross(v2) / 3.0 * (v1 + v2);
                        areaTwice += v1.Kross(v2);
                    }
                    return ((1.0 / areaTwice) * temp).ToPoint();
                }
            }
        }

        public bool IsPointIn(Point2D p)
        {
            double temp = 0;
            for (int i = 0; i < Count; i++)
            {
                int j = (i < Count - 1) ? (i + 1) : 0;
                Vector2D v1 = points[i] - p;
                Vector2D v2 = points[j] - p;
                temp += v1.MinusPiToPiAngleTo(v2);
            }
            if (Math.Abs(Math.Abs(temp) - 2 * Math.PI) < 0.1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Extent2D
    {
        public Point2D min;
        public Point2D max;

        // 20111014
        [System.Xml.Serialization.XmlAttribute]
        public bool IsNull { get; set; }
        public static Extent2D Null { get { return new Extent2D { IsNull = true }; } }

        private Extent2D()
        {
        }

        public Extent2D(Point2D p1, Point2D p2)
        {
            min = new Point2D(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            max = new Point2D(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
            IsNull = false;
        }

        public Extent2D(IEnumerable<Point2D> pts)
        {
            min = new Point2D(pts.Min(p => p.X), pts.Min(p => p.Y));
            max = new Point2D(pts.Max(p => p.X), pts.Max(p => p.Y));
            IsNull = false;
        }

        public Extent2D(double minx, double miny, double maxx, double maxy)
        {
            min = new Point2D(minx, miny);
            max = new Point2D(maxx, maxy);
            IsNull = false;
        }

        public Extent2D(string extentStr)
        {
            string[] pp = extentStr.Split('|');
            min = new Point2D(pp[0]);
            max = new Point2D(pp[1]);
            IsNull = false;
        }

        public static Extent2D operator +(Extent2D e1, Extent2D e2)
        {
            if (e1.IsNull)
            {
                return e2;
            }
            else if (e2.IsNull)
            {
                return e1;
            }
            double minx = Math.Min(e1.min.X, e2.min.X);
            double miny = Math.Min(e1.min.Y, e2.min.Y);
            double maxx = Math.Max(e1.max.X, e2.max.X);
            double maxy = Math.Max(e1.max.Y, e2.max.Y);
            return new Extent2D(minx, miny, maxx, maxy);
        }

        // 20111014
        public void AddPoint(Point2D point)
        {
            if (this.IsNull)
            {
                min = point;
                max = point;
                IsNull = false;
            }
            else
            {
                min = new Point2D(Math.Min(min.X, point.X), Math.Min(min.Y, point.Y));
                max = new Point2D(Math.Max(max.X, point.X), Math.Max(max.Y, point.Y));
            }
        }

        // 20111014
        public void Add(Extent2D ext)
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
                min = new Point2D(Math.Min(min.X, ext.min.X), Math.Min(min.Y, ext.min.Y));
                max = new Point2D(Math.Max(max.X, ext.max.X), Math.Max(max.Y, ext.max.Y));
            }
        }

        // 20111015
        public double XRange { get { return max.X - min.X; } }
        public double YRange { get { return max.Y - min.Y; } }
        public double Area { get { return XRange * YRange; } }
        public Point2D Center { get { return (0.5 * (min.ToVector() + max.ToVector())).ToPoint(); } }

        public List<Point2D> GetGrid(double interval)
        {
            List<Point2D> pts = new List<Point2D>();
            for (double x = min.X + interval / 2; x < max.X; x += interval)
            {
                for (double y = min.Y + interval / 2; y < max.Y; y += interval)
                {
                    pts.Add(new Point2D(x, y));
                }
            }
            return pts;
        }

        public List<Point2D> GetHexGrid(double interval, double yScale = 1)
        {
            List<Point2D> pts = new List<Point2D>();
            bool even = false;
            double yInterval = yScale * interval;
            for (double y = min.Y + yInterval / 2; y < max.Y; y += yInterval)            
            {
                double x0 = even ? min.X : min.X + interval / 2;
                for (double x = x0; x < max.X; x += interval)
                {
                    pts.Add(new Point2D(x, y));
                }
                even = !even;
            }
            return pts;
        }

        public Point2D[,] GetGrid2d(double interval)
        {
            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            for (double x = min.X + interval / 2; x < max.X; x += interval)
            {
                xs.Add(x);
            }
            for (double y = min.Y + interval / 2; y < max.Y; y += interval)
            {
                ys.Add(y);
            }
            Point2D[,] pts = new Point2D[xs.Count, ys.Count];
            for (int i = 0; i < xs.Count; i++)
            {
                for (int j = 0; j < ys.Count; j++)
                {
                    pts[i, j] = new Point2D(xs[i], ys[j]);
                }
            }
            return pts;
        }

        public override string ToString()
        {
            return min.ToString() + "|" + max.ToString();
        }

        public bool IsPointIn(Point2D point)
        {
            return point.X >= min.X && point.X <= max.X && point.Y >= min.Y && point.Y <= max.Y;
        }

        public bool IsPointSetIn(Point2DString pSet)
        {
            double delta = 0.1;
            for (double d = 0; d < pSet.Count - 1 + delta; d += delta)
            {
                if (!this.IsPointIn(pSet[d]))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsPointSetCross(Point2DString pSet)
        {
            double delta = 0.1;
            for (double d = 0; d < pSet.Count - 1 + delta; d += delta)
            {
                if (this.IsPointIn(pSet[d]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsExtentsIn(Extent2D extents)
        {
            return extents.min.X >= min.X && extents.min.Y >= min.Y && extents.max.X <= max.X && extents.max.Y <= max.Y;
        }

        public bool IsCrossedBy(Extent2D extents)
        {
            //Point2D p1 = new Point2D(min.x, max.y);
            //Point2D p2 = new Point2D(max.x, min.y);
            //return extents.IsPointIn(p1) || extents.IsPointIn(p2) || extents.IsPointIn(min) || extents.IsPointIn(max) || IsExtentsIn(extents) || extents.IsExtentsIn(this);

            Extent2D sum = this + extents;
            return (sum.XRange <= this.XRange + extents.XRange) && (sum.YRange <= this.YRange + extents.YRange);
        }

        public bool IsCoveredBy(Extent2D extents) // newly 20130328
        {
            return extents.min.X >= min.X && extents.min.Y >= min.Y && extents.max.X <= max.X && extents.max.Y <= max.Y;
        }

        public Extent2D Extend(double factor)
        {
            return new Extent2D((Center.ToVector() + factor * (min - Center)).ToPoint(), (Center.ToVector() + factor * (max - Center)).ToPoint());
        }

        public static double CoincidenceEvaluation(Extent2D ext1, Extent2D ext2) // newly 20130402
        {
            double dist = ext1.Center.DistTo(ext2.Center);
            double minArea = Math.Min(ext1.Area, ext2.Area);
            return dist * dist / minArea;
        }
    }

    public class LineSeg
    {
        public Point2D[] p = new Point2D[2];

        public LineSeg(Point2D a, Point2D b)
        {
            p[0] = a;
            p[1] = b;
        }

        public double Length
        {
            get
            {
                return p[0].DistTo(p[1]);
            }
        }

        public Point2D GetDivPoint(double lambda)
        {
            if (double.IsInfinity(lambda))
            {
                return p[1];
            }
            else
            {
                return ((1 / (1 + lambda)) * (p[0].ToVector() + lambda * p[1].ToVector())).ToPoint();
            }
        }

        public bool IsPointOn(Point2D point)
        {
            if (Math.Abs((point - p[0]).Kross(point - p[1]) - 0) < 0.001)
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
        private Point2D A, B, C, D;
        public double lambda, miu;

        public LineSegIntersect(LineSeg l1, LineSeg l2)
        {
            A = l1.p[0];
            B = l1.p[1];
            C = l2.p[0];
            D = l2.p[1];
        }

        public bool Intersect()
        {
            if ((A - B).Kross(C - D) == 0)
            {
                return false;
            }

            if (C.X == D.X)
            {
                lambda = (C.X - A.X) / (B.X - C.X);
                double y = (A.Y + lambda * B.Y) / (1 + lambda);
                miu = (y - C.Y) / (D.Y - y);
            }
            else if (A.X == B.X)
            {
                miu = (A.X - C.X) / (D.X - A.X);
                double y = (C.Y + miu * D.Y) / (1 + miu);
                lambda = (y - A.Y) / (B.Y - y);
            }
            else if (C.Y == D.Y)
            {
                lambda = (C.Y - A.Y) / (B.Y - C.Y);
                double x = (A.X + lambda * B.X) / (1 + lambda);
                miu = (x - C.X) / (D.X - x);
            }
            else if (A.Y == B.Y)
            {
                miu = (A.Y - C.Y) / (D.Y - A.Y);
                double x = (C.X + miu * D.X) / (1 + miu);
                lambda = (x - A.X) / (B.X - x);
            }
            else
            {
                lambda = (C.X * A.Y - D.X * A.Y - A.X * C.Y + D.X * C.Y + A.X * D.Y - C.X * D.Y) / (-C.X * B.Y + D.X * B.Y + B.X * C.Y - D.X * C.Y - B.X * D.Y + C.X * D.Y);
                miu = (-B.X * A.Y + C.X * A.Y + A.X * B.Y - C.X * B.Y - A.X * C.Y + B.X * C.Y) / (B.X * A.Y - D.X * A.Y - A.X * B.Y + D.X * B.Y + A.X * D.Y - B.X * D.Y); // from Mathematica
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

        public Point2D Coincidence
        {
            get
            {
                return new LineSeg(A, B).GetDivPoint(lambda);
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
        private Point2D A, B, C, D;
        private double lambda;

        public LineSegPointRelation(LineSeg line, Point2D point)
        {
            A = line.p[0];
            B = line.p[1];
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
                return C.DistTo(D);
            }
        }

        public Point2D Foot
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

        public Point2D Transform(Point2D p)
        {
            return new Point2D((p.X - X21) * ScaleX21, (p.Y - Y21) * ScaleY21);
        }

        public Point2D TransformBack(Point2D p)
        {
            return new Point2D(p.X / ScaleX21 + X21, p.Y / ScaleY21 + Y21);
        }
    }

    public static class Algorithms
    {
        public static double TriangleInterpolate(double x, double y, Point3D a, Point3D b, Point3D c)
        {
            double x1 = a.X, x2 = b.X, x3 = c.X, y1 = a.Y, y2 = b.Y, y3 = c.Y, z1 = a.Z, z2 = b.Z, z3 = c.Z;
            return (x2 * y * z1 - x3 * y * z1 - x * y2 * z1 + x3 * y2 * z1 + x * y3 * z1 - x2 * y3 * z1 -
                    x1 * y * z2 + x3 * y * z2 + x * y1 * z2 - x3 * y1 * z2 - x * y3 * z2 + x1 * y3 * z2 +
                    x1 * y * z3 - x2 * y * z3 - x * y1 * z3 + x2 * y1 * z3 + x * y2 * z3 - x1 * y2 * z3)
                    / (x2 * y1 - x3 * y1 - x1 * y2 + x3 * y2 + x1 * y3 - x2 * y3);
        }
    }
}
