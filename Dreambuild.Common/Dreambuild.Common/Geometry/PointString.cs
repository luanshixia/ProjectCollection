using Dreambuild.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Geometry
{
    /// <summary>
    /// Multi points, polyline or polygon.
    /// </summary>
    public class PointString
    {
        public List<Vector> Points;

        public PointString(params Vector[] pts)
        {
            this.Points = pts.ToList();
        }

        public PointString(IEnumerable<Vector> pts)
            : this(pts.ToArray())
        {
        }

        public Vector Get(int i)
        {
            return this.Points[i];
        }

        public Extents GetExtents()
        {
            return Extents.FromPoints(Points);
        }

        public double Length()
        {
            var len = 0d;
            for (int i = 0; i < this.Points.Count - 1; i++)
            {
                len += this.Points[i].Dist(this.Points[i + 1]);
            }
            return len;
        }

        public double Area()
        {
            return Math.Abs(this.AlgebraicArea());
        }

        public double AlgebraicArea()
        {
            var a = 0d;
            for (int i = 0; i < this.Points.Count; i++)
            {
                int j = (i < this.Points.Count - 1) ? (i + 1) : 0;
                a += 0.5 * this.Points[i].Kross(this.Points[j]);
            }
            return a;
        }

        public Vector Average()
        {
            return this.Points.Reduce((x, y) => x.Add(y)).Div(this.Points.Count);
        }

        public Vector Centroid()
        {
            var a = 0d;
            var a1 = 0d;
            var c = Vector.Zero;
            if (this.Points.Count == 1)
            {
                return this.Points[0].Copy();
            }
            for (int i = 0; i < this.Points.Count; i++)
            {
                int j = (i < this.Points.Count - 1) ? (i + 1) : 0;
                a1 = 0.5 * this.Points[i].Kross(this.Points[j]);
                a += a1;
                c = c.Add(this.Points[i].Add(this.Points[j]).Div(3).Mult(a1));
            }
            return c.Div(a);
        }

        public Vector Lerp(double param)
        {
            var i = (int)Math.Floor(param);
            var j = i + 1;
            if (j >= this.Points.Count)
            {
                return this.Points[i].Copy();
            }
            return this.Points[i].Lerp(this.Points[j], param - i);
        }

        public Vector Dir(double param)
        {
            var i = (int)Math.Floor(param);
            var j = i + 1;
            if (j >= this.Points.Count)
            {
                var n = this.Points.Count;
                return this.Points[n - 1].Sub(this.Points[n - 2]).Normalize();
            }
            return this.Points[j].Sub(this.Points[i]).Normalize();
        }

        public Vector GetPointAtDist(double dist)
        {
            return Lerp(dist / Length());
        }

        public bool IsPointIn(Vector p)
        {
            var a = 0d;
            for (int i = 0; i < this.Points.Count; i++)
            {
                int j = (i < this.Points.Count - 1) ? (i + 1) : 0;
                a += this.Points[i].Sub(p).AngleTo(this.Points[j].Sub(p), AngleRange.MinusPiToPi);
            }
            return Math.Abs(Math.Abs(a) - 2 * Math.PI) < 0.1;
        }

        public override string ToString()
        {
            return string.Join("|", this.Points);
        }

        public static PointString Parse(string str)
        {
            return new PointString(str.Split('|').Select(s => Vector.Parse(s)).ToArray());
        }
    }
}
