using Dreambuild.Geometry;
using Dreambuild.Geometry3D;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Dreambuild.Gis.Helix
{
    public class CityScene
    {
        public HelixViewport3D Viewport { get; private set; }
        public Map Map { get; private set; }
        public Dictionary<string, List<ModelVisual3D>> Models { get; private set; }
        public Dictionary<string, Func<IFeature, Material>> MaterialBuilders { get; private set; }
        public Dictionary<string, Func<IFeature, MeshGeometry3D>> GeometryBuilders { get; private set; }

        public CityScene(HelixViewport3D viewport, Map map)
        {
            Viewport = viewport;
            Map = map;
            Models = new Dictionary<string, List<ModelVisual3D>>();
            MaterialBuilders = new Dictionary<string, Func<IFeature, Material>>();
            GeometryBuilders = new Dictionary<string, Func<IFeature, MeshGeometry3D>>();

            MaterialBuilders["地块"] = f => FillHelper.Simple(Colors.White, 1);
            MaterialBuilders["建筑"] = f => FillHelper.Simple(Colors.White, 0.8);
            MaterialBuilders["道路"] = f => FillHelper.Simple(Colors.Orange, 1);
            GeometryBuilders["地块"] = f => MeshHelper.Polygon(GetPolyline(f));
            GeometryBuilders["建筑"] = f => MeshHelper.Block(GetPolyline(f), f["高度"].TryParseToDouble(10));
            GeometryBuilders["道路"] = f => MeshHelper.Road(GetPolyline(f), f["宽度"].TryParseToDouble(21));
        }

        public void Update()
        {
            Models.SelectMany(x => x.Value).ForEach(x => Viewport.Children.Remove(x));
            Models.Clear();

            foreach (var layer in Map.Layers)
            {
                if (GeometryBuilders.ContainsKey(layer.Name) && MaterialBuilders.ContainsKey(layer.Name))
                {
                    Models[layer.Name] = new List<ModelVisual3D>();
                    foreach (var feature in layer.Features)
                    {
                        var mesh = GeometryBuilders[layer.Name](feature);
                        if (mesh != null)
                        {
                            var material = MaterialBuilders[layer.Name](feature);
                            var model = new GeometryModel3D(mesh, material);
                            model.BackMaterial = material;
                            var visual = new ModelVisual3D { Content = model };
                            Models[layer.Name].Add(visual);
                            Viewport.Children.Add(visual);
                        }
                    }
                }
            }
        }

        private static PointString GetPolyline(IFeature f)
        {
            return new PointString(f.GeoData);
        }
    }

    public static class MeshHelper
    {
        public static MeshGeometry3D ToWpfMesh(this Mesh mesh)
        {
            MeshGeometry3D result = new MeshGeometry3D();
            result.Positions = new Point3DCollection(mesh.Vertices.Cast<Geometry.Vector>().Select(p => new System.Windows.Media.Media3D.Point3D(p.X, p.Y, p.Z)));
            var vertexIndices = mesh.Triangles.Cast<Triangle>().Select(f => new int[] { f.V0, f.V1, f.V2 }).SelectMany(x => x).ToArray();
            result.TriangleIndices = new Int32Collection(vertexIndices);

            if (mesh.Normals != null && mesh.Normals.Count > 0)
            {
                result.Normals = new Vector3DCollection(mesh.Normals.Cast<Geometry.Vector>().Select(n => new System.Windows.Media.Media3D.Vector3D(n.X, n.Y, n.Z)));
            }
            if (mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count > 0)
            {
                result.TextureCoordinates = new PointCollection(mesh.TextureCoordinates.Cast<Geometry.Vector>().Select(t => new Point(t.X, t.Y)));
            }
            return result;
        }

        public static MeshGeometry3D Road(PointString poly, double width)
        {
            var grid = new Geometry.Vector[(poly.Points.Count - 1) * 2, 3];
            for (int i = 0; i < poly.Points.Count - 1; i++)
            {
                var a = poly.Points[i];
                var b = poly.Points[i + 1];
                var dir = b - a;
                var norm = dir.Cross(Geometry.Vector.ZAxis).Normalize();
                int j = 2 * i;
                grid[j, 0] = a.Add(-0.5 * width * norm);
                grid[j, 1] = a;
                grid[j, 2] = a.Add(0.5 * width * norm);
                grid[j + 1, 0] = b.Add(-0.5 * width * norm);
                grid[j + 1, 1] = b;
                grid[j + 1, 2] = b.Add(0.5 * width * norm);
            }
            return Dreambuild.Geometry3D.MeshBuilder.RectGrid(grid).ToMesh().ToWpfMesh();
        }

        public static MeshGeometry3D Block(PointString poly, double height)
        {
            return Dreambuild.Geometry3D.MeshBuilder.ExtrudeWithCaps(poly, height).ToMesh().ToWpfMesh();
        }

        public static MeshGeometry3D Polygon(PointString poly)
        {
            var planar = Dreambuild.Geometry3D.MeshBuilder.Planar(poly);
            return planar == null ? null : planar.ToMesh().ToWpfMesh();
        }
    }

    public static class FillHelper
    {
        public static Material Simple(Color color, double opacity, double specularPower = 85)
        {
            SolidColorBrush brush = new SolidColorBrush(color) { Opacity = opacity };
            DiffuseMaterial material1 = new DiffuseMaterial(brush);
            SpecularMaterial material2 = new SpecularMaterial(brush, specularPower);
            MaterialGroup material = new MaterialGroup();
            material.Children.Add(material1);
            material.Children.Add(material2);
            return material;
        }
    }
}
