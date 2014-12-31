using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dreambuild.Geometry
{
    public struct Vector3D
    {
        [XmlAttribute]
        public double X;
        [XmlAttribute]
        public double Y;
        [XmlAttribute]
        public double Z;

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            double a = v1.X + v2.X;
            double b = v1.Y + v2.Y;
            double c = v1.Z + v2.Z;
            return new Vector3D(a, b, c);
        }

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            double a = v1.X - v2.X;
            double b = v1.Y - v2.Y;
            double c = v1.Z - v2.Z;
            return new Vector3D(a, b, c);
        }

        public static Vector3D operator *(double lambda, Vector3D v)
        {
            return new Vector3D(lambda * v.X, lambda * v.Y, lambda * v.Z);
        }

        public double Abs
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public Vector3D Unit
        {
            get
            {
                return new Vector3D(X / Abs, Y / Abs, Z / Abs);
            }
        }

        public double Dot(Vector3D v1)
        {
            return X * v1.X + Y * v1.Y + Z * v1.Z;
        }

        public Vector3D Cross(Vector3D v1)
        {
            double xx = Y * v1.Z - Z * v1.Y;
            double yy = Z * v1.X - X * v1.Z;
            double zz = X * v1.Y - Y * v1.X;
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
            return Unit;
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

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", X.ToDecimalString(), Y.ToDecimalString(), Z.ToDecimalString());
        }
    }
}
