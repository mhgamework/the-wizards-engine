using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.DualContouring.Rendering;
using MHGameWork.TheWizards.DualContouring.Terrain;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.VoxelEngine.Worlding
{
    /// <summary>
    /// Provides access and storage to all world data for a given node from the world's octree
    /// </summary>
    public class WorldNode : IOctreeNode<WorldNode>
    {

        public void Initialize(WorldNode parent)
        {

        }
        public void Destroy()
        {
        }

        /// <summary>
        /// Stores the RenderElement, when loaded in renderer
        /// </summary>
        public VoxelSurface VoxelSurface { get; set; }
        /// <summary>
        /// Stores the surface mesh for this node, if calculated
        /// </summary>
        public RawMeshData SurfaceMesh { get; set; }
        /// <summary>
        /// Stores the voxel data for this node, if calculated
        /// </summary>
        public HermiteDataGrid VoxelData { get; set; }
        /// <summary>
        /// Stores a voxel data generator, which can be evaluated to calculate voxel data for this node and or children
        /// </summary>
        public AbstractHermiteGrid VoxelDataGenerator { get; set; }




        #region OctreeNode

        public WorldNode[] Children { get; set; }
        public Point3 LowerLeft { get; set; }
        public int Size { get; set; }
        public int Depth { get; set; }

        #endregion
    }
}