using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame._Tests;
using MHGameWork.TheWizards.Scattered.Model;
using IInterceptor = Castle.Core.Interceptor.IInterceptor;
using IInvocation = Castle.Core.Interceptor.IInvocation;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Provides the data storage for a single voxel
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