using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongJi.Geometry
{
    public static class DecimalPlaces
    {
        public static int Places { get; set; }

        static DecimalPlaces()
        {
            Places = 2;
        }

        public static string ToDecimalString(this double num)
        {
            string str = num.ToString();
            int i = str.IndexOf('.');
            int j = i + Places + 1;
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

    /// <summary>
    /// 表示二维点。
    /// </summary>
    public struct Point2D
    {
        [System.Xml.Serialization.XmlAttribute]
        public double x;
        [System.Xml.Serialization.XmlAttribute]
        public double y;

        /// <summary>
        /// 从坐标创建点。
        /// </summary>
        /// <param name="xx">x坐标</param>
        /// <param name="yy">y坐标</param>
        public Point2D(double xx, double yy)
        {
            x = xx;
            y = yy;
        }

        /// <summary>
        /// 从字符串创建点。
        /// </summary>
        /// <param name="ptStr">字符串</param>
        public Point2D(string ptStr)
        {
            string[] xy = ptStr.Split(',');
            x = Convert.ToDouble(xy[0]);
            y = Convert.ToDouble(xy[1]);
        }

        /// <summary>
        /// 点相等运算符。
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return p1.DistTo(p2) < 0.01;
        }

        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return p1.DistTo(p2) >= 0.01;
        }

        public static Vector2D operator -(Point2D p1, Point2D p2)
        {
            double a = p1.x - p2.x;
            double b = p1.y - p2.y;
            return new Vector2D(a, b);
        }

        public Point2D Move(Vector2D v)
        {
            return new Point2D(x + v.x, y + v.y);
        }

        public Vector2D VectorFromOrigin
        {
            get
            {
                return new Vector2D(x, y);
            }
        }

        public double DistTo(Point2D p2)
        {
            return Math.Sqrt(Math.Pow(x - p2.x, 2) + Math.Pow(y - p2.y, 2));
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", x.ToDecimalString(), y.ToDecimalString());
        }

        public Point3D ToPoint3D()
        {
            return new Point3D(x, y, 0);
        }

        private static Point2D _null = new Point2D(double.NaN, double.NaN);
        public static Point2D Null { get { return _null; } }
        public static bool IsNull(Point2D p)
        {
            return double.IsNaN(p.x);
        }
    }

    public struct Vector2D
    {
        public double x;
        public double y;

        public Vector2D(double xx, double yy)
        {
            x = xx;
            y = yy;
        }

        public Point2D ToPoint()
        {
            return new Point2D(x, y);
        }

        public static Vector2D operator +(Vector2D p1, Vector2D p2)
        {
            double a = p1.x + p2.x;
            double b = p1.y + p2.y;
            return new Vector2D(a, b);
        }

        public static Vector2D operator -(Vector2D p1, Vector2D p2)
        {
            double a = p1.x - p2.x;
            double b = p1.y - p2.y;
            return new Vector2D(a, b);
        }

        public static Vector2D operator *(double lambda, Vector2D p)
        {
            return new Vector2D(lambda * p.x, lambda * p.y);
        }

        public double Abs
        {
            get
            {
                return Math.Sqrt(x * x + y * y);
            }
        }

        public Vector2D Unit
        {
            get
            {
                return new Vector2D(x / Abs, y / Abs);
            }
        }

        public double Dot(Vector2D v1)
        {
            return x * v1.x + y * v1.y;
        }

        public double ScalarCross(Vector2D v1)
        {
            return x * v1.y - y * v1.x;
        }

        public double DirAngleZeroTo2Pi()
        {
            double angle = this.ZeroToPiAngleTo(Vector2D.XAxis);
            if (this.y < 0)
            {
                angle = 2 * Math.PI - angle;
            }
            return angle;
        }

        public double ZeroToPiAngleTo(Vector2D v1)
        {
            double cosTheta = this.Dot(v1) / (this.Abs * v1.Abs);
            return Math.Acos(cosTheta);
        }

        public double ZeroTo2PiAngleTo(Vector2D v1)
        {
            double angle0 = this.DirAngleZeroTo2Pi();
            double angle1 = v1.DirAngleZeroTo2Pi();
            double angleDelta = angle1 - angle0;
            if (angleDelta < 0) angleDelta = angleDelta + 2 * Math.PI;
            return angleDelta;
        }

        public double MinusPiToPiAngleTo(Vector2D v1)
        {
            double angle0 = this.DirAngleZeroTo2Pi();
            double angle1 = v1.DirAngleZeroTo2Pi();
            double angleDelta = angle1 - angle0;
            if (angleDelta < -Math.PI) angleDelta = angleDelta + 2 * Math.PI;
            else if (angleDelta > Math.PI) angleDelta = angleDelta - 2 * Math.PI;
            return angleDelta;
        }

        public static Vector2D Zero
        {
            get
            {
                return new Vector2D(0, 0);
            }
        }

        public static Vector2D XAxis
        {
            get
            {
                return new Vector2D(1, 0);
            }
        }

        public static Vector2D YAxis
        {
            get
            {
                return new Vector2D(0, 1);
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", x.ToDecimalString(), y.ToDecimalString());
        }

        public Vector2D Normalize()
        {
            return new Vector2D(x / Abs, y / Abs);
        }

        public Vector2D Rotate(double angle)
        {
            return new Vector2D(x * Math.Cos(angle) + y * Math.Sin(angle), -x * Math.Sin(angle) + y * Math.Cos(angle));
        }
    }

    public struct Point3D
    {
        public double x;
        public double y;
        public double z;

        public Point3D(double xx, double yy, double zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }

        public static Vector3D operator -(Point3D p1, Point3D p2)
        {
            double a = p1.x - p2.x;
            double b = p1.y - p2.y;
            double c = p1.z - p2.z;
            return new Vector3D(a, b, c);
        }

        public Point3D Move(Vector3D v)
        {
            return new Point3D(x + v.x, y + v.y, z + v.z);
        }

        public Point3D Transform(Geometry3D.Matrix4 m)
        {
            return m.MultiplyVector3(this);
        }

        public void TransformSelf(Geometry3D.Matrix4 m)
        {
            var u = m.MultiplyVector3(this);
            x = u.x;
            y = u.y;
            z = u.z;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", x.ToDecimalString(), y.ToDecimalString(), z.ToDecimalString());
        }

        public Point2D ToPoint2D()
        {
            return new Point2D(x, y);
        }
    }

    public struct Vector3D
    {
        public double x;
        public double y;
        public double z;

        public Vector3D(double xx, double yy, double zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }

        public static Vector3D operator +(Vector3D p1, Vector3D p2)
        {
            double a = p1.x + p2.x;
            double b = p1.y + p2.y;
            double c = p1.z + p2.z;
            return new Vector3D(a, b, c);
        }

        public static Vector3D operator -(Vector3D p1, Vector3D p2)
        {
            double a = p1.x - p2.x;
            double b = p1.y - p2.y;
            double c = p1.z - p2.z;
            return new Vector3D(a, b, c);
        }

        public static Vector3D operator *(double lambda, Vector3D p)
        {
            return new Vector3D(lambda * p.x, lambda * p.y, lambda * p.z);
        }

        public double Abs
        {
            get
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public Vector3D Unit
        {
            get
            {
                return new Vector3D(x / Abs, y / Abs, z / Abs);
            }
        }

        public double Dot(Vector3D v1)
        {
            return x * v1.x + y * v1.y + z * v1.z;
        }

        public Vector3D Cross(Vector3D v1)
        {
            double xx = y * v1.z - z * v1.y;
            double yy = z * v1.x - x * v1.z;
            double zz = x * v1.y - y * v1.x;
            return new Vector3D(xx, yy, zz);
        }

        public static Vector3D Zero
        {
            get
            {
                return new Vector3D(0, 0, 0);
            }
        }

        public static Vector3D XAxis
        {
            get
            {
                return new Vector3D(1, 0, 0);
            }
        }

        public static Vector3D YAxis
        {
            get
            {
                return new Vector3D(0, 1, 0);
            }
        }

        public static Vector3D ZAxis
        {
            get
            {
                return new Vector3D(0, 0, 1);
            }
        }

        public Vector3D Normalize()
        {
            return new Vector3D(x / Abs, y / Abs, z / Abs);
        }

        public double ZeroToPiAngleTo(Vector3D v1)
        {
            double cosTheta = this.Dot(v1) / (this.Abs * v1.Abs);
            return Math.Acos(cosTheta);
        }

        public Vector3D Transform(Geometry3D.Matrix4 m)
        {
            return m.MultiplyVector3(this);
        }

        public void TransformSelf(Geometry3D.Matrix4 m)
        {
            var u = m.MultiplyVector3(this);
            x = u.x;
            y = u.y;
            z = u.z;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", x.ToDecimalString(), y.ToDecimalString(), z.ToDecimalString());
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
                double distance = Math.Sqrt((points[i].x - points[i + 1].x) * (points[i].x - points[i + 1].x) + (points[i].y - points[i + 1].y) * (points[i].y - points[i + 1].y));
                if (tempDist + distance < dist)
                {
                    tempDist = tempDist + distance;
                    continue;
                }
                else
                {
                    isValued = true;
                    if (points[i + 1].x == points[i].x)
                    {
                        Derivative = double.PositiveInfinity;
                    }
                    else
                    {
                        Derivative = (points[i + 1].y - points[i].y) / (points[i + 1].x - points[i].x);
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
                return Math.Sqrt((p.x - points[0].x) * (p.x - points[0].x) + (p.y - points[0].y) * (p.y - points[0].y));
            }
            else
            {
                int count = points.Count;
                for (int i = 0; i < count - 1; i++)
                {
                    if (points[i].x == points[i + 1].x && p.x == points[i].x)
                    {
                        if ((p.y - points[i].y) * (p.y - points[i + 1].y) <= 0)
                        {
                            distance = distance + Math.Sqrt((p.x - points[i].x) * (p.x - points[i].x) + (p.y - points[i].y) * (p.y - points[i].y));
                            isOnLine = true;
                            break;
                        }
                        else
                        {
                            isOnLine = false;
                            break;
                        }
                    }

                    else if (points[i].y == points[i + 1].y && p.y == points[i].y)
                    {
                        if ((p.x - points[i].x) * (p.x - points[i + 1].x) <= 0)
                        {
                            distance = distance + Math.Sqrt((p.x - points[i].x) * (p.x - points[i].x) + (p.y - points[i].y) * (p.y - points[i].y));
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
                        if ((p.y - points[i].y) / (p.x - points[i].x) == (p.y - points[i + 1].y) / (p.x - points[i + 1].x) && (p.y - points[i].y != 0) && (points[i + 1].x - points[i].x) * (p.x - points[i].x) >= 0)
                        {
                            distance = distance + Math.Sqrt((p.x - points[i].x) * (p.x - points[i].x) + (p.y - points[i].y) * (p.y - points[i].y));
                            isOnLine = true;
                            break;
                        }
                        else
                        {
                            distance = distance + Math.Sqrt((points[i + 1].x - points[i].x) * (points[i + 1].x - points[i].x) + (points[i + 1].y - points[i].y) * (points[i + 1].y - points[i].y));
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
                double distance = Math.Sqrt((points[i].x - points[i + 1].x) * (points[i].x - points[i + 1].x) + (points[i].y - points[i + 1].y) * (points[i].y - points[i + 1].y));
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
                    if (points[i].x == points[i + 1].x)
                    {
                        pt.x = points[i].x;
                    }
                    else
                    {
                        pt.x = (points[i + 1].x - points[i].x) * proportion + points[i].x;
                    }

                    if (points[i].y == points[i + 1].y)
                    {
                        pt.y = points[i].y;
                    }
                    else
                    {
                        pt.y = (points[i + 1].y - points[i].y) * proportion + points[i].y;
                    }
                    break;
                }
            }

            if (pt.x == -1 && pt.y == -1)
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
                Point2D pt = new Point2D(points[0].x + offset, points[0].y);
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

                        Point2D d0 = new Point2D(p11.x - p10.x, p11.y - p10.y);
                        Point2D d1 = new Point2D(p20.x - p21.x, p20.y - p21.y);

                        listPt.Add(GetIntersection(p10, p21, d0, d1));
                    }
                }
            }
            Point2DString ptSet = new Point2DString(listPt);
            return ptSet;
        }

        private Point2D GetIntersection(Point2D p0, Point2D p1, Point2D d0, Point2D d1)
        {
            Point2D p = new Point2D(p1.x - p0.x, p1.y - p0.y);
            double kross = d0.x * d1.y - d0.y * d1.x;
            double sqrKross = kross * kross;
            double sqrLen0 = d0.x * d0.x + d0.y * d0.y;
            double sqrLen1 = d1.x * d1.x + d1.y * d1.y;

            //lines are not parallel
            if (sqrKross > 0.0001 * sqrLen0 * sqrLen1)
            {
                double s = (p.x * d1.y - p.y * d1.x) / kross;
                Point2D pt = new Point2D(p0.x + d0.x * s, p0.y + d0.y * s);
                return pt;
            }

            //lines are parallel
            double sqrLenE = p.x * p.x + p.y * p.y;
            kross = p.x * d0.y - p.y * d0.x;
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
            double offset_X = (-1) * (pt2.y - pt1.y) / (Math.Sqrt((pt2.x - pt1.x) * (pt2.x - pt1.x) + (pt2.y - pt1.y) * (pt2.y - pt1.y))) * offset;
            double offset_Y = (pt2.x - pt1.x) / (Math.Sqrt((pt2.x - pt1.x) * (pt2.x - pt1.x) + (pt2.y - pt1.y) * (pt2.y - pt1.y))) * offset;
            Point2D ptFirst = new Point2D(pt1.x + offset_X, pt1.y + offset_Y);
            Point2D ptSecond = new Point2D(pt2.x + offset_X, pt2.y + offset_Y);
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
            source.Sort((p1, p2) => (p1.x - p2.x == 0) ? (int)(p1.y - p2.y) : (int)(p1.x - p2.x));

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
            num = ((pz.y - py.y) * (py.x - px.x)) - ((py.y - px.y) * (pz.x - py.x));
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
                    temp += v1.ScalarCross(v2);
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
                        temp += v1.ScalarCross(v2) / 3.0 * (v1 + v2);
                        areaTwice += v1.ScalarCross(v2);
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
            min = new Point2D(Math.Min(p1.x, p2.x), Math.Min(p1.y, p2.y));
            max = new Point2D(Math.Max(p1.x, p2.x), Math.Max(p1.y, p2.y));
            IsNull = false;
        }

        public Extent2D(IEnumerable<Point2D> pts)
        {
            min = new Point2D(pts.Min(p => p.x), pts.Min(p => p.y));
            max = new Point2D(pts.Max(p => p.x), pts.Max(p => p.y));
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
            double minx = Math.Min(e1.min.x, e2.min.x);
            double miny = Math.Min(e1.min.y, e2.min.y);
            double maxx = Math.Max(e1.max.x, e2.max.x);
            double maxy = Math.Max(e1.max.y, e2.max.y);
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
                min = new Point2D(Math.Min(min.x, point.x), Math.Min(min.y, point.y));
                max = new Point2D(Math.Max(max.x, point.x), Math.Max(max.y, point.y));
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
                min = new Point2D(Math.Min(min.x, ext.min.x), Math.Min(min.y, ext.min.y));
                max = new Point2D(Math.Max(max.x, ext.max.x), Math.Max(max.y, ext.max.y));
            }
        }

        // 20111015
        public double XRange { get { return max.x - min.x; } }
        public double YRange { get { return max.y - min.y; } }
        public double Area { get { return XRange * YRange; } }
        public Point2D Center { get { return (0.5 * (min.VectorFromOrigin + max.VectorFromOrigin)).ToPoint(); } }

        public List<Point2D> GetGrid(double interval)
        {
            List<Point2D> pts = new List<Point2D>();
            for (double x = min.x + interval / 2; x < max.x; x += interval)
            {
                for (double y = min.y + interval / 2; y < max.y; y += interval)
                {
                    pts.Add(new Point2D(x, y));
                }
            }
            return pts;
        }

        public Point2D[,] GetGrid2d(double interval)
        {
            List<double> xs = new List<double>();
            List<double> ys = new List<double>();
            for (double x = min.x + interval / 2; x < max.x; x += interval)
            {
                xs.Add(x);
            }
            for (double y = min.y + interval / 2; y < max.y; y += interval)
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
            return point.x >= min.x && point.x <= max.x && point.y >= min.y && point.y <= max.y;
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
            return extents.min.x >= min.x && extents.min.y >= min.y && extents.max.x <= max.x && extents.max.y <= max.y;
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
            return extents.min.x >= min.x && extents.min.y >= min.y && extents.max.x <= max.x && extents.max.y <= max.y;
        }

        public Extent2D Extend(double factor)
        {
            return new Extent2D((Center.VectorFromOrigin + factor * (min - Center)).ToPoint(), (Center.VectorFromOrigin + factor * (max - Center)).ToPoint());
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
                return ((1 / (1 + lambda)) * (p[0].VectorFromOrigin + lambda * p[1].VectorFromOrigin)).ToPoint();
            }
        }

        public bool IsPointOn(Point2D point)
        {
            if (Math.Abs((point - p[0]).ScalarCross(point - p[1]) - 0) < 0.001)
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
            if ((A - B).ScalarCross(C - D) == 0)
            {
                return false;
            }

            if (C.x == D.x)
            {
                lambda = (C.x - A.x) / (B.x - C.x);
                double y = (A.y + lambda * B.y) / (1 + lambda);
                miu = (y - C.y) / (D.y - y);
            }
            else if (A.x == B.x)
            {
                miu = (A.x - C.x) / (D.x - A.x);
                double y = (C.y + miu * D.y) / (1 + miu);
                lambda = (y - A.y) / (B.y - y);
            }
            else if (C.y == D.y)
            {
                lambda = (C.y - A.y) / (B.y - C.y);
                double x = (A.x + lambda * B.x) / (1 + lambda);
                miu = (x - C.x) / (D.x - x);
            }
            else if (A.y == B.y)
            {
                miu = (A.y - C.y) / (D.y - A.y);
                double x = (C.x + miu * D.x) / (1 + miu);
                lambda = (x - A.x) / (B.x - x);
            }
            else
            {
                lambda = (C.x * A.y - D.x * A.y - A.x * C.y + D.x * C.y + A.x * D.y - C.x * D.y) / (-C.x * B.y + D.x * B.y + B.x * C.y - D.x * C.y - B.x * D.y + C.x * D.y);
                miu = (-B.x * A.y + C.x * A.y + A.x * B.y - C.x * B.y - A.x * C.y + B.x * C.y) / (B.x * A.y - D.x * A.y - A.x * B.y + D.x * B.y + A.x * D.y - B.x * D.y); // from Mathematica
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
                lambda = (A.x - C.x) / (C.x - B.x);
                if (double.IsNaN(lambda))
                {
                    lambda = (A.y - C.y) / (C.y - B.y);
                }
            }
            else
            {
                lambda = (-A.x * A.x + A.x * B.x + A.x * C.x - B.x * C.x - A.y * A.y + A.y * B.y + A.y * C.y - B.y * C.y) / (A.x * B.x - B.x * B.x - A.x * C.x + B.x * C.x + A.y * B.y - B.y * B.y - A.y * C.y + B.y * C.y); // from Mathematica
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
            return new Point2D((p.x - X21) * ScaleX21, (p.y - Y21) * ScaleY21);
        }

        public Point2D TransformBack(Point2D p)
        {
            return new Point2D(p.x / ScaleX21 + X21, p.y / ScaleY21 + Y21);
        }
    }

    public static class Algorithms
    {
        public static double TriangleInterpolate(double x, double y, Point3D a, Point3D b, Point3D c)
        {
            double x1 = a.x, x2 = b.x, x3 = c.x, y1 = a.y, y2 = b.y, y3 = c.y, z1 = a.z, z2 = b.z, z3 = c.z;
            return (x2 * y * z1 - x3 * y * z1 - x * y2 * z1 + x3 * y2 * z1 + x * y3 * z1 - x2 * y3 * z1 -
                    x1 * y * z2 + x3 * y * z2 + x * y1 * z2 - x3 * y1 * z2 - x * y3 * z2 + x1 * y3 * z2 +
                    x1 * y * z3 - x2 * y * z3 - x * y1 * z3 + x2 * y1 * z3 + x * y2 * z3 - x1 * y2 * z3)
                    / (x2 * y1 - x3 * y1 - x1 * y2 + x3 * y2 + x1 * y3 - x2 * y3);
        }
    }
}
