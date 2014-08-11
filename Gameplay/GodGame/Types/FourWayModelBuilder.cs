using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    public class FourWayModelBuilder
    {
        public IMesh BaseMesh;
        public IMesh WayMesh; //must be aligned in (1,0) "left" direction
        public IMesh NoWayMesh; //must be aligned in (1,0) "left" direction

        private Dictionary<string, IMesh> cachedMeshes = new Dictionary<string, IMesh>();

        public IMesh CreateMesh(bool hasLeft, bool hasTop, bool hasRight, bool hasBottom)
        {
            if (BaseMesh == null && (WayMesh == null || NoWayMesh == null))
                throw new Exception("Meshes not initialized");

            IMesh mesh;
            var cacheString = getCacheString(hasLeft, hasTop, hasRight, hasBottom);
            cachedMeshes.TryGetValue(cacheString, out mesh);
            if (mesh != null)
                return mesh;

            mesh = BaseMesh != null ? MeshBuilder.Transform(BaseMesh, Matrix.Identity) : new RAMMesh();

            appendMeshTo(hasLeft ? WayMesh : NoWayMesh, mesh, Matrix.Identity);
            appendMeshTo(hasTop ? WayMesh : NoWayMesh, mesh, Matrix.RotationY(-(float)Math.PI * 0.5f));
            appendMeshTo(hasRight ? WayMesh : NoWayMesh, mesh, Matrix.RotationY((float)Math.PI));
            appendMeshTo(hasBottom ? WayMesh : NoWayMesh, mesh, Matrix.RotationY((float)Math.PI * 0.5f));

            cachedMeshes.Add(cacheString, mesh);

            return mesh;
        }

        private void appendMeshTo(IMesh source, IMesh destination, Matrix transform)
        {
            if (source == null)
                return;
            MeshBuilder.AppendMeshTo(source, destination, transform);
        }

        private string getCacheString(bool hasLeft, bool hasTop, bool hasRight, bool hasBottom)
        {
            var ret = "";
            ret += hasLeft ? "1" : "0";
            ret += hasTop ? "1" : "0";
            ret += hasRight ? "1" : "0";
            ret += hasBottom ? "1" : "0";
            return ret;
        }
    }
}