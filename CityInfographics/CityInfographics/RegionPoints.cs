using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Geometry;
using TongJi.Gis;

namespace CityInfographics
{
    public class RegionPoints
    {
        public Map City { get; private set; }
        public Dictionary<Point2D, IFeature> Points { get; private set; }
        public const double CellSize = 10;

        public Dictionary<string, int> CommercialForms { get; private set; }
        public Dictionary<string, double> BuildingAreas { get; private set; }
        public Dictionary<int, TimeRecordList> DataOfForms { get; private set; }

        private int[] _formCodes = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public RegionPoints(Map city)
        {
            City = city;
            Points = new Dictionary<Point2D, IFeature>();
            CommercialForms = new Dictionary<string, int>();
            BuildingAreas = new Dictionary<string, double>();
            DataOfForms = new Dictionary<int, TimeRecordList>();

            LoadDbs();
            BuildPoints();
        }

        private void LoadDbs()
        {
            var lines = System.IO.File.ReadAllLines("Data\\buildings.csv", Encoding.Default).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Split(',')).ToArray();
            lines.ForEach(x =>
            {
                CommercialForms[x[0]] = x[2].TryParseToInt32();
                BuildingAreas[x[0]] = x[1].TryParseToDouble();
            });
            _formCodes.ForEach(x =>
            {
                DataOfForms[x] = new TimeRecordList(string.Format("Data\\{0}.csv", x));
            });
        }

        private void BuildPoints()
        {
            var extents = City.GetExtents();
            var regions = City.Layers["building"].Features.Select(f => Tuple.Create(f, new Polygon(f.GeoData))).ToList();
            // 高效版
            var xs = extents.min.x.ListTo(extents.max.x, CellSize).ToList();
            var ys = extents.min.y.ListTo(extents.max.y, CellSize).ToList();
            foreach (var region in regions)
            {
                var regionExtents = region.Item2.GetExtent();
                var xs1 = xs.Where(x => x >= regionExtents.min.x && x <= regionExtents.max.x).ToList();
                var ys1 = ys.Where(y => y >= regionExtents.min.y && y <= regionExtents.max.y).ToList();
                var pts = xs1.Cross(ys1).ToList();
                foreach (var pt in pts)
                {
                    Point2D p = new Point2D(pt.Item1, pt.Item2);
                    if (region.Item2.IsPointIn(p))
                    {
                        Points[p] = region.Item1;
                    }
                }
            }
            // 低效版
            //for (double x = extents.min.x; x < extents.max.x; x += CellSize)
            //{
            //    for (double y = extents.min.y; y < extents.max.y; y += CellSize)
            //    {
            //        Point2D p = new Point2D(x, y);
            //        foreach (var region in regions)
            //        {
            //            if (region.Item2.IsPointIn(p))
            //            {
            //                Points[p] = region.Item1;
            //                break;
            //            }
            //        }
            //    }
            //}
        }

        public double GetValue(IFeature f, int m, int d, int h, int i)
        {
            string code = f["c"];
            //code = (code + "#").Trim('0').Trim('#'); // 去掉开头的0
            if (CommercialForms.ContainsKey(code))
            {
                int commercialForm = CommercialForms[code];
                if (DataOfForms.ContainsKey(commercialForm))
                {
                    var records = DataOfForms[commercialForm];
                    return records.GetValue(m, d, h, i);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
