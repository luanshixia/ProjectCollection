using System;
using System.Collections.Generic;
using System.Linq;

namespace TongJi.Geometry.Computational
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator -(Vector2 v1)
        {
            return new Vector2(-v1.X, -v1.Y);
        }

        public static Vector2 operator *(float lambda, Vector2 v)
        {
            return new Vector2(lambda * v.X, lambda * v.Y);
        }

        public float Abs
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
        }

        public float Length()
        {
            return Abs;
        }

        public Vector2 Unit
        {
            get
            {
                return new Vector2(X / Abs, Y / Abs);
            }
        }

        public Vector2 Normalize()
        {
            return Unit;
        }

        public float Dot(Vector2 v1)
        {
            return X * v1.X + Y * v1.Y;
        }

        public float Kross(Vector2 v1)
        {
            return X * v1.Y - Y * v1.X;
        }

        public static Vector2 Zero
        {
            get
            {
                return new Vector2(0, 0);
            }
        }

        public static Vector2 UnitX
        {
            get
            {
                return new Vector2(1, 0);
            }
        }

        public static Vector2 UnitY
        {
            get
            {
                return new Vector2(0, 1);
            }
        }

        public float DirAngleZeroTo2Pi()
        {
            double angle = this.ZeroToPiAngleTo(Vector2.UnitX);
            if (this.Y < 0)
            {
                angle = 2 * Math.PI - angle;
            }
            return (float)angle;
        }

        public float ZeroToPiAngleTo(Vector2 v1)
        {
            double cosTheta = this.Dot(v1) / (this.Abs * v1.Abs);
            return (float)Math.Acos(cosTheta);
        }

        public float ZeroTo2PiAngleTo(Vector2 v1)
        {
            double angle0 = this.DirAngleZeroTo2Pi();
            double angle1 = v1.DirAngleZeroTo2Pi();
            double angleDelta = angle1 - angle0;
            if (angleDelta < 0) angleDelta = angleDelta + 2 * Math.PI;
            return (float)angleDelta;
        }

        public float MinusPiToPiAngleTo(Vector2 v1)
        {
            double angle0 = this.DirAngleZeroTo2Pi();
            double angle1 = v1.DirAngleZeroTo2Pi();
            double angleDelta = angle1 - angle0;
            if (angleDelta < -Math.PI) angleDelta = angleDelta + 2 * Math.PI;
            else if (angleDelta > Math.PI) angleDelta = angleDelta - 2 * Math.PI;
            return (float)angleDelta;
        }

        public Vector2 Transform(Matrix3 m)
        {
            return m.MultiplyVector2(this);
        }

        public void TransformSelf(Matrix3 m)
        {
            var u = m.MultiplyVector2(this);
            X = u.X;
            Y = u.Y;
        }

        public override string ToString()
        {
            return string.Format("{0:0.00},{1:0.00}", X, Y);
        }
    }

    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            float a = v1.X + v2.X;
            float b = v1.Y + v2.Y;
            float c = v1.Z + v2.Z;
            return new Vector3(a, b, c);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            float a = v1.X - v2.X;
            float b = v1.Y - v2.Y;
            float c = v1.Z - v2.Z;
            return new Vector3(a, b, c);
        }

        public static Vector3 operator -(Vector3 v1)
        {
            float a = - v1.X;
            float b = - v1.Y;
            float c = - v1.Z;
            return new Vector3(a, b, c);
        }

        public static Vector3 operator *(float lambda, Vector3 v)
        {
            return new Vector3(lambda * v.X, lambda * v.Y, lambda * v.Z);
        }

        public float Abs
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public float Length()
        {
            return Abs;
        }

        public Vector3 Unit
        {
            get
            {
                return new Vector3(X / Abs, Y / Abs, Z / Abs);
            }
        }

        public Vector3 Normalize()
        {
            return Unit;
        }

        public float Dot(Vector3 v1)
        {
            return X * v1.X + Y * v1.Y + Z * v1.Z;
        }

        public Vector3 Cross(Vector3 v1)
        {
            float xx = Y * v1.Z - Z * v1.Y;
            float yy = Z * v1.X - X * v1.Z;
            float zz = X * v1.Y - Y * v1.X;
            return new Vector3(xx, yy, zz);
        }

        public static Vector3 Zero
        {
            get
            {
                return new Vector3(0, 0, 0);
            }
        }

        public static Vector3 UnitX
        {
            get
            {
                return new Vector3(1, 0, 0);
            }
        }

        public static Vector3 UnitY
        {
            get
            {
                return new Vector3(0, 1, 0);
            }
        }

        public static Vector3 UnitZ
        {
            get
            {
                return new Vector3(0, 0, 1);
            }
        }

        public float ZeroToPiAngleTo(Vector3 v1)
        {
            double cosTheta = this.Dot(v1) / (this.Abs * v1.Abs);
            return (float)Math.Acos(cosTheta);
        }

        public Vector3 Transform(Geometry3D.Matrix4 m)
        {
            return m.MultiplyVector3(this);
        }

        public void TransformSelf(Geometry3D.Matrix4 m)
        {
            var u = m.MultiplyVector3(this);
            X = u.X;
            Y = u.Y;
            Z = u.Z;
        }

        public override string ToString()
        {
            return string.Format("{0:0.00},{1:0.00},{2:0.00}", X, Y, Z);
        }
    }

    public class Polyline2
    {
    }

    public class Polyline3
    {
    }

    public class PolygonDecompsition
    {
        public static bool IsDiagonal(Polyline poly, int i0, int i1)
        {
            int n = poly.Count;
            List<Point2D> v = poly.Points;

            int iM = (i0 - 1) % n;
            int iP = (i0 + 1) % n;
            if (!SegmentInCone(v[i0], v[i1], v[iM], v[iP]))
            {
                return false;
            }
            for (int j0 = 0, j1 = n - 1; j0 < n; j1 = j0, j0++)
            {
                if (j0 != i0 && j0 != i1 && j1 != i0 && j1 != i1)
                {
                    if (SegmentsIntersect(v[i0], v[i1], v[j0], v[j1]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static double Kross(Vector2D u, Vector2D v)
        {
            // Kross(u,v)=Cross((u,0),(v,0)).z
            return u.x * v.y - u.y * v.x;
        }

        public static bool SegmentInCone(Point2D v0, Point2D v1, Point2D vm, Point2D vp)
        {
            // assert: vm, v0, vp are not collinear

            Vector2D diff = v1 - v0;
            Vector2D edgeL = vm - v0;
            Vector2D edgeR = vp - v0;

            if (Kross(edgeR, edgeL) > 0) // vertex is convex
            {
                return Kross(diff, edgeR) > 0 && Kross(diff, edgeL) < 0;
            }
            else // vertex is reflex
            {
                return Kross(diff, edgeR) < 0 || Kross(diff, edgeL) > 0;
            }
        }

        public static bool SegmentsIntersect(Point2D u0, Point2D u1, Point2D v0, Point2D v1)
        {
            LineSegIntersect intersect = new LineSegIntersect(new LineSeg(u0, u1), new LineSeg(v0, v1));
            return intersect.Intersect();
        }
    }
}
