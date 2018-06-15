using Dreambuild.Extensions;
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
            this.Map = map;
            this.Models = new Dictionary<string, List<ModelVisual3D>>();
            this.MaterialBuilders = new Dictionary<string, Func<IFeature, Material>>();
            this.GeometryBuilders = new Dictionary<string, Func<IFeature, MeshGeometry3D>>();

            this.MaterialBuilders["地块"] = f => FillHelper.Simple(Colors.White, 1);
            this.MaterialBuilders["建筑"] = f => FillHelper.Simple(Colors.White, 0.8);
            this.MaterialBuilders["道路"] = f => FillHelper.Simple(Colors.Orange, 1);
            this.GeometryBuilders["地块"] = f => MeshHelper.Polygon(CityScene.GetPolyline(f));
            this.GeometryBuilders["建筑"] = f => MeshHelper.Block(CityScene.GetPolyline(f), f["高度"].TryParseToDouble(10));
            this.GeometryBuilders["道路"] = f => MeshHelper.Road(CityScene.GetPolyline(f), f["宽度"].TryParseToDouble(21));
        }

        public void Update()
        {
            this.Models.SelectMany(x => x.Value).ForEach(x => this.Viewport.Children.Remove(x));
            this.Models.Clear();

            foreach (var layer in Map.Layers)
            {
                if (this.GeometryBuilders.ContainsKey(layer.Name) && this.MaterialBuilders.ContainsKey(layer.Name))
                {
                    this.Models[layer.Name] = new List<ModelVisual3D>();
                    foreach (var feature in layer.Features)
                    {
                        var mesh = this.GeometryBuilders[layer.Name](feature);
                        if (mesh != null)
                        {
                            var material = this.MaterialBuilders[layer.Name](feature);
                            var model = new GeometryModel3D(mesh, material)
                            {
                                BackMaterial = material
                            };
                            var visual = new ModelVisual3D { Content = model };
                            this.Models[layer.Name].Add(visual);
                            this.Viewport.Children.Add(visual);
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
            var result = new MeshGeometry3D
            {
                Positions = new Point3DCollection(mesh.Vertices.Cast<Geometry.Vector>().Select(p => new Point3D(p.X, p.Y, p.Z))),
                TriangleIndices = new Int32Collection(mesh.Triangles.Cast<Triangle>().Select(f => new int[] { f.V0, f.V1, f.V2 }).SelectMany(x => x))
            };

            if (mesh.Normals != null && mesh.Normals.Count > 0)
            {
                result.Normals = new Vector3DCollection(mesh.Normals.Cast<Geometry.Vector>().Select(n => new Vector3D(n.X, n.Y, n.Z)));
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
            for (var i = 0; i < poly.Points.Count - 1; i++)
            {
                var a = poly.Points[i];
                var b = poly.Points[i + 1];
                var dir = b - a;
                var norm = dir.Cross(Geometry.Vector.ZAxis).Normalize();
                var j = 2 * i;
                grid[j, 0] = a.Add(-0.5 * width * norm);
                grid[j, 1] = a;
                grid[j, 2] = a.Add(0.5 * width * norm);
                grid[j + 1, 0] = b.Add(-0.5 * width * norm);
                grid[j + 1, 1] = b;
                grid[j + 1, 2] = b.Add(0.5 * width * norm);
            }
            return Geometry3D.MeshBuilder.RectGrid(grid).ToMesh().ToWpfMesh();
        }

        public static MeshGeometry3D Block(PointString poly, double height)
        {
            return Geometry3D.MeshBuilder.ExtrudeWithCaps(poly, height).ToMesh().ToWpfMesh();
        }

        public static MeshGeometry3D Polygon(PointString poly)
        {
            var planar = Geometry3D.MeshBuilder.Planar(poly);
            return planar?.ToMesh().ToWpfMesh();
        }
    }

    public static class FillHelper
    {
        public static Material Simple(Color color, double opacity, double specularPower = 85)
        {
            var brush = new SolidColorBrush(color) { Opacity = opacity };
            var material1 = new DiffuseMaterial(brush);
            var material2 = new SpecularMaterial(brush, specularPower);
            var material = new MaterialGroup();
            material.Children.Add(material1);
            material.Children.Add(material2);
            return material;
        }
    }
}
