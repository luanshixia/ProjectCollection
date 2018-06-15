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
            this.Viewport = viewport;
            this.Map = map;
            this.Blocks = new List<Visual3D>();
            this.Roads = new List<Visual3D>();

            this.Init();
        }

        private void Init()
        {
            var basePaths = this.Map.Layers["地块"].Features;
            foreach (var feature in basePaths)
            {
                var height = 100d; //basePath["f"].TryParseToDouble() * FloorHeight;
                var block = BlockScene.BuildBlock(feature, height);
                this.Blocks.Add(block);
                this.Viewport.Children.Add(block);
            }

            var roadFeatures = this.Map.Layers["道路"].Features;
            foreach (var feature in roadFeatures)
            {
                var width = 30d;
                var road = BlockScene.BuildRoad(feature, width);
                this.Roads.Add(road);
                this.Viewport.Children.Add(road);
            }

            this.Viewport.ZoomExtents();
        }

        private static ModelVisual3D BuildBlock(IFeature feature, double height)
        {
            var curve = new PointString(feature.GeoData);
            var mesh = Geometry3D.MeshBuilder.ExtrudeWithCaps(curve, height).ToMesh().ToWpfMesh();
            var material = BlockScene.GetBlockMaterial(feature);
            var model = new GeometryModel3D(mesh, material)
            {
                BackMaterial = material
            };
            return new ModelVisual3D { Content = model };
        }

        private static ModelVisual3D BuildRoad(IFeature feature, double width)
        {
            var curve = new PointString(feature.GeoData);
            var mesh = MeshHelper.Road(new PointString(feature.GeoData), width);
            var material = BlockScene.GetRoadMaterial(feature);
            var model = new GeometryModel3D(mesh, material)
            {
                BackMaterial = material
            };
            return new ModelVisual3D { Content = model };
        }

        private static Material GetBlockMaterial(IFeature feature)
        {
            var brush = new SolidColorBrush(Colors.White) { Opacity = 0.8 };
            var material1 = new DiffuseMaterial(brush);
            var material2 = new SpecularMaterial(brush, 90);
            var material = new MaterialGroup();
            material.Children.Add(material1);
            material.Children.Add(material2);
            return material;
        }

        private static Material GetRoadMaterial(IFeature feature)
        {
            var brush = new SolidColorBrush(Colors.Orange);
            var material1 = new DiffuseMaterial(brush);
            var material2 = new SpecularMaterial(brush, 90);
            var material = new MaterialGroup();
            material.Children.Add(material1);
            material.Children.Add(material2);
            return material;
        }
    }

    public class MyMeshElement3D : MeshElement3D
    {
        protected override MeshGeometry3D Tessellate()
        {
            return this.Model.Geometry as MeshGeometry3D;
        }
    }
}
