using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dreambuild.Geometry
{
    /// <summary>
    /// 2-d vector
    /// </summary>
    public struct Vector2D
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
        /// vector from x, y coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// get the point form of the vector
        /// </summary>
        /// <returns></returns>
        public Point2D ToPoint()
        {
            return new Point2D(X, Y);
        }

        /// <summary>
        /// equality operator
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator ==(Vector2D v1, Vector2D v2)
        {
            return (v1 - v2).Abs < Parameters.Epsilon;
        }

        /// <summary>
        /// inequality operator
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator !=(Vector2D v1, Vector2D v2)
        {
            return (v1 - v2).Abs >= Parameters.Epsilon;
        }

        /// <summary>
        /// addition operator
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            double a = v1.X + v2.X;
            double b = v1.Y + v2.Y;
            return new Vector2D(a, b);
        }

        /// <summary>
        /// negative operator
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2D operator -(Vector2D v)
        {
            return new Vector2D(-v.X, -v.Y);
        }

        /// <summary>
        /// subtraction operator
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return v1 + (-v2);
        }

        /// <summary>
        /// scalar multiplication operator
        /// </summary>
        /// <param name="lambda"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2D operator *(double lambda, Vector2D v)
        {
            return new Vector2D(lambda * v.X, lambda * v.Y);
        }

        /// <summary>
        /// get the result of scaling the vector by a factor
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vector2D Scale(double factor)
        {
            return factor * this;
        }

        /// <summary>
        /// get the modulus of the vector
        /// </summary>
        public double Abs
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        /// <summary>
        /// get the unit vector of the vector
        /// </summary>
        public Vector2D Unit
        {
            get
            {
                return new Vector2D(X / Abs, Y / Abs);
            }
        }

        /// <summary>
        /// get the dot product of the vector and another
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public double Dot(Vector2D v1)
        {
            return X * v1.X + Y * v1.Y;
        }

        /// <summary>
        /// get the pseudo (scalar) cross product (aka kross) of the vector and another
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public double Kross(Vector2D v1)
        {
            return X * v1.Y - Y * v1.X;
        }

        /// <summary>
        /// get the direction angle of the vector, ranging 0 ~ 2Pi
        /// </summary>
        /// <returns></returns>
        public double DirAngleZeroTo2Pi()
        {
            double angle = this.ZeroToPiAngleTo(Vector2D.XAxis);
            if (this.Y < 0)
            {
                angle = 2 * Math.PI - angle;
            }
            return angle;
        }

        /// <summary>
        /// get the angle to another vector, ranging 0 ~ Pi
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public double ZeroToPiAngleTo(Vector2D v1)
        {
            double cosTheta = this.Dot(v1) / (this.Abs * v1.Abs);
            return Math.Acos(cosTheta);
        }

        /// <summary>
        /// get the angle to another vector, ranging 0 ~ 2Pi
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public double ZeroTo2PiAngleTo(Vector2D v1)
        {
            double angle0 = this.DirAngleZeroTo2Pi();
            double angle1 = v1.DirAngleZeroTo2Pi();
            double angleDelta = angle1 - angle0;
            if (angleDelta < 0) angleDelta = angleDelta + 2 * Math.PI;
            return angleDelta;
        }

        /// <summary>
        /// get the angle to another angle, ranging -Pi ~ Pi
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public double MinusPiToPiAngleTo(Vector2D v1)
        {
            double angle0 = this.DirAngleZeroTo2Pi();
            double angle1 = v1.DirAngleZeroTo2Pi();
            double angleDelta = angle1 - angle0;
            if (angleDelta < -Math.PI) angleDelta = angleDelta + 2 * Math.PI;
            else if (angleDelta > Math.PI) angleDelta = angleDelta - 2 * Math.PI;
            return angleDelta;
        }

        /// <summary>
        /// vector zero
        /// </summary>
        public static Vector2D Zero
        {
            get
            {
                return new Vector2D(0, 0);
            }
        }

        /// <summary>
        /// vector (1, 0)
        /// </summary>
        public static Vector2D XAxis
        {
            get
            {
                return new Vector2D(1, 0);
            }
        }

        /// <summary>
        /// vector (0, 1)
        /// </summary>
        public static Vector2D YAxis
        {
            get
            {
                return new Vector2D(0, 1);
            }
        }

        /// <summary>
        /// get the string notation of the vector
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", X.ToDecimalString(), Y.ToDecimalString());
        }

        /// <summary>
        /// get the unit vector of the vector
        /// </summary>
        /// <returns></returns>
        public Vector2D Normalize()
        {
            return Unit;
        }

        /// <summary>
        /// get the result of rotating the vector by a angle
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector2D Rotate(double angle)
        {
            return new Vector2D(X * Math.Cos(angle) + Y * Math.Sin(angle), -X * Math.Sin(angle) + Y * Math.Cos(angle));
        }
    }
}
