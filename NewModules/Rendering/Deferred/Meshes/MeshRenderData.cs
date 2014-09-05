using System;
using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public class MeshRenderData : IDisposable
    {
        public IMesh Mesh;

        public MeshRenderData(IMesh mesh)
        {
            Mesh = mesh;
        }

        public List<Matrix> WorldMatrices = new List<Matrix>();
        public List<DeferredMeshRenderElement> Elements = new List<DeferredMeshRenderElement>();
        public MeshRenderMaterial[] Materials;


        public void Dispose()
        {
            if (Elements.Count > 0) throw new InvalidOperationException("Cannot delete this mesh's cache since there are still elements using the renderdata.");


            foreach (var mat in Materials)
                mat.Dispose();
            Materials = null;
        }
    }
}