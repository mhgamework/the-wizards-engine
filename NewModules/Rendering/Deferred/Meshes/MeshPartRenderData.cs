using System;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MHGameWork.TheWizards.Rendering.Deferred.Meshes
{
    /// <summary>
    /// Responsible for executing a draw call for a meshpart 
    /// </summary>
    public class MeshPartRenderData:IDisposable
    {
        public Buffer VertexBuffer;
        public Buffer IndexBuffer;
        public int PrimitiveCount;
        //public int VertexCount;


        

        public void Draw( DeviceContext context )
        {
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetIndexBuffer(IndexBuffer, SlimDX.DXGI.Format.R32_UInt, 0); //Using int indexbuffers
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, DeferredMeshVertex.SizeInBytes, 0));

            context.DrawIndexed(PrimitiveCount * 3, 0, 0);
            //drawCalls = DrawCalls + 1;
        }

        public void Dispose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
            VertexBuffer = null;
            IndexBuffer = null;
        }
    }
}