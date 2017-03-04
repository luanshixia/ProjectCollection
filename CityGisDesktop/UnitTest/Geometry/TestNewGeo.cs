using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Dreambuild.Geometry;

namespace UnitTest.Geometry
{
    [TestFixture]
    public class TestNewGeo
    {
        static Action<bool, string> ok = Assert.True;
        static Action<double, double, string> floatEqual = (a, b, m) => ok(Utils.FloatEquals(a, b), m);

        [Test]
        public void TestVector()
        {
            var v = new Vector(2, 1);
            var e = Vector.XAxis;
            var piOver2 = Math.PI * 0.5;
            var piOver4 = Math.PI * 0.25;

            ok(v.ToString() == "2,1,0", "v is vector (2, 1)");
            ok(v.MagSq() == v.Kross(v.Rotate(piOver2)), "v.magSq() == v.kross(v.rotate(piOver2))");
            ok(v.Mag().ToString("0.###") == "2.236", "v.mag() == 2.236");
            floatEqual(v.Normalize().Mag(), 1, "v.normalize().mag() == 1");
            floatEqual(v.Rotate(-piOver2).Dot(v), 0, "v.rotate(-piOver2).dot(v) == 0");
            ok(v.Cross(v).Mag() == 0, "v.cross(v).mag() == 0");
            ok(Vector.FromAngle(piOver4).Heading() == piOver4, "geo.Vector.fromAngle(piOver4).heading() == piOver4");
            ok(v.SetMag(5).Mag() == 5, "v.setMag(5).mag() == 5");
            floatEqual(v.AngleTo(e), v.Heading(), "v.angleTo(e) == v.heading()");
            ok(v.Lerp(e, 1).Equals(e), "v.lerp(e, 1).equals(e)");
        }

        [Test]
        public void TestExtents()
        {
            var e = Extents.Empty;
            ok(e.IsEmpty(), "e.isEmpty()");

            e = e.AddPoint(Vector.Zero);
            ok(!e.IsEmpty(), "!e.isEmpty()");
            ok(e.Area() == 0, "e.area() == 0");

            e = e.AddPoint(new Vector(1, 1));
            ok(e.Area() == 1, "e.area() == 1");
            ok(e.Center().Equals(new Vector(0.5, 0.5)), "e.center().equals(new geo.Vector(0.5, 0.5))");

            e = e.Extend(2);
            ok(e.Area() == 4, "e.area() == 4");
        }

        [Test]
        public void TestPointString()
        {
            var poly = new PointString(new[] { Vector.Zero, new Vector(1, 0), new Vector(1, 1) });

            ok(poly.GetExtents().Equals(Extents.Create(0, 1, 0, 1)), "poly.extents().equals(geo.Extents.create(0, 1, 0, 1))");
            ok(poly.Length() == 2, "poly.length() == 2");
            ok(poly.Area() == 0.5, "poly.area() == 0.5");
            ok(poly.Average().Equals(new Vector(2d / 3, 1d / 3)), "poly.average().equals(new geo.Vector(2 / 3, 1 / 3))");
            ok(poly.Centroid().Equals(poly.Average()), "poly.centroid().equals(poly.average())");
            ok(poly.Lerp(1.5).Equals(new Vector(1, 0.5)), "poly.lerp(1.5).equals(new geo.Vector(1, 0.5))");
            ok(poly.IsPointIn(poly.Centroid()), "poly.isPointIn(poly.centroid())");
            ok(!poly.IsPointIn(Vector.YAxis), "!poly.isPointIn(geo.Vector.yAxis())");
        }
    }
}
