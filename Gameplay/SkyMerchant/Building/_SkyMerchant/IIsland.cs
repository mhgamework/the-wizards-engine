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
}