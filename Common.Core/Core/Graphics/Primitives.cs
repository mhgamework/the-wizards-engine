using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Entity.Rendering;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Common.Core.Collada;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient;

namespace MHGameWork.TheWizards.Common.Core.Graphics
{
    /// <summary>
    /// TODO: the WorldMatrix field should be deprecated here!
    /// </summary>
    public class Primitives : IRenderPrimitives
    {
        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;
        public int PrimitiveCount;
        public int VertexCount;
        public int VertexStride;

        public void RenderPrimitives()
        {
            if ( VertexBuffer == null ) return;
            if ( IndexBuffer == null ) return;
            if ( PrimitiveCount == 0 ) return;
            if ( VertexStride == 0 ) return;
            GraphicsDevice device = VertexBuffer.GraphicsDevice;

            device.Indices = IndexBuffer;
            device.Vertices[ 0 ].SetSource( VertexBuffer, 0, VertexStride );
            device.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0, VertexCount, 0, PrimitiveCount );


        }

        public void InitializeFromVertices<T>( IXNAGame game, T[] verts, int _vertexStride ) where T : struct
        {
            if ( VertexBuffer != null ) VertexBuffer.Dispose();
            if ( IndexBuffer != null ) IndexBuffer.Dispose();

            VertexStride = _vertexStride;
            PrimitiveCount = verts.Length / 3;
            VertexCount = verts.Length;

            // TODO: Optimize vertices first and build index buffer from that!
            //ushort[] indices = OptimizeVertexBufferSkinned();

            // We only support max. 65535 optimized vertices, which is really a
            // lot, but more would require a int index buffer (twice as big) and
            // I have never seen any realtime 3D model that needs more vertices ^^
            if ( verts.Length > ushort.MaxValue )
                throw new InvalidOperationException(
                    "Too much vertices to index, optimize vertices or use " +
                    "fewer vertices. Vertices=" + verts.Length +
                    ", Max Vertices for Index Buffer=" + ushort.MaxValue );

            ushort[] indices = new ushort[ verts.Length ];
            for ( ushort i = 0; i < indices.Length; i++ )
            {
                indices[ i ] = i;
            }

            // Create the vertex buffer from our vertices.
            VertexBuffer = new VertexBuffer( game.GraphicsDevice, verts.Length * VertexStride, BufferUsage.WriteOnly );
            VertexBuffer.SetData( verts );



            // Create the index buffer from our indices (Note: While the indices
            // will point only to 16bit (ushort) vertices, we can have a lot
            // more indices in this list than just 65535).
            IndexBuffer = new IndexBuffer(
                game.GraphicsDevice,
                typeof( ushort ),
                indices.Length, BufferUsage.WriteOnly );
            IndexBuffer.SetData( indices );
        }

        public Microsoft.Xna.Framework.Matrix WorldMatrix
        {
            get { return Matrix.Identity; }
        }
    }
}
