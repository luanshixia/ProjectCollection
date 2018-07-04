using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Geometry
{
    /// <summary>
    /// Bounding box of 2d or 3d.
    /// </summary>
    public class Extents
    {
        public Vector? Min;
        public Vector? Max;

        public Extents()
            : this(null, null)
        {
        }

        public Extents(Vector min)
            : this(min, min)
        {
        }

        public Extents(Vector? min, Vector? max)
        {
            this.Min = min;
            this.Max = max;
        }

        public Extents(IEnumerable<Vector> points)
        {
            this.Min = new Vector(points.Min(point => point.X), points.Min(point => point.Y), points.Min(point => point.Z));
            this.Max = new Vector(points.Max(point => point.X), points.Max(point => point.Y), points.Max(point => point.Z));
        }

        public Extents Copy()
        {
            return new Extents(this.Min, this.Max);
        }

        public bool Equals(Extents e)
        {
            if (this.IsEmpty() || e.IsEmpty())
            {
                return this.IsEmpty() && e.IsEmpty();
            }
            return this.Min.Value.Equals(e.Min.Value) && this.Max.Value.Equals(e.Max.Value);
        }

        public Extents Add(Extents e)
        {
            if (this.IsEmpty())
            {
                return e.Copy();
            }
            else if (e.IsEmpty())
            {
                return this.Copy();
            }
            return Extents.Create(
                Math.Min(this.Min.Value.X, e.Min.Value.X), Math.Max(this.Max.Value.X, e.Max.Value.X),
                Math.Min(this.Min.Value.Y, e.Min.Value.Y), Math.Max(this.Max.Value.Y, e.Max.Value.Y),
                Math.Min(this.Min.Value.Z, e.Min.Value.Z), Math.Max(this.Max.Value.Z, e.Max.Value.Z));
        }

        public Extents AddPoint(Vector p)
        {
            if (this.IsEmpty())
            {
                return new Extents(p, p);
            }
            return Extents.Create(
                Math.Min(this.Min.Value.X, p.X), Math.Max(this.Max.Value.X, p.X),
                Math.Min(this.Min.Value.Y, p.Y), Math.Max(this.Max.Value.Y, p.Y),
                Math.Min(this.Min.Value.Z, p.Z), Math.Max(this.Max.Value.Z, p.Z));
        }

        public Extents Extend(double factor)
        {
            var center = this.Center();
            return new Extents(center.Add(this.Min.Value.Sub(center).Mult(factor)), center.Add(this.Max.Value.Sub(center).Mult(factor)));
        }

        public Extents Offset(double amount)
        {
            return new Extents(this.Min.Value.Add(new Vector(-amount, -amount)), this.Max.Value.Add(new Vector(amount, amount)));
        }

        public double Range(int dimension)
        {
            return this.Max.Value.Sub(this.Min.Value).Get(dimension);
        }

        public double Area()
        {
            return this.Range(0) * this.Range(1);
        }

        public double Volume()
        {
            return this.Range(0) * this.Range(1) * this.Range(2);
        }

        public Vector Center()
        {
            return this.Min.Value.Add(this.Max.Value).Mult(0.5);
        }

        public bool IsEmpty()
        {
            return !this.Min.HasValue;
        }

        public bool IsPointIn(Vector p)
        {
            return p.X >= this.Min.Value.X && p.X <= this.Max.Value.X
                && p.Y >= this.Min.Value.Y && p.Y <= this.Max.Value.Y
                && p.Z >= this.Min.Value.Z && p.Z <= this.Max.Value.Z;
        }

        public bool IsExtentsIn(Extents e)
        {
            return e.IsIn(this);
        }

        public bool IsIn(Extents e)
        {
            return this.Min.Value.X >= e.Min.Value.X && this.Max.Value.X <= e.Max.Value.X
                && this.Min.Value.Y >= e.Min.Value.Y && this.Max.Value.Y <= e.Max.Value.Y
                && this.Min.Value.Z >= e.Min.Value.Z && this.Max.Value.Z <= e.Max.Value.Z;
        }

        public bool IsCross(Extents e)
        {
            var union = this.Add(e);
            return new[] { 0, 1, 2 }.All(i => union.Range(i) <= this.Range(i) + e.Range(i));
        }

        public static Extents Create(double minx, double maxx, double miny, double maxy, double minz = 0, double maxz = 0)
        {
            return new Extents(new Vector(minx, miny, minz), new Vector(maxx, maxy, maxz));
        }

        public static Extents Empty
        {
            get
            {
                return new Extents();
            }
        }

        public static Extents FromPoints(IEnumerable<Vector> pts)
        {
            return Extents.FromPoints(pts.ToArray());
        }

        public static Extents FromPoints(params Vector[] pts)
        {
            var min = new Vector(pts.Min(p => p.X), pts.Min(p => p.Y));
            var max = new Vector(pts.Max(p => p.X), pts.Max(p => p.Y));
            return new Extents(min, max);
        }
    }
}
