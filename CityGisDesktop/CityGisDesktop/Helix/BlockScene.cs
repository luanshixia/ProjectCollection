using Dreambuild.Geometry;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;


namespace Dreambuild.Gis.Helix
{
    public class BlockScene
    {
        public HelixViewport3D Viewport { get; private set; }
        public Map Map { get; set; }
        public List<Visual3D> Blocks { get; private set; }
        public List<Visual3D> Roads { get; private set; }

        public const double FloorHeight = 3.0;

        public BlockScene(HelixViewport3D viewport, Map map)
        {
            Viewport = viewport;
            Map = map;
            Blocks = new List<Visual3D>();
            Roads = new List<Visual3D>();

            Init();
        }

        private void Init()
        {
            var basePaths = Map.Layers["地块"].Features;
            foreach (IFeature feature in basePaths)
            {
                double height = 100; //basePath["f"].TryParseToDouble() * FloorHeight;
                var block = BuildBlock(feature, height);
                Blocks.Add(block);
                Viewport.Children.Add(block);
            }

            var roadFeatures = Map.Layers["道路"].Features;
            foreach (IFeature feature in roadFeatures)
            {
                double width = 30;
                var road = BuildRoad(feature, width);
                Roads.Add(road);
                Viewport.Children.Add(road);
            }

            Viewport.ZoomExtents();
        }

        private static ModelVisual3D BuildBlock(IFeature feature, double height)
        {
            var curve = new PointString(feature.GeoData);
            var mesh = Dreambuild.Geometry3D.MeshBuilder.ExtrudeWithCaps(curve, height).ToMesh().ToWpfMesh();
            var material = GetBlockMaterial(feature);
            var model = new GeometryModel3D(mesh, material);
            model.BackMaterial = material;
            return new ModelVisual3D { Content = model };
        }

        private static ModelVisual3D BuildRoad(IFeature feature, double width)
        {
            var curve = new PointString(feature.GeoData);
            var mesh = MeshHelper.Road(new PointString(feature.GeoData), width);
            var material = GetRoadMaterial(feature);
            var model = new GeometryModel3D(mesh, material);
            model.BackMaterial = material;
            return new ModelVisual3D { Content = model };
        }

        //[Obsolete]
        //private static ModelVisual3D BuildBaseMap(Map map)
        //{
        //    var extents = map.GetExtents();
        //    double width = extents.max.X - extents.min.X;
        //    double height = extents.max.Y - extents.min.Y;
        //    double cx = (extents.min.X + extents.max.X) / 2;
        //    double cy = (extents.min.Y + extents.max.Y) / 2;
        //    var mapControl = new Dreambuild.Gis.Display.MapControl();
        //    mapControl.InitializeMap(map);
        //    VisualBrush brush = new VisualBrush(Dreambuild.Gis.Display.MapControl.Current);
        //    RectangleVisual3D rect = new RectangleVisual3D { Length = width, Width = height, Origin = new Point3D(cx, cy, 0), Fill = brush };
        //    return rect;
        //}

        private static Material GetBlockMaterial(IFeature feature)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.White) { Opacity = 0.8 };
            DiffuseMaterial material1 = new DiffuseMaterial(brush);
            SpecularMaterial material2 = new SpecularMaterial(brush, 90);
            MaterialGroup material = new MaterialGroup();
            material.Children.Add(material1);
            material.Children.Add(material2);
            return material;
        }

        private static Material GetRoadMaterial(IFeature feature)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Orange);
            DiffuseMaterial material1 = new DiffuseMaterial(brush);
            SpecularMaterial material2 = new SpecularMaterial(brush, 90);
            MaterialGroup material = new MaterialGroup();
            material.Children.Add(material1);
            material.Children.Add(material2);
            return material;
        }
    }

    public class MyMeshElement3D : MeshElement3D
    {
        protected override MeshGeometry3D Tessellate()
        {
            return Model.Geometry as MeshGeometry3D;
        }
    }
}
