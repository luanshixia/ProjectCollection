using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TongJi.Geometry;
using TongJi.Geometry3D;

namespace Silverlight3dApp
{
    public class MeshGeometry
    {
        #region Fields

        readonly Scene scene;
        readonly GraphicsDevice graphicsDevice;
        readonly VertexBuffer vertexBuffer;
        readonly IndexBuffer indexBuffer;
        readonly SilverlightEffect mySilverlightEffect;
        readonly SilverlightEffectParameter worldViewProjectionParameter;
        readonly SilverlightEffectParameter worldParameter;
        readonly SilverlightEffectParameter lightPositionParameter;

        #endregion

        #region Properties

        public int VerticesCount { get; private set; }
        public int FaceCount { get; private set; }

        public Matrix World
        {
            set
            {
                worldParameter.SetValue(value);
            }
        }

        public Matrix WorldViewProjection
        {
            set
            {
                worldViewProjectionParameter.SetValue(value);
            }
        }

        public Vector3 LightPosition
        {
            set
            {
                lightPositionParameter.SetValue(value);
            }
        }

        #endregion

        #region Creation

        public MeshGeometry(Scene scene, Mesh mesh)
        {
            this.scene = scene;
            this.graphicsDevice = scene.GraphicsDevice;
            this.mySilverlightEffect = scene.ContentManager.Load<SilverlightEffect>("CustomEffect");

            // Cache effect parameters
            worldViewProjectionParameter = mySilverlightEffect.Parameters["WorldViewProjection"];
            worldParameter = mySilverlightEffect.Parameters["World"];
            lightPositionParameter = mySilverlightEffect.Parameters["LightPosition"];

            // Init static parameters
            this.LightPosition = scene.LightPosition;

            // Temporary lists
            List<VertexPositionColorNormal> vertices = new List<VertexPositionColorNormal>();
            List<ushort> indices = new List<ushort>();

            Vector4 color = new Vector4(1, 1, 0.8f, 1);
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                var v = mesh.Vertices[i];
                var n = mesh.Normals[i];
                vertices.Add(new VertexPositionColorNormal(new Vector3((float)v.x, (float)v.y, (float)v.z), new Vector3((float)n.x, (float)n.y, (float)n.z), color));
            }
            foreach (var face in mesh.Triangles)
            {
                indices.Add((ushort)face.V0);
                indices.Add((ushort)face.V1);
                indices.Add((ushort)face.V2);
            }

            // Create a vertex buffer, and copy our vertex data into it.
            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColorNormal.VertexDeclaration, vertices.Count, BufferUsage.None);
            vertexBuffer.SetData(0, vertices.ToArray(), 0, vertices.Count, VertexPositionColorNormal.Stride);

            // Create an index buffer, and copy our index data into it.
            indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Count, BufferUsage.None);
            indexBuffer.SetData(0, indices.ToArray(), 0, indices.Count);

            // Statistics
            VerticesCount = vertices.Count;
            FaceCount = indices.Count / 3;
        }

        #endregion

        #region Methods

        public void Draw()
        {
            foreach (var pass in mySilverlightEffect.CurrentTechnique.Passes)
            {
                // Apply pass
                pass.Apply();

                // Set vertex buffer and index buffer
                graphicsDevice.SetVertexBuffer(vertexBuffer);
                graphicsDevice.Indices = indexBuffer;

                // The shaders are already set so we can draw primitives
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesCount, 0, FaceCount);
            }
        }

        #endregion
    }
}
