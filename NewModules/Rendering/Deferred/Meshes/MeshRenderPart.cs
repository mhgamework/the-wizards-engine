using SlimDX;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public class MeshRenderPart
    {
        public Matrix ObjectMatrix;
        public Buffer VertexBuffer;
        public Buffer IndexBuffer;
        public int PrimitiveCount;
        public int VertexCount;
    }
}