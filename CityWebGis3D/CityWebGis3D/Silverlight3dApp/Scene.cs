using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TongJi.Geometry3D;
using System.Xml.Linq;
using System.Windows;

namespace Silverlight3dApp
{
    public class Scene : IDisposable
    {
        #region Fields

        public readonly DrawingSurface _drawingSurface;
        readonly ContentManager contentManager;
        //readonly MeshGeometry mg;

        float aspectRatio;

        #endregion

        #region Properties

        public ContentManager ContentManager
        {
            get
            {
                return contentManager;
            }
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return GraphicsDeviceManager.Current.GraphicsDevice;
            }
        }

        public Camera Camera { get; private set; }
        public Vector3 LightPosition { get; private set; }
        public List<MeshGeometry> MeshGeometries { get; private set; }

        public static Scene Current { get; private set; }

        #endregion

        #region Creation

        public Scene(DrawingSurface drawingSurface)
        {
            _drawingSurface = drawingSurface;

            // Register for size changed to update the aspect ratio
            _drawingSurface.SizeChanged += _drawingSurface_SizeChanged;

            // Get a content manager to access content pipeline
            contentManager = new ContentManager(null)
            {
                RootDirectory = "Content"
            };

            // Initializing variables
            LightPosition = new Vector3(1, 1, -1);
            BuildScene03();
            //mg = new MeshGeometry(this, TongJi.Geometry3D.SurfaceExamples.ExtrudeWithCaps());

            //Camera = new FreeCamera(new Vector3(0, 0, 1.5f * maxHeight), 1, 1, GraphicsDevice);
            //Camera = new FreeFlyCamera(new Vector3(0, 0, 0), Vector3.UnitX + Vector3.UnitY, Vector3.UnitZ, GraphicsDevice);
            //Camera = new OneAxisLevelFlyCamera(new Vector3(0, 0, 0), Vector3.UnitX + Vector3.UnitY, GraphicsDevice);
            //Camera = new TwoAxesLevelFlyCamera(new Vector3(0, 0, 0), Vector3.UnitX + Vector3.UnitY, GraphicsDevice);
            //Camera = new LevelCamera(new Vector3(0, 0, 1.5f * maxHeight), new Vector3(num * 1.5f * size / 2, num * 1.5f * size / 2, 0), 1, 1000, GraphicsDevice);
            //Camera = new FixAxisCamera(new Vector3(0, 0, 1.5f * maxHeight), new Vector3(num * 1.5f * size / 2, num * 1.5f * size / 2, maxHeight), new Vector3(num * 1.5f * size / 2, num * 1.5f * size / 2, 0), 0, 1, 1000, GraphicsDevice);
            //Camera = new ArcBallCamera(new Vector3(0, 0, 1.5f * maxHeight), new Vector3(num * 1.5f * size / 2, num * 1.5f * size / 2, 0), -MathHelper.Pi * 100, MathHelper.Pi * 100, 1, 1000, GraphicsDevice);
            //Camera = new ArcBallCamera1(Vector3.Zero, 0, 0, -MathHelper.Pi * 100, MathHelper.Pi * 100, 5, 1, 10, GraphicsDevice);
            Current = this;
        }

        private void BuildScene(List<Mesh> meshes)
        {
            MeshGeometries = new List<MeshGeometry>();
            meshes.Where(m => m.Vertices.Count > 0).Select(m => new MeshGeometry(this, m)).ForEach(m => MeshGeometries.Add(m));
        }

        private void BuildScene01()
        {
            MeshGeometries = new List<MeshGeometry>();
            Random rand = new Random();
            int size = 10;
            int num = 10;
            int maxHeight = 100;
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    var box = MeshHelpers.Box(size, size, rand.Next(maxHeight / 20, maxHeight));
                    var translate = Matrix4.Translation(i * 1.5 * size, j * 1.5 * size, 0);
                    box.Transform(translate);
                    var mg = new MeshGeometry(this, box);
                    MeshGeometries.Add(mg);
                }
            }
        }

        private void BuildScene02()
        {
            var uri = new Uri("ziyang.ciml", UriKind.Relative);
            var map = new TongJi.Gis.Map(XDocument.Load(Application.GetResourceStream(uri).Stream).Root);
            var city = new TongJi.City.Analysis3D.CityScene(map);
            BuildScene(city.Meshes);
            var pos = ToVector3(city.Meshes[0].Bounds.min);
            pos.Z = 500;
            Camera = new OneAxisLevelFlyCamera(pos, Vector3.UnitX + Vector3.UnitY, GraphicsDevice);
            //Camera = new ArcBallCamera(ToVector3(city.Meshes[0].Bounds.min), ToVector3(city.Meshes[0].Bounds.min), -MathHelper.Pi * 100, MathHelper.Pi * 100, 1, 1000, GraphicsDevice);
        }

        private void BuildScene03()
        {
            var mesh = SurfaceExamples.NonConvexPlanar();
            var sphere = MeshHelpers.Sphere(1);
            var meshes = new List<Mesh> { mesh, sphere };
            BuildScene(meshes);
            Camera = new ArcBallCamera(new Vector3(20, 20, 20), Vector3.Zero, -MathHelper.Pi * 100, MathHelper.Pi * 100, 1, 1000, GraphicsDevice);
        }

        private static Vector3 ToVector3(TongJi.Geometry.Point3D point)
        {
            return new Vector3((float)point.x, (float)point.y, (float)point.z);
        }

        #endregion

        #region Methods

        void _drawingSurface_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            aspectRatio = (float)(_drawingSurface.ActualWidth / _drawingSurface.ActualHeight);
            Camera.AspectRatio = aspectRatio;
        }

        public void Draw()
        {
            // Clear surface
            GraphicsDeviceManager.Current.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(0.2f, 0.2f, 0.2f, 1.0f), 1.0f, 0);

            Camera.Update();

            // Compute matrices
            Matrix world = Matrix.Identity; //Matrix.CreateRotationX(rotationAngle) * Matrix.CreateRotationY(rotationAngle);
            Matrix view = Camera.View; //Matrix.CreateLookAt(new Vector3(0, 0, -5.0f), Vector3.Zero, Vector3.UnitY);
            Matrix projection = Camera.Projection; //Matrix.CreatePerspectiveFieldOfView(0.85f, aspectRatio, 0.01f, 1000.0f);

            foreach (var mg in MeshGeometries)
            {
                // Update per frame parameter values
                mg.World = world;
                mg.WorldViewProjection = world * view * projection;

                // Drawing the cube
                mg.Draw();
            }
        }

        public void Dispose()
        {
            _drawingSurface.SizeChanged -= _drawingSurface_SizeChanged;
        }


        #endregion
    }
}
