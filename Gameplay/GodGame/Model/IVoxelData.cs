using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame._Tests;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Provides the data storage for a single voxel
    /// TODO: Voxel data is actually not defined by some stupid gameplay fields
    ///     It is defined by the fact that it is observable data, and that it is network and disk serializable.
    ///     The actual gameplay fields are not part of the engine subsystem, but of the gameplay system
    /// </summary>
    public interface IVoxelData
    {
        IGameVoxelType Type { get; set; }
        int DataValue { get; set; }
        int MagicLevel { get; set; }
        float Height { get; set; }
        int WorkerCount { get; set; }

        /// <summary>
        /// TODO: could be optimized to not always store an inventory
        /// </summary>
        Inventory Inventory { get; }

        RoadType.RoadData Road { get; }

        InfestationVoxelType.InfestationData Infestation { get; set; }

        T Get<T>() where T : IVoxelDataExtension;
    }
}