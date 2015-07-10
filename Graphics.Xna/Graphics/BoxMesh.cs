using System;
using System.Collections.Generic;
using System.Text;
using Graphics.Xna.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Common.Core.Graphics;
namespace MHGameWork.TheWizards.Graphics
{
    /// <summary>
    /// Represents a box that starts at 0,0,0 and extends in the axis directions with a length
    /// </summary>
    public class BoxMesh : IXNAObject
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

        private Vector3 dimension;
        public Vector3 Dimensions
        {
            get { return dimension; }
            set
            {
                dimension = value; UpdateWorldMatrix();
                calculateBoundingBox();
            }
        }


        private Vector3 pivotPoint;
        /// <summary>
        /// The relative location of the (0,0,0) in object space, relative to the origin of the box (see class summary)
        /// This is the pivot point, before dimensions are applied
        /// </summary>
        public Vector3 PivotPoint
        {
            get { return pivotPoint; }
            set { pivotPoint = value; UpdateWorldMatrix(); calculateBoundingBox(); }
        }

        private Matrix worldMatrix;
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; UpdateWorldMatrix(); calculateBoundingBox(); }
        }

        private Vector3 lightDirection;
        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = Vector3.Normalize(value); shader.LightDirection = lightDirection; }
        }

        private VertexDeclaration vertexDeclaration;

        private ColladaShader shader;

        private Primitives primitives;


        public BoxMesh()
        {
            worldMatrix = Matrix.Identity;
            pivotPoint = Vector3.Zero;
            Dimensions = Vector3.One;
            color = Color.Green;
            lightDirection = new Vector3(-2, -1, -1);

            calculateBoundingBox();
        }

        private void calculateBoundingBox()
        {
            BoundingBox bb = new BoundingBox(Vector3.Zero, Vector3.One);

            Vector3[] corners = bb.GetCorners();
            Matrix mat = calculateObjectWorldMatrix();
            Vector3.Transform(corners, ref mat, corners);

            boundingBox = BoundingBox.CreateFromPoints(corners);
        }

        public virtual void Initialize(IXNAGame game)
        {

            TangentVertex[] vertices;
            short[] indices;
            CreateUnitBoxVerticesAndIndices(out vertices, out indices);

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

        public static void CreateUnitBoxVerticesAndIndices(out TangentVertex[] vertices, out short[] indices)
        {
            GeometryHelper.CreateUnitBoxVerticesAndIndices(out vertices, out indices);
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


        /// <summary>
        /// Calculates the actual world matrix used in the shader
        /// </summary>
        private Matrix calculateObjectWorldMatrix()
        {
            return Matrix.CreateTranslation(-pivotPoint) * Matrix.CreateScale(Dimensions) * worldMatrix;
        }

        private void UpdateWorldMatrix()
        {
            if (shader == null) return;
            shader.World = calculateObjectWorldMatrix();
        }




    }
}
