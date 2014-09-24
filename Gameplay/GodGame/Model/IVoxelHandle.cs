using System;
using System.Collections.Generic;
using DirectX11;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.GodGame.Internal.Model
{
    public interface IVoxelHandle
    {
        float TickLength { get; }
        float TotalTime { get; }
        IVoxelData Data { get; }
        Seeder Seeder { get; }
        GameVoxelType Type { get; }

        /// <summary>
        /// DO NOT USE THIS IN GAMEPLAY LAYER!
        /// Simply exists because some design issues are to time consuming and not relevant enough to solve.
        /// </summary>
        /// <returns></returns>
        GameVoxel GetInternalVoxel();

        IEnumerable<IVoxelHandle> Get8Connected();

        /// <summary>
        /// Return order: (1,0) (0,1) (-1,0) (0,-1)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IVoxelHandle> Get4Connected();

        IEnumerable<IVoxelHandle> GetRange(int radius);
        IEnumerable<IVoxelHandle> GetRangeCircle(int radius);
        IVoxelHandle GetRelative(Point2 p);
        Point2 GetOffset(IVoxel other);

        /// <summary>
        /// When called ensures that on average each ('averageInterval' seconds) the function returns true,
        /// as long as the elapsed value is small enough compared to the average interval 
        /// (otherwise multiple events could occur in a single interval)
        /// Uses Poisson distribution for 1 or more events inside a single interval
        /// TODO: only executes action once, could execute multiple times to be more correct 
        /// </summary>
        /// <returns></returns>
        void EachRandomInterval(float averageInterval, Action action);

        void ChangeType(GameVoxelType type);
        bool CanAcceptItemType(ItemType type);
        bool CanAcceptItemType(IVoxelHandle deliveryHandle, ItemType type);
        bool CanAddWorker();
        float DistanceTo(IVoxelHandle handle);
    }

}