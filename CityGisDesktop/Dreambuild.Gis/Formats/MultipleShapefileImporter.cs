namespace Dreambuild.Gis.Formats
{
    using System.IO;

    public class MultipleShapefileImporter : IMapImporter
    {
        private string _folder;

        public MultipleShapefileImporter(string folder)
        {
            this._folder = folder;
        }

        public Map GetMap()
        {
            var map = new Map();
            var files = Directory.GetFiles(this._folder, "*.shp", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var importer = new ShapefileImporter(file);
                map.Layers.Add(importer.GetVectorLayer());
            }
            return map;
        }
    }
}
