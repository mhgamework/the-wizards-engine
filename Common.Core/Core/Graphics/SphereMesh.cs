using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Common.Core.Graphics;
namespace MHGameWork.TheWizards.Graphics
{
    /// <summary>
    /// Represents a box that starts at 0,0,0 and extends in the axis directions with a length
    /// </summary>
    public class SphereMesh : IXNAObject
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

        private int segments;
        public int Segments
        {
            [DebuggerStepThrough]
            get { return segments; }
            [DebuggerStepThrough]
            set { segments = value; }
        }

        private float radius;
        public float Radius
        {
            [DebuggerStepThrough]
            get { return radius; }
            [DebuggerStepThrough]
            set { radius = value;
                UpdateWorldMatrix(); calculateBoundingBox(); }
        }

        private Matrix worldMatrix;
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; UpdateWorldMatrix();
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

        private Primitives primitives;


        public SphereMesh()
        {
            worldMatrix = Matrix.Identity;
            radius = 1;
            color = Color.Green;
            segments = 12;
            lightDirection = Vector3.Normalize(new Vector3(-2, -1, -1));
        }

        public virtual void Initialize(IXNAGame game)
        {

            TangentVertex[] vertices;
            short[] indices;
            CreateUnitSphereVerticesAndIndices(segments, out vertices, out indices);

            primitives = new Primitives();

            primitives.VertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertices.Length, BufferUsage.None);
            primitives.VertexBuffer.SetData<TangentVertex>(vertices);

