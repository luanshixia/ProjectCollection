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
        private Dictionary<IFeature, string> _features; // TODO: Bad data structure. Use PairList/DoubleDictionary.

        public DxfImporter(string fileName)
        {
            this._dxf = DxfDocument.Load(fileName);
            this._features = new Dictionary<IFeature, string>();
            this.ReadFeatures();
        }

        public Map GetMap()
        {
            var map = new Map();
            this._features.Values.Distinct().ForEach(name =>
            {
                var features = this._features.Where(f => f.Value == name).Select(x => x.Key).ToList();
                var geoType = features.GuessGeoType();
                var layer = new VectorLayer(name, geoType);
                features.ForEach(f => layer.Features.Add(f));
                map.Layers.Add(layer);
            });
            return map;
        }

        private void ReadFeatures()
        {
            this._dxf.Points.ForEach(p =>
            {
                var f = new Feature(new Vector(p.Location.X, p.Location.Y));
                this._features.Add(f, p.Layer.Name);
            });
            this._dxf.Lines.ForEach(l =>
            {
                var pt1 = new Vector(l.StartPoint.X, l.StartPoint.Y);
                var pt2 = new Vector(l.EndPoint.X, l.EndPoint.Y);
                var f = new Feature(pt1, pt2);
                this._features.Add(f, l.Layer.Name);
            });
            this._dxf.LwPolylines.ForEach(l =>
            {
                var pts = l.PoligonalVertexes(5, 0.001, 0.001).Select(p => new Vector(p.X, p.Y)).ToArray();
                if (pts.Count() > 0)
                {
                    var f = new Feature(pts);
                    this._features.Add(f, l.Layer.Name);
                }
            });
        }
    }
}
