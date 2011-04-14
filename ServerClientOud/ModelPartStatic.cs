using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.ServerClient
{
    class ModelPartStatic : IModelPart
    {
        public TWModel model;

        public Matrix localMatrix = Matrix.Identity;

        public int TriangleCount;
        public int VertexCount;
        public int VertexStride;
        public VertexDeclaration VertexDeclaration;

        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;

        public bool containsAlfa = false;
        public BasicShader Shader;

        /// <summary>
        /// The product of the local matrix and the parent models world matrix;
        /// </summary>
        private Matrix worldMatrix = Matrix.Identity;


        public ModelPartStatic( TWModel nModel )
        {
            model = nModel;
        }



        public void SaveToXML( TWXmlNode node )
        {
            node.AddChildNode( "TriangleCount", TriangleCount.ToString() );
            node.AddChildNode( "VertexCount", VertexCount.ToString() );
            node.AddChildNode( "VertexStride", VertexStride.ToString() );
            //XMLSerializer.AddNodeVertexDeclaration( node, "VertexDeclaration", VertexDeclaration );
            //XMLSerializer.AddNodeVertexBuffer( node, "VertexBuffer", VertexBuffer );
            //XMLSerializer.AddNodeIndexBuffer( node, "IndexBuffer", IndexBuffer );



            TWXmlNode shaderNode = node.CreateChildNode( "Shader" );
            Shader.SaveToXML( shaderNode );


        }

        public void LoadFromXML( TWXmlNode node )
        {
        }


        public static ModelPartStatic FromColladaMeshPart( TWModel model, ColladaMesh.PrimitiveList meshPart )
        {
            ModelPartStatic modelPart = new ModelPartStatic( model );

            modelPart.TriangleCount = meshPart.PrimitiveCount;

            List<TangentVertex> vertices = new List<TangentVertex>();
            List<int> indices = new List<int>();
            // Construct data
            vertices.Clear();


            // We have to use int indices here because we often have models
            // with more than 64k triangles (even if that gets optimized later).
            for ( int i = 0; i < modelPart.TriangleCount * 3; i++ )
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

            modelPart.VertexCount = vertices.Count;
            modelPart.VertexDeclaration = TangentVertex.CreateVertexDeclaration( modelPart.Game );
            modelPart.VertexStride = TangentVertex.SizeInBytes;

            // Create the vertex buffer from our vertices.
            modelPart.VertexBuffer = new VertexBuffer( modelPart.Game.GraphicsDevice, typeof( TangentVertex ), modelPart.VertexCount, BufferUsage.None );
            modelPart.VertexBuffer.SetData( vertices.ToArray() );

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
            modelPart.IndexBuffer = new IndexBuffer( modelPart.Game.GraphicsDevice, typeof( int ), indices.Count, BufferUsage.None );
            modelPart.IndexBuffer.SetData( indices.ToArray() );


            //
            // Load the shader and set the material properties
            //

            
            modelPart.Shader = BasicShader.LoadFromFXFile( modelPart.Game,
                modelPart.Game.EngineFiles.GetColladaModelShaderStream(), null );

            //modelPart.Shader.SetTechnique( "SpecularPerPixelNormalMapping" );
            modelPart.Shader.SetTechnique( "SpecularPerPixel" );
            //modelPart.Shader.SetTechnique( "SpecularPerPixelColored" );


            //TODO: world matrix not correctly implemented
            //TODO: lightdir

            modelPart.Shader.SetParameter( "lightDir", Vector3.Normalize( new Vector3( 0.6f, 1f, 0.6f ) ) );
            //lightDir.SetValue( -engine.ActiveCamera.CameraDirection );
            //lightDir.SetValue( BaseGame.LightDirection );
            ColladaMaterial mat = meshPart.Material;

            // Set all material properties
            if ( mat != null )
            {
                modelPart.Shader.SetParameter( "ambientColor", new Vector4( 0.25f, 0.25f, 0.25f, 1f ) );
                //AmbientColor = setMat.Ambient;
                modelPart.Shader.SetParameter( "diffuseColor", mat.Diffuse );
                modelPart.Shader.SetParameter( "specularColor", mat.Specular );

                //modelPart.Shader.SetParameter( "shininess", 80f );
                //modelPart.Shader.SetParameter( "shininess", mat.Shininess );


                TWTexture texture;
                if ( mat.DiffuseTexture != null )
                {
                    texture = TWTexture.FromImageFile( modelPart.Game, new GameFile( mat.DiffuseTexture.GetFullFilename() ) );

                    modelPart.Shader.SetParameter( "diffuseTexture", texture );
                    modelPart.Shader.SetParameter( "diffuseTextureRepeatU", mat.DiffuseTextureRepeatU );
                    modelPart.Shader.SetParameter( "diffuseTextureRepeatV", mat.DiffuseTextureRepeatV );
                }
                if ( mat.NormalTexture != null )
                {
                    texture = TWTexture.FromImageFile( modelPart.Game, new GameFile( mat.NormalTexture.GetFullFilename() ) );

                    modelPart.Shader.SetParameter( "normalTexture", texture );
                    modelPart.Shader.SetParameter( "normalTextureRepeatU", mat.NormalTextureRepeatU );
                    modelPart.Shader.SetParameter( "normalTextureRepeatV", mat.NormalTextureRepeatV );

                }
                //NormalTexture = setMat.normalTexture;
            } // if (setMat)


            return modelPart;
        }

        #region IModelPart Members

        public void Render()
        {

            /*Game.GraphicsDevice.RenderState.AlphaTestEnable = true;
            Game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
            Game.GraphicsDevice.RenderState.ReferenceAlpha = 200;*/


            Shader.World = worldMatrix;
            Shader.ViewProjection = Game.Camera.ViewProjection;
            Shader.ViewInverse = Game.Camera.ViewInverse;
            Shader.RenderMultipass( RenderPrimitives );


        }

        public void RenderPrimitives()
        {
            Game.GraphicsDevice.Vertices[ 0 ].SetSource( VertexBuffer, 0, VertexStride );
            Game.GraphicsDevice.Indices = IndexBuffer;
            Game.GraphicsDevice.VertexDeclaration = VertexDeclaration;
            Game.GraphicsDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, VertexCount, 0, TriangleCount );
        }

        public void UpdateWorldMatrix( Microsoft.Xna.Framework.Matrix newWorldMatrix )
        {
            worldMatrix = localMatrix * newWorldMatrix;
        }

        #endregion

        public IXNAGame Game
        {
            get { return model.Game; }
        }

    }
}
