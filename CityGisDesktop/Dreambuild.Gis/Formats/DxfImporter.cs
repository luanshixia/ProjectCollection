using Dreambuild.Extensions;
using Dreambuild.Geometry;
using netDxf;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Gis.Formats
{
    public class DxfImporter : IMapImporter
    {
        private DxfDocument _dxf;
        private Dictionary<IFeature, string> _features;

        public DxfImporter(string fileName)
        {
            _dxf = DxfDocument.Load(fileName);
            _features = new Dictionary<IFeature, string>();
            ReadFeatures();
        }

        public Map GetMap()
        {
            Map map = new Map();
            var layerNames = _features.Values.Distinct().ToList();
            layerNames.ForEach(name =>
            {
                var features = _features.Where(f => f.Value == name).Select(x => x.Key).ToList();
                string geoType = features.GuessGeoType();
                VectorLayer layer = new VectorLayer(name, geoType);
                features.ForEach(f => layer.Features.Add(f));
                map.Layers.Add(layer);
            });
            return map;
        }

        private void ReadFeatures()
        {
            _dxf.Points.ForEach(p =>
            {
                var f = new Feature(new Vector(p.Location.X, p.Location.Y));
                _features.Add(f, p.Layer.Name);
            });
            _dxf.Lines.ForEach(l =>
            {
                var pt1 = new Vector(l.StartPoint.X, l.StartPoint.Y);
                var pt2 = new Vector(l.EndPoint.X, l.EndPoint.Y);
                var f = new Feature(pt1, pt2);
                _features.Add(f, l.Layer.Name);
            });
            _dxf.LwPolylines.ForEach(l =>
            {
                var pts = l.PoligonalVertexes(5, 0.001, 0.001).Select(p => new Vector(p.X, p.Y)).ToArray();
                if (pts.Count() > 0)
                {
                    var f = new Feature(pts);
                    _features.Add(f, l.Layer.Name);
                }
            });
        }
    }
}
