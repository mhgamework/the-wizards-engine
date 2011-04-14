using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Entity;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class EditorModelRenderData : IRenderPrimitives, IDisposable
    {
        private Matrix objectMatrix;

        public int TriangleCount;
        public int VertexCount;
        public int VertexStride;
        public VertexDeclaration VertexDeclaration;

        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;

        public EditorMaterial Material;

        public void SetParentWorldMatrix( Matrix parentMatrix )
        {
            worldMatrix = ObjectMatrix * parentMatrix;

        }

        private Matrix worldMatrix = Matrix.Identity;
        Matrix IRenderPrimitives.WorldMatrix
        {
            get
            {
                return worldMatrix;
            }
        }

        public Matrix ObjectMatrix
        {
            get { return objectMatrix; }
            set
            {
                //do as trick to keep the parentMatrix
                worldMatrix = worldMatrix * Matrix.Invert( objectMatrix ) * value;
                objectMatrix = value;
            }
        }

        /// <summary>
        /// TODO: This method is fishy, because of a batching method for material that is not shared among other models yet
        /// </summary>
        public void Render( IXNAGame game )
        {
            RenderBatched();
            Material.Render( game );

        }


        public void RenderBatched()
        {
            if ( Material != null )
                Material.RenderPrimitivesBatched( this );
        }

        void IRenderPrimitives.RenderPrimitives()
        {
            //Note: We could require the graphicsdevice as a parameter, but since the vertexbuffer or indexbuffer
            //        won't render (i think?) on any other device than the one it was created on,
            //        we can safely assume that we are rendering to that graphicsdevice.
            VertexBuffer.GraphicsDevice.Vertices[ 0 ].SetSource( VertexBuffer, 0, VertexStride );
            VertexBuffer.GraphicsDevice.Indices = IndexBuffer;
            VertexBuffer.GraphicsDevice.VertexDeclaration = VertexDeclaration;
            VertexBuffer.GraphicsDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, VertexCount, 0, TriangleCount );
        }

        //public static EditorModelRenderData FromEditorModelFullData( EditorModelFullData fullData )



        public void Initialize( IXNAGame game, ModelFullData fullData )
        {
            ObjectMatrix = fullData.ObjectMatrix;

            TriangleCount = fullData.TriangleCount;
            VertexCount = fullData.TriangleCount * 3;

            // Use TangentVertex for now
            TangentVertex[] vertices = new TangentVertex[ VertexCount ];
            // We are not really using the indexbuffer at the moment
            int[] indices = new int[ VertexCount ];


            // Construct data

            // We have to use int indices here because we often have models
            // with more than 64k triangles (even if that gets optimized later).
            // TODO: optimze the triangles
            for ( int i = 0; i < VertexCount; i++ )
            {
                vertices[ i ] = new TangentVertex( fullData.Positions[ i ], fullData.TexCoords[ i ].X, fullData.TexCoords[ i ].Y, fullData.Normals[ i ], fullData.Tangents[ i ] );
                indices[ i ] = i;
            }


            EditorMaterialCollada mat = new EditorMaterialCollada();

            mat.Ambient = fullData.Ambient;
            mat.Diffuse = fullData.Diffuse;
            mat.Specular = fullData.Specular;
            mat.Shininess = fullData.Shininess;

            if ( fullData.DiffuseTexture != null )
                mat.DiffuseTexture = fullData.DiffuseTexture;
            mat.DiffuseTextureRepeatU = fullData.DiffuseTextureRepeatU;
            mat.DiffuseTextureRepeatV = fullData.DiffuseTextureRepeatV;

            if ( mat.NormalTexture != null )
                mat.NormalTexture = fullData.NormalTexture;
            mat.NormalTextureRepeatU = fullData.NormalTextureRepeatU;
            mat.NormalTextureRepeatV = fullData.NormalTextureRepeatV;

            mat.Initialize( game );

            Material = mat;

            //
            // Create the Vertex buffer and the index buffer
            //

            //VertexCount = vertices.Count;
            VertexDeclaration = TangentVertex.CreateVertexDeclaration( game );
            VertexStride = TangentVertex.SizeInBytes;

            // Create the vertex buffer from our vertices.
            VertexBuffer = new VertexBuffer( game.GraphicsDevice, typeof( TangentVertex ), VertexCount, BufferUsage.None );
            VertexBuffer.SetData( vertices );

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

            // MHGW EDIT: TODO: at the moment int indices, should be short
            IndexBuffer = new IndexBuffer( game.GraphicsDevice, typeof( int ), indices.Length, BufferUsage.None );
            IndexBuffer.SetData( indices );

        }


        #region IDisposable Members

        public void Dispose()
        {
            if ( VertexDeclaration != null )
                VertexDeclaration.Dispose();
            if ( IndexBuffer != null )
                IndexBuffer.Dispose();
            if ( VertexBuffer != null )
                VertexBuffer.Dispose();
            VertexDeclaration = null;
            IndexBuffer = null;
            VertexBuffer = null;
        }

        #endregion
    }
}
