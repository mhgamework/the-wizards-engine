using System;
using System.Collections.Generic;
using SlimDX;
using System.Linq;

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
        public List<DeferredMeshElement> Elements = new List<DeferredMeshElement>();
        public MeshRenderMaterial[] Materials;


        public void Dispose()
        {
            if (Elements.Any(el => !el.IsDeleted)) throw new InvalidOperationException("Cannot delete this mesh's cache since there are still elements using the renderdata.");


            foreach (var mat in Materials)
                mat.Dispose();
            Materials = null;
        }
    }
}