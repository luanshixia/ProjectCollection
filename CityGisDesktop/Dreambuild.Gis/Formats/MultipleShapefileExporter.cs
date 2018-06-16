namespace Dreambuild.Gis.Formats
{
    public class MultipleShapefileExporter
    {
        private Map _map;

        public MultipleShapefileExporter(Map map)
        {
            this._map = map;
        }

        public void Export(string folder)
        {
            foreach (VectorLayer layer in this._map.Layers)
            {
                var exporter = new ShapefileExporter(layer);
                exporter.Export(folder, layer.Name);
            }
        }
    }
}
