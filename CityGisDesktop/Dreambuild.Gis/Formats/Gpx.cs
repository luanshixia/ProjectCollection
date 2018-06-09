using Dreambuild.Extensions;
using Dreambuild.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Dreambuild.Gis.Formats
{
    public class Gpx
    {
        public static XNamespace ns = XNamespace.Get("http://www.topografix.com/GPX/1/1");
        public List<GpxTrack> Tracks { get; set; }

        public static Gpx Load(string fileName)
        {
            return new Gpx
            {
                Tracks = XDocument
                    .Load(fileName).Root.Elements(ns + "trk")
                    .Select(x => GpxTrack.FromXElement(x))
                    .ToList()
            };
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }

        public Map ToMap()
        {
            var map = new Map();
            map.Layers.Add(new VectorLayer("Node", VectorLayer.GEOTYPE_POINT));
            map.Layers.Add(new VectorLayer("Link", VectorLayer.GEOTYPE_LINEAR));

            this.Tracks
                .SelectMany(t => t.Segs.SelectMany(s => s.Points))
                .ForEach(p => map.AddFeature("Node", p.ToFeature()));

            this.Tracks
                .SelectMany(t => t.Segs.SelectMany(s => s.ToFeatures()))
                .ForEach(f => map.AddFeature("Link", f));

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
            return new GpxTrack
            {
                Name = xe.Element(Gpx.ns + "name").Value,
                Description = xe.Element(Gpx.ns + "desc").Value,
                Segs = xe.Elements(Gpx.ns + "trkseg").Select(x => GpxTrackSeg.FromXElement(x)).ToList()
            };
        }
    }

    public class GpxTrackSeg
    {
        public List<GpxTrackPoint> Points { get; set; }

        public static GpxTrackSeg FromXElement(XElement xe)
        {
            return new GpxTrackSeg
            {
                Points = xe
                    .Elements(Gpx.ns + "trkpt")
                    .Select(x => GpxTrackPoint.FromXElement(x))
                    .ToList()
            };
        }

        public List<IFeature> ToFeatures()
        {
            return this.Points
                .PairwiseSelect((pt1, pt2) => new Feature
                {
                    GeoData = new List<Vector> { pt1.GetPosition(), pt2.GetPosition() },
                    ["Velocity"] = (pt2.GetPosition().Dist(pt1.GetPosition()) / (pt2.Time - pt1.Time).TotalSeconds).ToString()
                } as IFeature)
                .ToList();
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
            return new GpxTrackPoint
            {
                Latitude = xe.AttValue("lat").TryParseToDouble(),
                Longitude = xe.AttValue("lon").TryParseToDouble(),
                Elevation = xe.Element(Gpx.ns + "ele").Value.TryParseToDouble(),
                Time = DateTime.Parse(xe.Element(Gpx.ns + "time").Value) //.Replace("T", " ").Replace("Z", ""));
            };
        }

        public Vector GetPosition()
        {
            var mag = 1000000;
            return new Vector(Longitude * mag, Latitude * mag);
        }

        public IFeature ToFeature()
        {
            return new Feature
            {
                GeoData = new List<Vector> { GetPosition() },
                ["Elevation"] = Elevation.ToString(),
                ["Time"] = Time.ToString()
            };
        }
    }
}
