using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Model
{
    /// <summary>
    /// Contains the legacy voxel data from before using the extension system
    /// </summary>
    public interface IBasicVoxelDataExtension : IVoxelDataExtension
    {
        IGameVoxelType Type { get; set; }
        int DataValue { get; set; }
        int MagicLevel { get; set; }
        float Height { get; set; }
        int WorkerCount { get; set; }

        /// <summary>
        /// TODO: could be optimized to not always store an inventory
        /// </summary>
        Inventory Inventory { get; set; }

        RoadType.RoadData Road { get; set; }

        InfestationVoxelType.InfestationData Infestation { get; set; }

    }
}