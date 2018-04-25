using Dreambuild.Extensions;
using Dreambuild.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dreambuild.Common.Test
{
    public class GeometryTests
    {
        static void floatEqual(double a, double b, string m) => Assert.True(a.FloatEquals(b), m);

        [Fact]
        public void TestVector()
        {
            var v = new Vector(2, 1);
            var e = Vector.XAxis;
            var piOver2 = Math.PI * 0.5;
            var piOver4 = Math.PI * 0.25;

            Assert.True(v.ToString() == "2,1,0", "v is vector (2, 1)");
            Assert.True(v.MagSq() == v.Kross(v.Rotate(piOver2)), "v.magSq() == v.kross(v.rotate(piOver2))");
            Assert.True(v.Mag().ToString("0.###") == "2.236", "v.mag() == 2.236");
            floatEqual(v.Normalize().Mag(), 1, "v.normalize().mag() == 1");
            floatEqual(v.Rotate(-piOver2).Dot(v), 0, "v.rotate(-piOver2).dot(v) == 0");
            Assert.True(v.Cross(v).Mag() == 0, "v.cross(v).mag() == 0");
            Assert.True(Vector.FromAngle(piOver4).Heading() == piOver4, "geo.Vector.fromAngle(piOver4).heading() == piOver4");
            Assert.True(v.SetMag(5).Mag() == 5, "v.setMag(5).mag() == 5");
            floatEqual(v.AngleTo(e), v.Heading(), "v.angleTo(e) == v.heading()");
            Assert.True(v.Lerp(e, 1).Equals(e), "v.lerp(e, 1).equals(e)");
        }

        [Fact]
        public void TestExtents()
        {
            var e = Extents.Empty;
            Assert.True(e.IsEmpty(), "e.isEmpty()");

            e = e.AddPoint(Vector.Zero);
            Assert.True(!e.IsEmpty(), "!e.isEmpty()");
            Assert.True(e.Area() == 0, "e.area() == 0");

            e = e.AddPoint(new Vector(1, 1));
            Assert.True(e.Area() == 1, "e.area() == 1");
            Assert.True(e.Center().Equals(new Vector(0.5, 0.5)), "e.center().equals(new geo.Vector(0.5, 0.5))");

            e = e.Extend(2);
            Assert.True(e.Area() == 4, "e.area() == 4");
        }

        [Fact]
        public void TestPointString()
        {
            var poly = new PointString(new[] { Vector.Zero, new Vector(1, 0), new Vector(1, 1) });

            Assert.True(poly.GetExtents().Equals(Extents.Create(0, 1, 0, 1)), "poly.extents().equals(geo.Extents.create(0, 1, 0, 1))");
            Assert.True(poly.Length() == 2, "poly.length() == 2");
            Assert.True(poly.Area() == 0.5, "poly.area() == 0.5");
            Assert.True(poly.Average().Equals(new Vector(2d / 3, 1d / 3)), "poly.average().equals(new geo.Vector(2 / 3, 1 / 3))");
            Assert.True(poly.Centroid().Equals(poly.Average()), "poly.centroid().equals(poly.average())");
            Assert.True(poly.Lerp(1.5).Equals(new Vector(1, 0.5)), "poly.lerp(1.5).equals(new geo.Vector(1, 0.5))");
            Assert.True(poly.IsPointIn(poly.Centroid()), "poly.isPointIn(poly.centroid())");
            Assert.True(!poly.IsPointIn(Vector.YAxis), "!poly.isPointIn(geo.Vector.yAxis())");
        }
    }
}
