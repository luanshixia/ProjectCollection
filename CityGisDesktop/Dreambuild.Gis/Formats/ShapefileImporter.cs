using Dreambuild.Geometry;
using EGIS.ShapeFileLib;
using System.Collections.Generic;
using System.Linq;

namespace Dreambuild.Gis.Formats
{
    public class ShapefileImporter
    {
        private ShapeFile _shp;
        private string[] _fields;

        public ShapefileImporter(string path)
        {
            _shp = new ShapeFile(path);
            _fields = _shp.GetAttributeFieldNames();
        }

        public VectorLayer GetVectorLayer()
        {
            VectorLayer layer = new VectorLayer(System.IO.Path.GetFileNameWithoutExtension(_shp.FilePath), GetGeoType());
            for (int i = 0; i < _shp.RecordCount; i++)
            {
                Feature f = new Feature(GetGeoData(_shp.GetShapeDataD(i)[0]));
                SetProperties(f, _shp.GetAttributeFieldValues(i));
                layer.Features.Add(f);
            }
            return layer;
        }

        private string GetGeoType()
        {
            switch (_shp.ShapeType)
            {
                case ShapeType.Point:
                    return VectorLayer.GEOTYPE_POINT;
                case ShapeType.PolyLine:
                    return VectorLayer.GEOTYPE_LINEAR;
                case ShapeType.Polygon:
                    return VectorLayer.GEOTYPE_REGION;
                default:
                    return VectorLayer.GEOTYPE_REGION;
            }
        }

        private static List<Vector> GetGeoData(PointD[] data)
        {
            return data.Select(d => new Vector(d.X, d.Y)).ToList();
        }

        private void SetProperties(IFeature f, string[] data)
        {
            for (int i = 0; i < _fields.Length; i++)
            {
                f[_fields[i]] = data[i].Trim(); // trim for fixed-width space
            }
        }
    }
}
