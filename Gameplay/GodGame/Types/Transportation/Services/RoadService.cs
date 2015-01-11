using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Scattered.Model;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame.Types.Transportation.Services
{
    public static class RoadService
    {
        /// <summary>
        /// Can push from this voxel to a surrounding road voxel
        /// </summary>
        public static bool CanPush(IVoxel voxel, ItemType item)
        {
            //TODO: this algorithm is shitty
            return GetRoads(voxel).Any(v => v.Data.Type.CanAcceptItemType((IVoxelHandle)v, item));

        }

        public static void Push(IVoxel voxel, ItemType item)
        {
            if (!CanPush(voxel, item)) throw new InvalidOperationException();

            voxel.Data.Inventory.AddNewItems(item, 1);

            var roads = GetRoads(voxel).Where(v => v.Data.Type.CanAcceptItemType((IVoxelHandle)v, item));
            ((RoadType)roads.First().Data.Type).DeliverItemClosest((IVoxelHandle)roads.First(), (IVoxelHandle)voxel, item);
        }

        public static IEnumerable<IVoxel> GetRoads(IVoxel voxel)
        {
            return voxel.Get4Connected().Where(v => v.Data.Type is RoadType);
        }
    }
}