using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Rendering
{
    public class FourWayModelBuilder
    {
        public IMesh BaseMesh;
        public IMesh WayMesh; //must be aligned in (1,0) "left" direction
        public IMesh NoWayMesh; //must be aligned in (1,0) "left" direction

        public List<IMesh> HeightWayMeshes;

        private Dictionary<string, IMesh> cachedMeshes = new Dictionary<string, IMesh>();

        public IMesh CreateMesh(bool hasLeft, bool hasTop, bool hasRight, bool hasBottom)
        {
            /*if (BaseMesh == null && (WayMesh == null || NoWayMesh == null))
                throw new Exception("Meshes not initialized");

            IMesh mesh;
            var cacheString = getCacheString(hasLeft, 0, hasTop, 0, hasRight, 0, hasBottom, 0);
            cachedMeshes.TryGetValue(cacheString, out mesh);
            if (mesh != null)
                return mesh;

            mesh = BaseMesh != null ? MeshBuilder.Transform(BaseMesh, Matrix.Identity) : new RAMMesh();

            appendMeshTo(hasLeft ? WayMesh : NoWayMesh, mesh, Matrix.Identity);
            appendMeshTo(hasTop ? WayMesh : NoWayMesh, mesh, Matrix.RotationY(-(float)Math.PI * 0.5f));
            appendMeshTo(hasRight ? WayMesh : NoWayMesh, mesh, Matrix.RotationY((float)Math.PI));
            appendMeshTo(hasBottom ? WayMesh : NoWayMesh, mesh, Matrix.RotationY((float)Math.PI * 0.5f));

            cachedMeshes.Add(cacheString, mesh);

            return mesh;*/

            return CreateMesh(hasLeft, 0, hasTop, 0, hasRight, 0, hasBottom, 0);
        }

        public IMesh CreateMesh(bool hasLeft, int leftHeight, bool hasTop, int topHeight, bool hasRight, int rightHeight, bool hasBottom, int bottomHeight)
        {
            if (HeightWayMeshes == null)
            {
                leftHeight = 0;
                rightHeight = 0;
                topHeight = 0;
                bottomHeight = 0;
            }


            if (BaseMesh == null && (WayMesh == null || NoWayMesh == null))
                throw new Exception("Meshes not initialized");

            IMesh mesh;
            var cacheString = getCacheString(hasLeft, leftHeight, hasTop, topHeight, hasRight, rightHeight, hasBottom, bottomHeight);
            cachedMeshes.TryGetValue(cacheString, out mesh);
            if (mesh != null)
                return mesh;

            mesh = BaseMesh != null ? MeshBuilder.Transform(BaseMesh, Matrix.Identity) : new RAMMesh();

            appendMeshTo(hasLeft ? GetHeightWayMesh(leftHeight) : NoWayMesh, mesh, Matrix.Identity);
            appendMeshTo(hasTop ? GetHeightWayMesh(topHeight) : NoWayMesh, mesh, Matrix.RotationY(-(float)Math.PI * 0.5f));
            appendMeshTo(hasRight ? GetHeightWayMesh(rightHeight) : NoWayMesh, mesh, Matrix.RotationY((float)Math.PI));
            appendMeshTo(hasBottom ? GetHeightWayMesh(bottomHeight) : NoWayMesh, mesh, Matrix.RotationY((float)Math.PI * 0.5f));

            cachedMeshes.Add(cacheString, mesh);

            return mesh;
        }

        private IMesh GetHeightWayMesh(int height)
        {
            if (height == 0)
                return WayMesh;
            if (HeightWayMeshes == null || HeightWayMeshes.Count < height || height < 0)
                return new RAMMesh();

            return HeightWayMeshes[height - 1];
        }

        private void appendMeshTo(IMesh source, IMesh destination, Matrix transform)
        {
            if (source == null)
                return;
            MeshBuilder.AppendMeshTo(source, destination, transform);
        }

        private string getCacheString(bool hasLeft, int leftHeight, bool hasTop, int topHeight, bool hasRight, int rightHeight, bool hasBottom, int bottomHeight)
        {
            var ret = "";
            ret += hasLeft ? "1" : "0";
            ret += leftHeight;
            ret += hasTop ? "1" : "0";
            ret += topHeight;
            ret += hasRight ? "1" : "0";
            ret += rightHeight;
            ret += hasBottom ? "1" : "0";
            ret += bottomHeight;
            return ret;
        }
    }
}