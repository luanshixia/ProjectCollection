using Dreambuild.Extensions;
using Dreambuild.Geometry;
using System;
using System.Linq;

namespace Dreambuild.Geometry3D
{
    /// <summary>
    /// 4x4 metrix.
    /// </summary>
    /// <remarks>
    /// Special thanks to THREE.js.
    /// </remarks>
    public class Matrix4
    {
        public double n11;
        public double n12;
        public double n13;
        public double n14;
        public double n21;
        public double n22;
        public double n23;
        public double n24;
        public double n31;
        public double n32;
        public double n33;
        public double n34;
        public double n41;
        public double n42;
        public double n43;
        public double n44;

        public Matrix4()
        {
        }

        public Matrix4(Matrix4 that)
        {
            this.n11 = that.n11; this.n12 = that.n12; this.n13 = that.n13; this.n14 = that.n14;
            this.n21 = that.n21; this.n22 = that.n22; this.n23 = that.n23; this.n24 = that.n24;
            this.n31 = that.n31; this.n32 = that.n32; this.n33 = that.n33; this.n34 = that.n34;
            this.n41 = that.n41; this.n42 = that.n42; this.n43 = that.n43; this.n44 = that.n44;
        }

        public Matrix4(double n11, double n12, double n13, double n14, double n21, double n22, double n23, double n24, double n31, double n32, double n33, double n34, double n41, double n42, double n43, double n44)
        {
            this.n11 = n11; this.n12 = n12; this.n13 = n13; this.n14 = n14;
            this.n21 = n21; this.n22 = n22; this.n23 = n23; this.n24 = n24;
            this.n31 = n31; this.n32 = n32; this.n33 = n33; this.n34 = n34;
            this.n41 = n41; this.n42 = n42; this.n43 = n43; this.n44 = n44;
        }

        private static readonly Matrix4 _identity = new Matrix4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
        public static Matrix4 Identity { get { return _identity; } }

        public Matrix4 Copy()
        {
            return new Matrix4(this);
        }

        public static Matrix4 LookAt(Vector eye, Vector center, Vector up)
        {
            var z = (eye - center).Normalize();
            if (z.Abs == 0)
            {
                z.Z = 1;
            }

            var x = up.Cross(z).Normalize();
            if (x.Abs == 0)
            {
                z.X += 0.0001;
                x = up.Cross(z).Normalize();
            }

            var y = z.Cross(x).Normalize();

            var m = new Matrix4();

            m.n11 = x.X; m.n12 = y.X; m.n13 = z.X;
            m.n21 = x.Y; m.n22 = y.Y; m.n23 = z.Y;
            m.n31 = x.Z; m.n32 = y.Z; m.n33 = z.Z;

            return m;
        }

        public static Matrix4 Multiply(Matrix4 a, Matrix4 b)
        {
            var m = new Matrix4();

            double a11 = a.n11, a12 = a.n12, a13 = a.n13, a14 = a.n14,
            a21 = a.n21, a22 = a.n22, a23 = a.n23, a24 = a.n24,
            a31 = a.n31, a32 = a.n32, a33 = a.n33, a34 = a.n34,
            a41 = a.n41, a42 = a.n42, a43 = a.n43, a44 = a.n44,

            b11 = b.n11, b12 = b.n12, b13 = b.n13, b14 = b.n14,
            b21 = b.n21, b22 = b.n22, b23 = b.n23, b24 = b.n24,
            b31 = b.n31, b32 = b.n32, b33 = b.n33, b34 = b.n34,
            b41 = b.n41, b42 = b.n42, b43 = b.n43, b44 = b.n44;

            m.n11 = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41;
            m.n12 = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42;
            m.n13 = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43;
            m.n14 = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44;

            m.n21 = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41;
            m.n22 = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42;
            m.n23 = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43;
            m.n24 = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44;

            m.n31 = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41;
            m.n32 = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42;
            m.n33 = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43;
            m.n34 = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44;

            m.n41 = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41;
            m.n42 = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42;
            m.n43 = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43;
            m.n44 = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44;

            return m;
        }

        public Matrix4 Multiply(Matrix4 m)
        {
            return Multiply(this, m);
        }

