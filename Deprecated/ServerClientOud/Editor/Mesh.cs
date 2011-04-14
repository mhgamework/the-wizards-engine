using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient.Editor
{
    /// <summary>
    /// Basic mesh for rendering
    /// </summary>
    public class Mesh
    {
        public int TriangleCount;
        public int VertexCount;
        public int VertexStride;
        public VertexDeclaration VertexDeclaration;

        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;

        public Matrix LocalMatrix = Matrix.Identity;

        private List<TangentVertex> vertices;

        public List<TangentVertex> Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        private List<int> indices;

        public List<int> Indices
        {
            get { return indices; }
            set { indices = value; }
        }


        public BasicShader Shader;


        public Mesh()
        {
        }

        public static Mesh FromColladaMeshPrimitiveList( IXNAGame game, ColladaMesh.PrimitiveList meshPart )
        {
            Mesh mesh = new Mesh();

            mesh.TriangleCount = meshPart.PrimitiveCount;

            List<TangentVertex> vertices = new List<TangentVertex>();
            List<int> indices = new List<int>();
            // Construct data
            vertices.Clear();


            // We have to use int indices here because we often have models
            // with more than 64k triangles (even if that gets optimized later).
            for ( int i = 0; i < mesh.TriangleCount * 3; i++ )
            {
                if ( i == int.MaxValue ) throw new Exception();

                Vector3 position = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Position, i ); //GetPosition( i );
                Vector3 normal = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Normal, i );


                // Texture Coordinates
                Vector3 texcoord = Vector3.Zero;
                if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Texcoord, 1 ) )
                {
                    texcoord = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Texcoord, 1, i );
                    texcoord.Y = 1.0f - texcoord.Y; // V coordinate is inverted in max
                }


                // Tangent
                Vector3 tangent = Vector3.Zero;
                if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.TexTangent, 1 ) )
                {
                    tangent = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.TexTangent, 1, i );
                }

                vertices.Add( new TangentVertex( position, texcoord.X, texcoord.Y, normal, tangent ) );
                indices.Add( i );


            }

            //
            // Create the Vertex buffer and the index buffer
            //

            mesh.VertexCount = vertices.Count;
            mesh.VertexDeclaration = TangentVertex.CreateVertexDeclaration( game );
            mesh.VertexStride = TangentVertex.SizeInBytes;

            // Create the vertex buffer from our vertices.
            mesh.VertexBuffer = new VertexBuffer( game.GraphicsDevice, typeof( TangentVertex ), mesh.VertexCount, BufferUsage.None );
            mesh.VertexBuffer.SetData( vertices.ToArray() );

            //// We only support max. 65535 optimized vertices, which is really a
            //// lot, but more would require a int index buffer (twice as big) and
            //// I have never seen any realtime 3D model that needs more vertices ^^
            //if ( modelPart.VertexCount > ushort.MaxValue )
            //    throw new InvalidOperationException(
            //        "Too much vertices to index, optimize vertices or use " +
            //        "fewer vertices. Vertices=" + vertices.Count +
            //        ", Max Vertices for Index Buffer=" + ushort.MaxValue );

            // Create the index buffer from our indices (Note: While the indices
            // will point only to 16bit (ushort) vertices, we can have a lot
            // more indices in this list than just 65535).
            mesh.IndexBuffer = new IndexBuffer( game.GraphicsDevice, typeof( int ), indices.Count, BufferUsage.None );
            mesh.IndexBuffer.SetData( indices.ToArray() );


            //
            // Load the shader and set the material properties
            //

            mesh.Shader = BasicShader.LoadFromFXFile( game, game.EngineFiles.GetColladaModelShaderStream(), null );

            //modelPart.Shader.SetTechnique( "SpecularPerPixelNormalMapping" );
            //modelPart.Shader.SetTechnique( "SpecularPerPixel" );
            mesh.Shader.SetTechnique( "SpecularPerPixelColored" );


            //TODO: world matrix not correctly implemented
            //TODO: lightdir

            mesh.Shader.SetParameter( "lightDir", Vector3.Normalize( new Vector3( 0.6f, 1f, 0.6f ) ) );
            //lightDir.SetValue( -engine.ActiveCamera.CameraDirection );
            //lightDir.SetValue( BaseGame.LightDirection );
            ColladaMaterial mat = meshPart.Material;

            // Set all material properties
            if ( mat != null )
            {
                mesh.Shader.SetParameter( "ambientColor", new Vector4( 0.25f, 0.25f, 0.25f, 1f ) );
                //AmbientColor = setMat.Ambient;
                mesh.Shader.SetParameter( "diffuseColor", mat.Diffuse );
                mesh.Shader.SetParameter( "specularColor", mat.Specular );

                //modelPart.Shader.SetParameter( "shininess", 80f );
                //modelPart.Shader.SetParameter( "shininess", mat.Shininess );


                TWTexture texture;
                if ( mat.DiffuseTexture != null )
                {
                    texture = TWTexture.FromImageFile( game, new GameFile( mat.DiffuseTexture.GetFullFilename() ) );

                    mesh.Shader.SetTechnique( "SpecularPerPixel" );

                    mesh.Shader.SetParameter( "diffuseTexture", texture );
                    mesh.Shader.SetParameter( "diffuseTextureRepeatU", mat.DiffuseTextureRepeatU );
                    mesh.Shader.SetParameter( "diffuseTextureRepeatV", mat.DiffuseTextureRepeatV );

                }
                if ( mat.NormalTexture != null )
                {
                    texture = TWTexture.FromImageFile( game, new GameFile( mat.NormalTexture.GetFullFilename() ) );

                    mesh.Shader.SetTechnique( "SpecularPerPixel" );

                    mesh.Shader.SetParameter( "normalTexture", texture );
                    mesh.Shader.SetParameter( "normalTextureRepeatU", mat.NormalTextureRepeatU );
                    mesh.Shader.SetParameter( "normalTextureRepeatV", mat.NormalTextureRepeatV );

                }
                //NormalTexture = setMat.normalTexture;
            } // if (setMat)


            return mesh;

        }

        /// <summary>
        /// Render using the data in this mesh and the shader in this mesh
        /// </summary>
        /// <param name="game"></param>
        public void RenderDirect( IXNAGame game, Matrix worldMatrix )
        {


            /*Game.GraphicsDevice.RenderState.AlphaTestEnable = true;
            Game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
            Game.GraphicsDevice.RenderState.ReferenceAlpha = 200;*/


            Shader.World = LocalMatrix * worldMatrix;
            Shader.ViewProjection = game.Camera.ViewProjection;
            Shader.ViewInverse = game.Camera.ViewInverse;
            Shader.RenderMultipass( RenderPrimitives );


        }


        public void RenderPrimitives()
        {
            GraphicsDevice device = VertexBuffer.GraphicsDevice;
            device.Vertices[ 0 ].SetSource( VertexBuffer, 0, VertexStride );
            device.Indices = IndexBuffer;
            device.VertexDeclaration = VertexDeclaration;
            device.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, VertexCount, 0, TriangleCount );

        }

    }
}
