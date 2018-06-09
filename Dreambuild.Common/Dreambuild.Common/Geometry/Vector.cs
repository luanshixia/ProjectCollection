using Dreambuild.Extensions;
using System;
using System.Linq;

namespace Dreambuild.Geometry
{
    /// <summary>
    /// Utility functions.
    /// </summary>
    public static class Utils
    {
        public const double Epsilon = 0.000001;
        private static Random _rand = new Random();

        public static double Random(double min, double max)
        {
            var rand = _rand.NextDouble();
            return min + rand * (max - min);
        }

        public static bool FloatEquals(this double a, double b)
        {
            return Math.Abs(a - b) < Epsilon;
        }
    }

    /// <summary>
    /// Point or vector of 2d or 3d.
    /// </summary>
    public struct Vector
    {
        public double X;
        public double Y;
        public double Z;

        public Vector(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double[] Array()
        {
            return new[] { X, Y, Z };
        }

        public double Get(int dimension)
        {
            return Array()[dimension];
        }

        public bool Equals(Vector v)
        {
            return this.X.FloatEquals(v.X) && this.Y.FloatEquals(v.Y) && this.Z.FloatEquals(v.Z);
        }

        public Vector Copy()
        {
            return this;
        }

        public double Abs
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public Vector Unit
        {
            get
            {
                return new Vector(X / Abs, Y / Abs, Z / Abs);
            }
        }

        public double Mag()
        {
            return Abs;
        }

        public double MagSq()
        {
            return X * X + Y * Y + Z * Z;
        }

        public Vector Add(Vector v)
        {
            return this + v;
        }

        public Vector Sub(Vector v)
        {
            return this - v;
        }

        public Vector Mult(double n)
        {
            return n * this;
        }

        public Vector Div(double n)
        {
            return Mult(1 / n);
        }

        public double Dist(Vector v)
        {
            return this.Sub(v).Mag();
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            double a = v1.X + v2.X;
            double b = v1.Y + v2.Y;
            double c = v1.Z + v2.Z;
            return new Vector(a, b, c);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            double a = v1.X - v2.X;
            double b = v1.Y - v2.Y;
            double c = v1.Z - v2.Z;
            return new Vector(a, b, c);
        }

        public static Vector operator *(double lambda, Vector v)
        {
            return new Vector(lambda * v.X, lambda * v.Y, lambda * v.Z);
        }

        public double Dot(Vector v1)
        {
            return X * v1.X + Y * v1.Y + Z * v1.Z;
        }

        public Vector Cross(Vector v1)
        {
            double xx = Y * v1.Z - Z * v1.Y;
            double yy = Z * v1.X - X * v1.Z;
            double zz = X * v1.Y - Y * v1.X;
            return new Vector(xx, yy, zz);
        }

        public double Kross(Vector v)
        {
            return X * v.Y - Y * v.X;
        }

        public Vector Normalize()
        {
            return Unit;
        }

        public Vector Limit(double max)
        {
            return this.Mag() > max ? this.SetMag(max) : this.Copy();
        }

        public Vector SetMag(double len)
        {
            return this.Normalize().Mult(len);
        }

        public double Heading()
        {
            return Math.Atan2(Y, X);
        }

        public double AngleTo(Vector v, AngleRange mode = AngleRange.ZeroToPi)
        {
            if (mode == AngleRange.ZeroToPi)
            {
                return Vector.AngleBetween(this, v);
            }
            var dir0 = this.Heading();
            var dir1 = v.Heading();
            var angle = dir1 - dir0;
            if (mode == AngleRange.ZeroTo2Pi)
            {
                if (angle < 0)
                {
                    angle += 2 * Math.PI;
                }
            }
            else if (mode == AngleRange.MinusPiToPi)
            {
                if (angle < -Math.PI)
                {
                    angle += 2 * Math.PI;
                }
                else if (angle > Math.PI)
                {
                    angle -= 2 * Math.PI;
                }
            }
            return angle;
        }

        public Vector Rotate(double theta)
        {
            var x = X * Math.Cos(theta) - Y * Math.Sin(theta);
            var y = this.X * Math.Sin(theta) + Y * Math.Cos(theta);
            var z = Z;
            return new Vector(x, y, z);
        }

        public Vector Lerp(Vector v, double amt)
        {
            return this.Add(v.Sub(this).Mult(amt));
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", X.ToDecimalString(), Y.ToDecimalString(), Z.ToDecimalString());
        }

        public static Vector Parse(string str)
        {
            var ns = str.Split(',').Select(s => s.TryParseToDouble()).ToArray();
            if (ns.Length >= 3)
            {
                return new Vector(ns[0], ns[1], ns[2]);
            }
            else if (ns.Length == 2)
            {
                return new Vector(ns[0], ns[1]);
            }
            else
            {
                return new Vector(ns[0]);
            }
        }

        public static Vector Random2D()
        {
            var theta = Utils.Random(0, Math.PI * 2);
            return Vector.FromAngle(theta);
        }

        public static Vector Random3D()
        {
            var theta = Utils.Random(0, Math.PI * 2);
            var phi = Utils.Random(-Math.PI / 2, Math.PI / 2);
            var xy = Vector.XAxis.Rotate(theta);
            var z = Vector.XAxis.Rotate(phi);
            return new Vector(xy.X, xy.Y, z.Y);
        }

        public static Vector FromAngle(double angle)
        {
            return Vector.XAxis.Rotate(angle);
        }

        public static double AngleBetween(Vector v1, Vector v2)
        {
            return Math.Acos(v1.Dot(v2) / (v1.Abs * v2.Abs));
        }

        public static Vector Zero
        {
            get
            {
                return new Vector(0, 0, 0);
            }
        }

        public static Vector XAxis
        {
            get
            {
                return new Vector(1, 0, 0);
            }
        }

        public static Vector YAxis
        {
            get
            {
                return new Vector(0, 1, 0);
            }
        }

        public static Vector ZAxis
        {
            get
            {
                return new Vector(0, 0, 1);
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(Vector) && this.Equals((Vector)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Vector left, Vector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector left, Vector right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// Range of angle value.
    /// </summary>
    public enum AngleRange
    {
        ZeroToPi,
        ZeroTo2Pi,
        MinusPiToPi
    }
}