        public Matrix4 MultiplyScalar(double s)
        {
            var m = new Matrix4();

            m.n11 = this.n11 * s; m.n12 = this.n12 * s; m.n13 = this.n13 * s; m.n14 = this.n14 * s;
            m.n21 = this.n21 * s; m.n22 = this.n22 * s; m.n23 = this.n23 * s; m.n24 = this.n24 * s;
            m.n31 = this.n31 * s; m.n32 = this.n32 * s; m.n33 = this.n33 * s; m.n34 = this.n34 * s;
            m.n41 = this.n41 * s; m.n42 = this.n42 * s; m.n43 = this.n43 * s; m.n44 = this.n44 * s;

            return m;
        }

        public Vector MultiplyVector3(Vector v)
        {
            double vx = v.X, vy = v.Y, vz = v.Z,
            d = 1 / (this.n41 * vx + this.n42 * vy + this.n43 * vz + this.n44);

            return new Vector
            {
                X = (this.n11 * vx + this.n12 * vy + this.n13 * vz + this.n14) * d,
                Y = (this.n21 * vx + this.n22 * vy + this.n23 * vz + this.n24) * d,
                Z = (this.n31 * vx + this.n32 * vy + this.n33 * vz + this.n34) * d
            };
        }

        public Vector4 MultiplyVector4(Vector4 v)
        {
            double vx = v.X, vy = v.Y, vz = v.Z, vw = v.W;

            return new Vector4
            {
                X = this.n11 * vx + this.n12 * vy + this.n13 * vz + this.n14 * vw,
                Y = this.n21 * vx + this.n22 * vy + this.n23 * vz + this.n24 * vw,
                Z = this.n31 * vx + this.n32 * vy + this.n33 * vz + this.n34 * vw,
                W = this.n41 * vx + this.n42 * vy + this.n43 * vz + this.n44 * vw,
            };
        }

        public double Determinant()
        {
            //TODO: make this more efficient
            //( based on http://www.euclideanspace.com/maths/algebra/matrix/functions/inverse/fourD/index.htm )

            return (
                n14 * n23 * n32 * n41 -
                n13 * n24 * n32 * n41 -
                n14 * n22 * n33 * n41 +
                n12 * n24 * n33 * n41 +

                n13 * n22 * n34 * n41 -
                n12 * n23 * n34 * n41 -
                n14 * n23 * n31 * n42 +
                n13 * n24 * n31 * n42 +

                n14 * n21 * n33 * n42 -
                n11 * n24 * n33 * n42 -
                n13 * n21 * n34 * n42 +
                n11 * n23 * n34 * n42 +

                n14 * n22 * n31 * n43 -
                n12 * n24 * n31 * n43 -
                n14 * n21 * n32 * n43 +
                n11 * n24 * n32 * n43 +

                n12 * n21 * n34 * n43 -
                n11 * n22 * n34 * n43 -
                n13 * n22 * n31 * n44 +
                n12 * n23 * n31 * n44 +

                n13 * n21 * n32 * n44 -
                n11 * n23 * n32 * n44 -
                n12 * n21 * n33 * n44 +
                n11 * n22 * n33 * n44
            );
        }

        public Matrix4 Transpose()
        {
            return new Matrix4(
                n11, n21, n31, n41,
                n12, n22, n32, n42,
                n13, n23, n33, n43,
                n14, n24, n34, n44
            );
        }

        public static Matrix4 Translation(double x, double y, double z)
        {
            return new Matrix4(
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1
            );
        }

