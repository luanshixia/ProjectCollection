using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TongJi.Gis
{
    public class Gpx
    {
        public static XNamespace ns = XNamespace.Get("http://www.topografix.com/GPX/1/1");
        public List<GpxTrack> Tracks { get; set; }

        public static Gpx Load(string fileName)
        {
            Gpx result = new Gpx();
            result.Tracks = XDocument.Load(fileName).Root.Elements(ns + "trk").Select(x => GpxTrack.FromXElement(x)).ToList();
            return result;
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }

        public Map ToMap()
        {
            Map map = new Map();
            map.Layers.Add(new VectorLayer("Node", "1"));
            map.Layers.Add(new VectorLayer("Link", "2"));
            Tracks.SelectMany(t => t.Segs.SelectMany(s => s.Points)).ForEach(p => map.AddFeature("Node", p.ToFeature()));
            Tracks.SelectMany(t => t.Segs.SelectMany(s => s.ToFeatures())).ForEach(f => map.AddFeature("Link", f));
            return map;
        }
    }

    public class GpxTrack
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GpxTrackSeg> Segs { get; set; }

        public static GpxTrack FromXElement(XElement xe)
        {
            GpxTrack track = new GpxTrack();
            track.Name = xe.Element(Gpx.ns + "name").Value;
            track.Description = xe.Element(Gpx.ns + "desc").Value;
            track.Segs = xe.Elements(Gpx.ns + "trkseg").Select(x => GpxTrackSeg.FromXElement(x)).ToList();
            return track;
        }
    }

    public class GpxTrackSeg
    {
        public List<GpxTrackPoint> Points { get; set; }

        public static GpxTrackSeg FromXElement(XElement xe)
        {
            GpxTrackSeg seg = new GpxTrackSeg();
            seg.Points = xe.Elements(Gpx.ns + "trkpt").Select(x => GpxTrackPoint.FromXElement(x)).ToList();
            return seg;
        }

        public List<IFeature> ToFeatures()
        {
            List<IFeature> result = new List<IFeature>();
            for (int i = 0; i < Points.Count - 1; i++)
            {
                var pt1 = Points[i];
                var pt2 = Points[i + 1];
                Feature f = new Feature();
                f.GeoData = new Geometry.Polyline(new List<Geometry.Point2D> { pt1.GetPosition(), pt2.GetPosition() }).ToString();
                f["Velocity"] = (pt2.GetPosition().DistTo(pt1.GetPosition()) / (pt2.Time - pt1.Time).TotalSeconds).ToString();
                result.Add(f);
            }
            return result;
        }
    }

    public class GpxTrackPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public DateTime Time { get; set; }

        public static GpxTrackPoint FromXElement(XElement xe)
        {
            GpxTrackPoint pt = new GpxTrackPoint();
            pt.Latitude = xe.AttValue("lat").TryParseToDouble();
            pt.Longitude = xe.AttValue("lon").TryParseToDouble();
            pt.Elevation = xe.Element(Gpx.ns + "ele").Value.TryParseToDouble();
            pt.Time = DateTime.Parse(xe.Element(Gpx.ns + "time").Value); //.Replace("T", " ").Replace("Z", ""));
            return pt;
        }

        public Geometry.Point2D GetPosition()
        {
            double mag = 1000000;
            return new Geometry.Point2D(Longitude * mag, Latitude * mag);
        }

        public IFeature ToFeature()
        {
            Feature f = new Feature();
            f.GeoData = GetPosition().ToString();
            f["Elevation"] = Elevation.ToString();
            f["Time"] = Time.ToString();
            return f;
        }
    }
}
