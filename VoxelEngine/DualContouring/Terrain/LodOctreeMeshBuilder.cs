using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred.Meshes;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring.Terrain
{
    public class LodOctreeMeshBuilder
    {
        private Color4[] colors = QuadTreeVisualizer.levelColor4s;

        private Dictionary<NodeIdentifier, HermiteDataGrid> cachedGrids =
            new Dictionary<NodeIdentifier, HermiteDataGrid>();

        public DualContouringMeshBuilder meshBuilder = new DualContouringMeshBuilder();
        public DeferredMeshElement CreateRenderElementForNode(LodOctreeNode node, int minNodeSize, IMesh mesh)
        {
            var el = TW.Graphics.AcquireRenderer().CreateMeshElement(mesh);
            float setApart = 1.1f;
            setApart = 1; // Disable spacing between cells
            el.WorldMatrix = Matrix.Scaling(new Vector3(node.size / minNodeSize)) *
                             Matrix.Translation(node.LowerLeft.ToVector3() * setApart);
            return el;
        }

        public IMesh CalculateNodeMesh(LodOctreeNode node, int minNodeSize, Func<Vector3, float> density)
        {
            var grid = getGrid(node, minNodeSize, density);

            var mesh = meshBuilder.buildMesh(grid);
            if (mesh.GetCoreData().Parts.Count > 0)
                mesh.GetCoreData().Parts[0].MeshMaterial = new MeshCoreData.Material() { ColoredMaterial = true, DiffuseColor = colors[node.depth % colors.Length].xna() };
            return mesh;
        }

        private HermiteDataGrid getGrid(LodOctreeNode node, int minNodeSize, Func<Vector3, float> density)
        {
            var nId = new NodeIdentifier(node);
            if (cachedGrids.ContainsKey(nId)) return cachedGrids[nId];
            var currScaling = node.size / minNodeSize;

            // Then we add another +1 to be able to connect the gaps between the hermite grids
            //TODO: do lod stitching here
            var gridSize = minNodeSize + 1;


            var grid = HermiteDataGrid.CopyGrid(
                new DensityFunctionHermiteGrid(v => density(v * currScaling + (Vector3)node.LowerLeft.ToVector3()),
                                               new Point3(gridSize, gridSize, gridSize)));
            cachedGrids[nId] = grid;
            return grid;
        }

        public void ListMeshLessNodes(LodOctreeNode n, List<LodOctreeNode> list)
        {
            if (n.Mesh == null) list.Add(n);

            if (n.Children == null) return;
            for (int i = 0; i < 8; i++)
            {
                ListMeshLessNodes(n.Children[i], list);
            }
        }

        private struct NodeIdentifier
        {
            private Point3 Pos;
            private int Size;

            public NodeIdentifier(LodOctreeNode node)
            {
                this.Pos = node.LowerLeft;
                this.Size = node.size;
            }

            public NodeIdentifier(Point3 pos, int size)
            {
                Pos = pos;
                Size = size;
            }
        }
    }
}