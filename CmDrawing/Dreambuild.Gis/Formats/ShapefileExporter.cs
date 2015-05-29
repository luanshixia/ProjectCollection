using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EGIS.ShapeFileLib;

namespace Dreambuild.Gis.Formats
{
    public class ShapefileExporter
    {
        private VectorLayer _layer;

        public ShapefileExporter(VectorLayer layer)
        {
            _layer = layer;
        }

        public void Export(string folder, string name)
        {
            if (_layer.Features.Count == 0)
            {
                return;
            }
            using (var writer = ShapeFileWriter.CreateWriter(folder, name, GetShapeType(), GetFields()))
            {
                foreach (var feature in _layer.Features)
                {
                    writer.AddRecord(GetPoints(feature), feature.GeoData.Count, GetFields(feature));
                }
            }
        }

        private ShapeType GetShapeType()
        {
            switch (_layer.GeoType)
            {
                case VectorLayer.GEOTYPE_POINT:
                    return ShapeType.Point;
                case VectorLayer.GEOTYPE_LINEAR:
                    return ShapeType.PolyLine;
                case VectorLayer.GEOTYPE_REGION:
                    return ShapeType.Polygon;
                default:
                    return ShapeType.Polygon;
            }
        }

        private DbfFieldDesc[] GetFields()
        {
            var props = _layer.Features[0].Properties.Keys.ToList();
            return props.Select((p, i) => new DbfFieldDesc
            {
                FieldType = DbfFieldType.Character,
                FieldLength = 100,
                FieldName = p,
                RecordOffset = i
            }).ToArray();
        }

        private static PointD[] GetPoints(IFeature f)
        {
            return f.GeoData.Select(p => new PointD(p.X, p.Y)).ToArray();
        }

        private static string[] GetFields(IFeature f)
        {
            return f.Properties.Values.ToArray();
        }
    }
}