        public static Matrix4 Scale(double x, double y, double z)
        {
            return new Matrix4(
                x, 0, 0, 0,
                0, y, 0, 0,
                0, 0, z, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4 RotationX(double theta)
        {
            double c = Math.Cos(theta), s = Math.Sin(theta);

            return new Matrix4(
                1, 0, 0, 0,
                0, c, -s, 0,
                0, s, c, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4 RotationY(double theta)
        {
            double c = Math.Cos(theta), s = Math.Sin(theta);

            return new Matrix4(
                 c, 0, s, 0,
                 0, 1, 0, 0,
                -s, 0, c, 0,
                 0, 0, 0, 1
            );
        }

        public static Matrix4 RotationZ(double theta)
        {
            double c = Math.Cos(theta), s = Math.Sin(theta);

            return new Matrix4(
                c, -s, 0, 0,
                s, c, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4 RotationAxis(Vector axis, double angle)
        {
            // Based on http://www.gamedev.net/reference/articles/article1199.asp

            double c = Math.Cos(angle),
            s = Math.Sin(angle),
            t = 1 - c,
            x = axis.X, y = axis.Y, z = axis.Z,
            tx = t * x, ty = t * y;

            return new Matrix4(
                tx * x + c, tx * y - s * z, tx * z + s * y, 0,
                tx * y + s * z, ty * y + c, ty * z - s * x, 0,
                tx * z - s * y, ty * z + s * x, t * z * z + c, 0,
                0, 0, 0, 1
            );
        }

        public Matrix4 Inverse()
        {
            // based on http://www.euclideanspace.com/maths/algebra/matrix/functions/inverse/fourD/index.htm

            var m = new Matrix4();

            m.n11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
            m.n12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
            m.n13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
            m.n14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;
            m.n21 = n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44;
            m.n22 = n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44;
            m.n23 = n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44;
            m.n24 = n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34;
            m.n31 = n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44;
            m.n32 = n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44;
            m.n33 = n13 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44;
            m.n34 = n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34;
            m.n41 = n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43;
            m.n42 = n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43;
            m.n43 = n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43;
            m.n44 = n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33;
            m = m.MultiplyScalar(1 / m.Determinant());

            return m;
        }

        public Matrix4 ExtractRotation()
        {
            var m = new Matrix4();

            var scaleX = 1 / new Vector(n11, n21, n31).Abs;
            var scaleY = 1 / new Vector(n12, n22, n32).Abs;
            var scaleZ = 1 / new Vector(n13, n23, n33).Abs;

            m.n11 = n11 * scaleX;
            m.n21 = n21 * scaleX;
            m.n31 = n31 * scaleX;

            m.n12 = n12 * scaleY;
            m.n22 = n22 * scaleY;
            m.n32 = n32 * scaleY;

            m.n13 = n13 * scaleZ;
            m.n23 = n23 * scaleZ;
            m.n33 = n33 * scaleZ;

            m.n44 = 1;

            return m;
        }

        public static Matrix4 Frustum(double left, double right, double top, double bottom, double near, double far)
        {
            double x, y, a, b, c, d;

            var m = new Matrix4();

            x = 2 * near / (right - left);
            y = 2 * near / (top - bottom);

            a = (right + left) / (right - left);
            b = (top + bottom) / (top - bottom);
            c = -(far + near) / (far - near);
            d = -2 * far * near / (far - near);

            m.n11 = x; m.n12 = 0; m.n13 = a; m.n14 = 0;
            m.n21 = 0; m.n22 = y; m.n23 = b; m.n24 = 0;
            m.n31 = 0; m.n32 = 0; m.n33 = c; m.n34 = d;
            m.n41 = 0; m.n42 = 0; m.n43 = -1; m.n44 = 0;

            return m;
        }

        public static Matrix4 Perspective(double fov, double aspect, double near, double far)
        {
            double ymax, ymin, xmin, xmax;

            ymax = near * Math.Tan(fov * Math.PI / 360);
            ymin = -ymax;
            xmin = ymin * aspect;
            xmax = ymax * aspect;

            return Frustum(xmin, xmax, ymin, ymax, near, far);
        }

        public static Matrix4 Ortho(double left, double right, double top, double bottom, double near, double far)
        {
            double x, y, z, w, h, p;

            var m = new Matrix4();

            w = right - left;
            h = top - bottom;
            p = far - near;

            x = (right + left) / w;
            y = (top + bottom) / h;
            z = (far + near) / p;

            m.n11 = 2 / w; m.n12 = 0; m.n13 = 0; m.n14 = -x;
            m.n21 = 0; m.n22 = 2 / h; m.n23 = 0; m.n24 = -y;
            m.n31 = 0; m.n32 = 0; m.n33 = -2 / p; m.n34 = -z;
            m.n41 = 0; m.n42 = 0; m.n43 = 0; m.n44 = 1;

            return m;
        }
    }

    public static class VectorMatrix4Extensions
    {
        public static Vector Transform(this Vector pt, Matrix4 m)
        {
            return m.MultiplyVector3(pt);
        }
    }

    /// <summary>
    /// Point or vector of 4d.
    /// </summary>
    public struct Vector4
    {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public Vector4(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public double[] Array()
        {
            return new[] { X, Y, Z, W };
        }

        public double Get(int dimension)
        {
            return this.Array()[dimension];
        }

        public bool Equals(Vector4 v)
        {
            return this.X.FloatEquals(v.X) && this.Y.FloatEquals(v.Y) && this.Z.FloatEquals(v.Z) && this.W.FloatEquals(v.W);
        }

        public Vector4 Copy()
        {
            // NOTE: structs do not actually need copy. This is conceptual.
            return this;
        }

        public double Abs
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
            }
        }

        public Vector4 Unit
        {
            get
            {
                return new Vector4(X / Abs, Y / Abs, Z / Abs, W / Abs);
            }
        }

        public double Mag()
        {
            return this.Abs;
        }

        public double MagSq()
        {
            return X * X + Y * Y + Z * Z + W * W;
        }

        public Vector4 Add(Vector4 v)
        {
            return this + v;
        }

        public Vector4 Sub(Vector4 v)
        {
            return this - v;
        }

        public Vector4 Mult(double n)
        {
            return n * this;
        }

        public Vector4 Div(double n)
        {
            return this.Mult(1 / n);
        }

        public double Dist(Vector4 v)
        {
            return this.Sub(v).Mag();
        }

        public static Vector4 operator +(Vector4 v1, Vector4 v2)
        {
            var x = v1.X + v2.X;
            var y = v1.Y + v2.Y;
            var z = v1.Z + v2.Z;
            var w = v1.W + v2.W;
            return new Vector4(x, y, z, w);
        }

        public static Vector4 operator -(Vector4 v1, Vector4 v2)
        {
            var x = v1.X - v2.X;
            var y = v1.Y - v2.Y;
            var z = v1.Z - v2.Z;
            var w = v1.W - v2.W;
            return new Vector4(x, y, z, w);
        }

        public static Vector4 operator *(double lambda, Vector4 v)
        {
            return new Vector4(lambda * v.X, lambda * v.Y, lambda * v.Z, lambda * v.W);
        }

        public double Dot(Vector4 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z + W * v.W;
        }

        public Vector4 Cross(Vector4 v)
        {
            throw new NotImplementedException();
        }

        public Vector4 Normalize()
        {
            return this.Unit;
        }

        public Vector4 Limit(double max)
        {
            return this.Mag() > max ? this.SetMag(max) : this.Copy();
        }

        public Vector4 SetMag(double len)
        {
            return this.Normalize().Mult(len);
        }

        public Vector4 Lerp(Vector4 v, double amt)
        {
            return this.Add(v.Sub(this).Mult(amt));
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", X.ToDecimalString(), Y.ToDecimalString(), Z.ToDecimalString(), W.ToDecimalString());
        }

        public static Vector4 Parse(string str)
        {
            var ns = str.Split(',').Select(s => s.TryParseToDouble()).ToArray();
            return new Vector4(ns[0], ns[1], ns[2], ns[3]);
        }

        public static Vector4 Zero
        {
            get
            {
                return new Vector4(0, 0, 0, 0);
            }
        }

        public static Vector4 XAxis
        {
            get
            {
                return new Vector4(1, 0, 0, 0);
            }
        }

        public static Vector4 YAxis
        {
            get
            {
                return new Vector4(0, 1, 0, 0);
            }
        }

        public static Vector4 ZAxis
        {
            get
            {
                return new Vector4(0, 0, 1, 0);
            }
        }

        public static Vector4 WAxis
        {
            get
            {
                return new Vector4(0, 0, 0, 1);
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(Vector4) && this.Equals((Vector4)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !(left == right);
        }
    }
}
