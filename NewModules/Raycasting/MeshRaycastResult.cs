using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Raycasting
{
    public struct MeshRaycastResult
    {
        public VertexPositionNormalTexture Vertex1;
        public VertexPositionNormalTexture Vertex2;
        public VertexPositionNormalTexture Vertex3;
        public float U;
        public float V;
        public MeshCoreData.Material Material;
    }
}
