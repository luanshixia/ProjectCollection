using Dreambuild.Geometry;
using EGIS.ShapeFileLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dreambuild.Gis.Formats
{
    public class ShapefileImporter
    {
        private ShapeFile _shp;
        private string[] _fields;

        public ShapefileImporter(string path)
        {
            this._shp = new ShapeFile(path);
            this._fields = this._shp.GetAttributeFieldNames();
        }

        public VectorLayer GetVectorLayer()
        {
            var layer = new VectorLayer(Path.GetFileNameWithoutExtension(this._shp.FilePath), this.GetGeoType());
            for (var i = 0; i < this._shp.RecordCount; i++)
            {
                var f = new Feature(ShapefileImporter.GetGeoData(this._shp.GetShapeDataD(i)[0]));
                this.SetProperties(f, this._shp.GetAttributeFieldValues(i));
                layer.Features.Add(f);
            }
            return layer;
        }

        private string GetGeoType()
        {
            switch (this._shp.ShapeType)
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
            for (var i = 0; i < this._fields.Length; i++)
            {
                f[this._fields[i]] = data[i].Trim(); // trim for fixed-width space
            }
        }
    }
}
