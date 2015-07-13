using System;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Water
{
    public class Mesh : IDisposable
    {
        public int NumberFaces;
        public int NumberVertices;

        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;
        public int VertexStride;
        public VertexDeclaration VertexDeclaration;


        public Mesh( IXNAGame game, int numFaces, int numVerts, int strideSize, VertexDeclaration decl )
        {
            VertexDeclaration = decl;
            NumberFaces = numFaces;
            NumberVertices = numVerts;

            VertexBuffer = new VertexBuffer( game.GraphicsDevice, strideSize * numVerts, BufferUsage.None );
            IndexBuffer = new IndexBuffer( game.GraphicsDevice, numFaces * 3 * sizeof( int ), BufferUsage.None, IndexElementSize.ThirtyTwoBits );

            VertexStride = strideSize;
        }


        public void SetVertexBufferData<T>( T[] v ) where T : struct
        {
            VertexBuffer.SetData( v );
        }

        public T[] GetVertices<T>() where T : struct
        {
            T[] ret = new T[ NumberVertices ];
            VertexBuffer.GetData( ret );
            return ret;

        }

        internal void SetIndexBufferData( int[] indices )
        {
            IndexBuffer.SetData( indices );
            //throw new Exception( "The method or operation is not implemented." );
        }

        public void GenerateAdjacency( float f, int[] adjac )
        {
            //throw new NotImplementedException();
        }

        public void OptimizeInPlace( object o, int[] adjac )
        {
            //throw new NotImplementedException();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if ( VertexBuffer != null ) VertexBuffer.Dispose();
            if ( IndexBuffer != null ) IndexBuffer.Dispose();

            //throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        public void DrawSubset( int i )
        {
            VertexBuffer.GraphicsDevice.VertexDeclaration = VertexDeclaration;
            VertexBuffer.GraphicsDevice.Vertices[ 0 ].SetSource( VertexBuffer, 0, VertexStride );
            VertexBuffer.GraphicsDevice.Indices = IndexBuffer;
            VertexBuffer.GraphicsDevice.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, NumberVertices, 0, NumberFaces );
        }
    }
}
