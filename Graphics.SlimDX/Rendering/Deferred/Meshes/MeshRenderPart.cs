using System;
using SlimDX;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public class MeshRenderPart :IDisposable
    {
        public Matrix ObjectMatrix;
        public Buffer VertexBuffer;
        public Buffer IndexBuffer;
        public int PrimitiveCount;
        public int VertexCount;

        public void Dispose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
            VertexBuffer = null;
            IndexBuffer = null;
        }
    }
}