            primitives.IndexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), indices.Length, BufferUsage.None);
            primitives.IndexBuffer.SetData(indices);

            primitives.VertexCount = vertices.Length;
            primitives.PrimitiveCount = indices.Length / 3;
            primitives.VertexStride = TangentVertex.SizeInBytes;

            ReloadShader(game);

            vertexDeclaration = TangentVertex.CreateVertexDeclaration(game);


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

        public SphereMesh(float radius, int segments, Color color)
            : this()
        {
            this.segments = segments;
            this.color = color;
            this.radius = radius;
            calculateBoundingBox();
        }

        private void calculateBoundingBox()
        {
            Vector3 transl = worldMatrix.Translation;
            BoundingBox bb = new BoundingBox(Vector3.One * -radius + transl, Vector3.One * radius + transl);

            boundingBox = bb;
        }

        /// <summary>
        /// Calculates the actual world matrix used in the shader
        /// </summary>
        private Matrix calculateObjectWorldMatrix()
        {
            return Matrix.CreateScale(radius) * worldMatrix;
        }

        public static void CreateUnitSphereVerticesAndIndices(int segments, out TangentVertex[] vertices, out short[] indices)
        {
            // Source: http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/sphere_cylinder/



            // Maak ringen van vertices van onder naar boven


            int i = 0;

            float phi, theta;
            float phiStep, thetaStep;
            float phiStart, phiEnd, thetaStart, thetaEnd;
            phiStep = MathHelper.TwoPi / segments;
            thetaStep = MathHelper.Pi / segments;

            phiStart = 0;
            phiEnd = MathHelper.TwoPi;
            thetaStart = -MathHelper.PiOver2 + thetaStep;
            thetaEnd = MathHelper.PiOver2;

            int numRings = (int)Math.Round((thetaEnd - thetaStart) / thetaStep);
            int numVertsOnRing = (int)Math.Round((phiEnd - phiStart) / phiStep);




            int numVertices = 1 + numRings * numVertsOnRing + 1;


            vertices = new TangentVertex[numVertices];

            // Bottom vertex: (0,-1,0)
            vertices[i].pos = new Vector3(0, -1, 0);
            i++;

            theta = thetaStart;
            for (int iRing = 0; iRing < numRings; iRing++, theta += thetaStep)
            {
                phi = 0;
                for (int iVert = 0; iVert < numVertsOnRing; iVert++, phi += phiStep)
                {
                    vertices[i].pos = new Vector3(
                        (float)Math.Cos(theta) * (float)Math.Cos(phi),
                        (float)Math.Sin(theta),
                        -(float)Math.Cos(theta) * (float)Math.Sin(phi));
                    //TODO: normals
                    i++;
                }
            }
            // Top vertex: (0,1,0)
            vertices[i].pos = new Vector3(0, 1, 0);
            i++;


            // Generate normals
            for (int j = 0; j < vertices.Length; j++)
            {
                vertices[j].normal = Vector3.Normalize(vertices[j].pos);
            }


            int numIndices = (numVertsOnRing * 2 * 3) * numRings;

            indices = new short[numIndices];
            i = 0;

            // Triangle fan at bottom and top, elsewhere strips between the rings

            // Top and bottom fan

            for (int iVert = 0; iVert < numVertsOnRing - 1; iVert++)
            {
                // Bottom fan
                indices[i] = (short)(0); i++;
                indices[i] = (short)(1 + iVert); i++;
                indices[i] = (short)(1 + (iVert + 1)); i++;

                // Top fan
                indices[i] = (short)(numVertices - 1); i++;
                indices[i] = (short)(1 + (numRings - 1) * numVertsOnRing + (iVert + 1)); i++;
                indices[i] = (short)(1 + (numRings - 1) * numVertsOnRing + iVert); i++;
            }

            // Top and bottom final fan
            indices[i] = (short)(0); i++;
            indices[i] = (short)(1 + numVertsOnRing - 1); i++;
            indices[i] = (short)(1 + 0); i++;

            indices[i] = (short)(numVertices - 1); i++;
            indices[i] = (short)(1 + (numRings - 1) * numVertsOnRing + 0); i++;
            indices[i] = (short)(1 + (numRings - 1) * numVertsOnRing + numVertsOnRing - 1); i++;

            // Strips
            for (int iRing = 0; iRing < numRings - 1; iRing++)
            {
                for (int iVert = 0; iVert < numVertsOnRing - 1; iVert++)
                {
                    indices[i] = (short)(1 + numVertsOnRing * iRing + iVert); i++;
                    indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + iVert); i++;
                    indices[i] = (short)(1 + numVertsOnRing * iRing + (iVert + 1)); i++;

                    indices[i] = (short)(1 + numVertsOnRing * iRing + (iVert + 1)); i++;
                    indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + iVert); i++;
                    indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + (iVert + 1)); i++;
                }
                // Final gap:
                indices[i] = (short)(1 + numVertsOnRing * iRing + (numVertsOnRing - 1)); i++;
                indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + (numVertsOnRing - 1)); i++;
                indices[i] = (short)(1 + numVertsOnRing * iRing + (0)); i++;

                indices[i] = (short)(1 + numVertsOnRing * iRing + (0)); i++;
                indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + (numVertsOnRing - 1)); i++;
                indices[i] = (short)(1 + numVertsOnRing * (iRing + 1) + (0)); i++;
            }

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


        //#region IRaycastable<BoxMesh,RaycastResult<BoxMesh>> Members

        //public MHGameWork.TheWizards.Raycast.RaycastResult<BoxMesh> Raycast( Ray ray )
        //{
        //    // Not test anymore!
        //    Vector3 testpoint = ray.Position + ray.Direction;

        //    Matrix mat = shader.World;
        //    mat = Matrix.Invert( mat );
        //    if ( float.IsNaN( mat.M11 ) || float.IsInfinity( mat.M11 ) )
        //    {
        //        // Inverse bestaat niet! Dit is dus een verkeerde transform matrix!!
        //        // Dit komt doordat de een van de coords van de dimensions 0 is. Doe dan geen raycast
        //        return new MHGameWork.TheWizards.Raycast.RaycastResult<BoxMesh>( (float?)null, this );
        //    }
        //    ray.Position = Vector3.Transform( ray.Position, mat );

        //    testpoint = Vector3.Transform( testpoint, mat );
        //    Vector3 testDir = Vector3.Normalize( testpoint - ray.Position );
        //    //TODO: check this
        //    //EDIT: this doesnt work, now using testdir
        //    ray.Direction = Vector3.Transform( ray.Direction, mat );
        //    ray.Direction = Vector3.Normalize( ray.Direction );

        //    ray.Direction = testDir;

        //    BoundingBox bb = new BoundingBox( Vector3.Zero, Vector3.One );
        //    float? dist = ray.Intersects( bb );

        //    MHGameWork.TheWizards.Raycast.RaycastResult<BoxMesh> result;
        //    result = new MHGameWork.TheWizards.Raycast.RaycastResult<BoxMesh>( dist, this );

        //    return result;
        //}

        //#endregion
    }
}
