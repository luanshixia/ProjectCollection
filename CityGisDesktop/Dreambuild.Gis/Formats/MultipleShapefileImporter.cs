
namespace Dreambuild.Gis.Formats
{
    public class MultipleShapefileImporter : IMapImporter
    {
        private string _folder;

        public MultipleShapefileImporter(string folder)
        {
            _folder = folder;
        }

        public Map GetMap()
        {
            var map = new Map();
            var files = System.IO.Directory.GetFiles(_folder, "*.shp", System.IO.SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                ShapefileImporter importer = new ShapefileImporter(file);
                map.Layers.Add(importer.GetVectorLayer());
            }
            return map;
        }
    }
}
