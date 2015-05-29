using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dreambuild.Geometry
{
    /// <summary>
    /// 2-d point
    /// </summary>
    public struct Point2D
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        [XmlAttribute]
        public double X;

        /// <summary>
        /// Y coordinate
        /// </summary>
        [XmlAttribute]
        public double Y;

        /// <summary>
        /// point from x, y coordinates
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// point from string
        /// </summary>
        /// <param name="ptStr">string notation</param>
        public Point2D(string ptStr)
        {
            string[] xy = ptStr.Split(',');
            X = Convert.ToDouble(xy[0]);
            Y = Convert.ToDouble(xy[1]);
        }

        /// <summary>
        /// equality operator
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return p1.DistTo(p2) < Parameters.Epsilon;
        }

        /// <summary>
        /// inequality operator
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return p1.DistTo(p2) >= Parameters.Epsilon;
        }

        /// <summary>
        /// subtraction operator, resulting in a 2-d vector
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector2D operator -(Point2D p1, Point2D p2)
        {
            double a = p1.X - p2.X;
            double b = p1.Y - p2.Y;
            return new Vector2D(a, b);
        }

        /// <summary>
        /// addition with a 2-d vector
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Point2D operator +(Point2D p, Vector2D v)
        {
            double a = p.X + v.X;
            double b = p.Y + v.Y;
            return new Point2D(a, b);
        }

        /// <summary>
        /// get the result of moving the point by a vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Point2D Move(Vector2D v)
        {
            return this + v;
        }

        /// <summary>
        /// get the vector form of the point
        /// </summary>
        /// <returns></returns>
        public Vector2D ToVector()
        {
            return new Vector2D(X, Y);
        }

        /// <summary>
        /// get the distance from the point to another point
        /// </summary>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double DistTo(Point2D p2)
        {
            return Math.Sqrt(Math.Pow(X - p2.X, 2) + Math.Pow(Y - p2.Y, 2));
        }

        /// <summary>
        /// get the string notation of the point
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", X.ToDecimalString(), Y.ToDecimalString());
        }

        /// <summary>
        /// get the 3-d point form
        /// </summary>
        /// <returns></returns>
        public Point3D ToPoint3D()
        {
            return new Point3D(X, Y, 0);
        }

        private static Point2D _null = new Point2D(double.NaN, double.NaN);
        /// <summary>
        /// get a point of invalid value
        /// </summary>
        public static Point2D Null { get { return _null; } }

        /// <summary>
        /// determine whether a point is invalid
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsNull(Point2D p)
        {
            return double.IsNaN(p.X);
        }
    }
}
