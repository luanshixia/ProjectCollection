using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dreambuild.Geometry
{
    public struct Point3D
    {
        [XmlAttribute]
        public double X;
        [XmlAttribute]
        public double Y;
        [XmlAttribute]
        public double Z;

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3D operator -(Point3D p1, Point3D p2)
        {
            double a = p1.X - p2.X;
            double b = p1.Y - p2.Y;
            double c = p1.Z - p2.Z;
            return new Vector3D(a, b, c);
        }

        public Point3D Move(Vector3D v)
        {
            return new Point3D(X + v.X, Y + v.Y, Z + v.Z);
        }

        public Point3D Transform(Geometry3D.Matrix4 m)
        {
            return m.MultiplyVector3(this);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", X.ToDecimalString(), Y.ToDecimalString(), Z.ToDecimalString());
        }

        public Point2D ToPoint2D()
        {
            return new Point2D(X, Y);
        }
    }
}
