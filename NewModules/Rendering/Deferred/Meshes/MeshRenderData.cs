﻿using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public class MeshRenderData
    {
        public IMesh Mesh;

        public MeshRenderData(IMesh mesh)
        {
            Mesh = mesh;
        }

        public List<Matrix> WorldMatrices = new List<Matrix>();
        public List<DeferredRendererMeshes> Elements = new List<DeferredRendererMeshes>();
        public MeshRenderMaterial[] Materials;

    }
}