using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Terrain;

namespace MHGameWork.TheWizards.VoxelEngine.Worlding
{
    /// <summary>
    /// Creates / new worldnodes for the octree (see this as a virtual nodes to real nodes factory). 
    /// In reality, the octree can be seen as a completely divided virtual tree, where the nodes are the in-memory parts of the virtual tree.
    /// This factory constructs the real nodes so that they correctly reference the data of the virtual nodes (implicitly implemented virtual nodes)
    /// 
    /// Currently, this is done by selecting the correct part of the VoxelGridGenerator from the parent node
    /// </summary>
    public class LargeWorldNodeFactory : IOctreeNodeFactory<WorldNode>
    {
        public void Destroy(WorldNode node)
        {
            if (node.VoxelSurface != null) node.VoxelSurface.Delete();
        }

        public WorldNode Create(WorldNode parent, int size, int depth, Point3 pos)
        {
            var ret = new WorldNode() { Size = size, Depth = depth, LowerLeft = pos };

            // Hacktime! only works for density functions
            if (parent != null)
                ret.VoxelDataGenerator = parent.VoxelDataGenerator;

            /* if (parent != null)
             {
                 var g = (DensityFunctionHermiteGrid)parent.VoxelDataGenerator;
                 var offset = pos - parent.LowerLeft;
                 var nGrid = new DensityFunctionHermiteGrid(v => g.DensityFunction((v - (Vector3)offset.ToVector3())), new Point3(size));
             }*/

            // This sadly does not work, since a fix needs to be found to subsample normals
            /*AbstractHermiteGrid parentGrid = null;
            if (parent != null) parentGrid = parent.VoxelData ?? parent.VoxelDataGenerator;

            if (parentGrid != null)
                ret.VoxelDataGenerator = new SubGrid(parentGrid, pos - parent.LowerLeft, new Point3(size, size, size), 2);*/

            return ret;
        }
    }
}