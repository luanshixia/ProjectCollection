using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Geometry;

namespace TongJi.Geometry
{
    public class Matrix3
    {
        public double n11;
        public double n12;
        public double n13;
        public double n21;
        public double n22;
        public double n23;
        public double n31;
        public double n32;
        public double n33;

        public Matrix3()
        {
        }

        public Matrix3(double n11, double n12, double n13, double n21, double n22, double n23, double n31, double n32, double n33)
        {
            this.n11 = n11; this.n12 = n12; this.n13 = n13;
            this.n21 = n21; this.n22 = n22; this.n23 = n23;
            this.n31 = n31; this.n32 = n32; this.n33 = n33;
        }

        private static Matrix3 _identity = new Matrix3(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1
        );
        public static Matrix3 Identity { get { return _identity; } }

        public static Matrix3 Multiply(Matrix3 a, Matrix3 b)
        {
            Matrix3 m = new Matrix3();

            double a11 = a.n11, a12 = a.n12, a13 = a.n13,
            a21 = a.n21, a22 = a.n22, a23 = a.n23,
            a31 = a.n31, a32 = a.n32, a33 = a.n33,

            b11 = b.n11, b12 = b.n12, b13 = b.n13,
            b21 = b.n21, b22 = b.n22, b23 = b.n23,
            b31 = b.n31, b32 = b.n32, b33 = b.n33;

            m.n11 = a11 * b11 + a12 * b21 + a13 * b31;
            m.n12 = a11 * b12 + a12 * b22 + a13 * b32;
            m.n13 = a11 * b13 + a12 * b23 + a13 * b33;

            m.n21 = a21 * b11 + a22 * b21 + a23 * b31;
            m.n22 = a21 * b12 + a22 * b22 + a23 * b32;
            m.n23 = a21 * b13 + a22 * b23 + a23 * b33;

            m.n31 = a31 * b11 + a32 * b21 + a33 * b31;
            m.n32 = a31 * b12 + a32 * b22 + a33 * b32;
            m.n33 = a31 * b13 + a32 * b23 + a33 * b33;

            return m;
        }

        public Matrix3 Multiply(Matrix3 m)
        {
            return Multiply(this, m);
        }

        public Matrix3 MultiplyScalar(double s)
        {
            Matrix3 m = new Matrix3();

            m.n11 = this.n11 * s; m.n12 = this.n12 * s; m.n13 = this.n13 * s;
            m.n21 = this.n21 * s; m.n22 = this.n22 * s; m.n23 = this.n23 * s;
            m.n31 = this.n31 * s; m.n32 = this.n32 * s; m.n33 = this.n33 * s;

            return m;
        }

        public Vector2D MultiplyVector2(Vector2D v)
        {
            double vx = v.x, vy = v.y,
            d = 1 / (this.n31 * vx + this.n32 * vy + this.n33);

            Vector2D u = new Vector2D();

            u.x = (this.n11 * vx + this.n12 * vy + this.n13) * d;
            u.y = (this.n21 * vx + this.n22 * vy + this.n23) * d;

            return u;
        }

        public Point2D MultiplyVector2(Point2D v)
        {
            double vx = v.x, vy = v.y,
            d = 1 / (this.n31 * vx + this.n32 * vy + this.n33);

            Point2D u = new Point2D();

            u.x = (this.n11 * vx + this.n12 * vy + this.n13) * d;
            u.y = (this.n21 * vx + this.n22 * vy + this.n23) * d;

            return u;
        }

        public Geometry.Computational.Vector2 MultiplyVector2(Geometry.Computational.Vector2 v)
        {
            double vx = v.X, vy = v.Y,
            d = 1 / (this.n31 * vx + this.n32 * vy + this.n33);

            Geometry.Computational.Vector2 u = new Geometry.Computational.Vector2();

            u.X = (float)((this.n11 * vx + this.n12 * vy + this.n13) * d);
            u.Y = (float)((this.n21 * vx + this.n22 * vy + this.n23) * d);

            return u;
        }

        public double Determinant()
        {
            // TODO: make this more efficient
            // by WANG Yang

            return n11 * (n22 * n33 - n23 * n32) + n12 * (n23 * n31 - n21 * n33) + n13 * (n21 * n32 - n22 * n31);
        }

        public Matrix3 Transpose()
        {
            return new Matrix3(
                n11, n21, n31,
                n12, n22, n32,
                n13, n23, n33
            );
        }

        public static Matrix3 Translation(double x, double y)
        {
            return new Matrix3(
                1, 0, x,
                0, 1, y,
                0, 0, 1
            );
        }

        public static Matrix3 Scale(double sx, double sy, Point2D center)
        {
            // by WANG Yang

            double cx = center.x;
            double cy = center.y;
            return new Matrix3(
                sx, 0, cx - cx * sx,
                0, sy, cy - cy * sy,
                0, 0, 1
            );
        }

        public static Matrix3 RotationZ(double theta, Point2D center)
        {
            // by WANG Yang

            double cx = center.x;
            double cy = center.y;
            double c = Math.Cos(theta), s = Math.Sin(theta);
            return new Matrix3(
                c, -s, cx - c * cx + s * cy,
                s, c, cy - s * cx - c * cy,
                0, 0, 1
            );
        }

        public Matrix3 Inverse()
        {
            // by WANG Yang

            Matrix3 m = new Matrix3();

            m.n11 = n22 * n33 - n23 * n32;
            m.n12 = n23 * n31 - n21 * n33;
            m.n13 = n21 * n32 - n22 * n31;
            m.n21 = n32 * n13 - n33 * n12;
            m.n22 = n33 * n11 - n31 * n13;
            m.n23 = n31 * n12 - n32 * n11;
            m.n31 = n12 * n23 - n13 * n22;
            m.n32 = n13 * n21 - n11 * n23;
            m.n33 = n11 * n22 - n12 * n21;

            m = m.Transpose().MultiplyScalar(1 / m.Determinant());

            return m;
        }

        public Matrix3 ExtractRotation()
        {
            Matrix3 m = new Matrix3();

            var scaleX = 1 / new Vector2D(n11, n21).Abs;
            var scaleY = 1 / new Vector2D(n12, n22).Abs;

            m.n11 = n11 * scaleX;
            m.n21 = n21 * scaleX;

            m.n12 = n12 * scaleY;
            m.n22 = n22 * scaleY;

            m.n33 = 1;

            return m;
        }
    }
}
