using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant.Voxels;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Building._SkyMerchant
{
    /// <summary>
    /// World related island aspects
    /// </summary>
    public interface IIsland
    {
        /// <summary>
        /// Indicates that build mode is enabled on the island. (Items should not fall)
        /// </summary>
        void EnterBuildMode();
        /// <summary>
        /// Indicates that the item is free again
        /// </summary>
        void ExitBuildMode();

        /// <summary>
        /// Returns true when there is an island ground voxel at given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool HasBlockAt(Vector3 pos);
    }

    [ModelObjectChanged]
    public class SimpleIsland : EngineModelObject, IIsland
    {
        private IFiniteVoxels voxels;

        public SimpleIsland(IslandPart part, IslandMeshFactory factory)
        {
            voxels = factory.CreateVoxels(part.Seed);
        }

        public bool BuildMode { get; private set; }

        public void EnterBuildMode()
        {
            BuildMode = true;
        }

        public void ExitBuildMode()
        {
            BuildMode = false;
        }

        public bool HasBlockAt(Vector3 pos)
        {
            return voxels.GetVoxel(pos.ToPoint3Rounded()) != null;
        }
    }
}