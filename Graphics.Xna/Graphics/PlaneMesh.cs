using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Graphics.Xna.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Common.Core.Graphics;
namespace MHGameWork.TheWizards.Graphics
{
    /// <summary>
    /// Represents a box that starts at 0,0,0 and extends in the axis directions with a length
    /// </summary>
    public class PlaneMesh
    {
        private BoundingBox boundingBox;
        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
        }

        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                if (shader != null)
                {
                    shader.DiffuseColor = color.ToVector4();
                }
            }
        }


        private float width;
        private float height;

        public float Width
        {
            get { return width; }
            set
            {
                width = value; UpdateWorldMatrix();
                calculateBoundingBox();
            }
        }

        public float Height
        {
            get { return height; }
            set
            {
                height = value; UpdateWorldMatrix();
                calculateBoundingBox();
            }
        }

        private Matrix worldMatrix;
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                worldMatrix = value; UpdateWorldMatrix();
                calculateBoundingBox();
            }
        }

        private Vector3 lightDirection;
        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = Vector3.Normalize(value); shader.LightDirection = lightDirection; }
        }

        private VertexDeclaration vertexDeclaration;

        private ColladaShader shader;

        public ColladaShader Shader
        {
            get { return shader; }
        }

        private Primitives primitives;


        public PlaneMesh()
        {
            worldMatrix = Matrix.Identity;
            width = 10;
            height = 10;
            color = Color.White;
            lightDirection = Vector3.Normalize(new Vector3(-2, -1, -1));
        }

        public PlaneMesh(int _width, int _height, Color _color)
        {
            worldMatrix = Matrix.Identity;
            width = _width;
            height = _height;
            color = _color;
            lightDirection = Vector3.Normalize(new Vector3(-2, -1, -1));
        }

        public virtual void Initialize(IXNAGame game)
        {

            TangentVertex[] vertices;
            short[] indices;
            CreatePlaneIndicesVertices(out vertices, out indices);

            primitives = new Primitives();

            primitives.VertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertices.Length, BufferUsage.None);
            primitives.VertexBuffer.SetData<TangentVertex>(vertices);

            primitives.IndexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), indices.Length, BufferUsage.None);
            primitives.IndexBuffer.SetData(indices);

            primitives.VertexCount = vertices.Length;
            primitives.PrimitiveCount = indices.Length / 3;
            primitives.VertexStride = TangentVertex.SizeInBytes;

            ReloadShader(game);

            vertexDeclaration = TangentVertexExtensions.CreateVertexDeclaration(game);


        }

        public void ReloadShader(IXNAGame game)
        {
            if (shader != null) shader.Dispose();

            //TODO: better use effectpool here
            shader = new ColladaShader(game, null);
            shader.Technique = ColladaShader.TechniqueType.Colored;
            shader.DiffuseColor = color.ToVector4();

            shader.LightDirection = Vector3.Normalize(lightDirection);
            shader.LightColor = Color.White.ToVector3();
            shader.AmbientColor = (new Color(100, 100, 100, 255)).ToVector4();

            UpdateWorldMatrix();
        }


        private void calculateBoundingBox()
        {
            Vector3 transl = worldMatrix.Translation;
            Vector3 range = new Vector3(width * 0.5f, 0, height * 0.5f);
            BoundingBox bb = new BoundingBox(-range + transl, range + transl);

            boundingBox = bb;
        }

        /// <summary>
        /// Calculates the actual world matrix used in the shader
        /// </summary>
        private Matrix calculateObjectWorldMatrix()
        {
            return Matrix.CreateScale(width, 1, height) * worldMatrix;
        }

        public static void CreatePlaneIndicesVertices(out TangentVertex[] vertices, out short[] indices)
        {
            vertices = new TangentVertex[4];
            vertices[0] = new TangentVertex(new Vector3(-0.5f, 0, -0.5f), new Vector2(0, 0), Vector3.Up, Vector3.Zero);
            vertices[1] = new TangentVertex(new Vector3(0.5f, 0, -0.5f), new Vector2(1, 0), Vector3.Up, Vector3.Zero);
            vertices[2] = new TangentVertex(new Vector3(-0.5f, 0, 0.5f), new Vector2(0, 1), Vector3.Up, Vector3.Zero);
            vertices[3] = new TangentVertex(new Vector3(0.5f, 0, 0.5f), new Vector2(1, 1), Vector3.Up, Vector3.Zero);

            indices = new short[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 3;

        }

        public void Render(IXNAGame game)
        {
            shader.ViewProjection = game.Camera.ViewProjection;

            game.GraphicsDevice.VertexDeclaration = vertexDeclaration;
            shader.RenderPrimitiveSinglePass(primitives, SaveStateMode.None);
        }

        public void Update(IXNAGame game)
        {
        }

        private void UpdateWorldMatrix()
        {
            if (shader == null) return;
            shader.World = calculateObjectWorldMatrix();
        }



    }
}
