using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dreambuild.Gis.Formats
{
    public class MultipleShapefileExporter
    {
        private Map _map;

        public MultipleShapefileExporter(Map map)
        {
            _map = map;
        }

        public void Export(string folder)
        {
            foreach (VectorLayer layer in _map.Layers)
            {
                var exporter = new ShapefileExporter(layer);
                exporter.Export(folder, layer.Name);
            }
        }
    }
}
