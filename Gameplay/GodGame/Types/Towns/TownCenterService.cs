using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    public class TownCenterService
    {
        private List<Town> townCenters = new List<Town>();


        public Town CreateTown(GameVoxel center)
        {
            var ret = new Town(this);
            townCenters.Add(ret);
            ret.TownVoxels.Add(center);
            return ret;
        }


        /// <summary>
        /// Adds given voxel to the town border, and takes it from its current town if one exists
        /// </summary>
        public void AssignVoxelToTown(Town town, GameVoxel voxel)
        {
            if (!town.IsAtBorder(voxel)) throw new InvalidOperationException("Voxel is not at the border of the town!");

            var oldTown = getTownForVoxel(voxel);
            if (oldTown != null) oldTown.TownVoxels.Remove(voxel);
            town.TownVoxels.Add(voxel);

        }

        private Town getTownForVoxel(GameVoxel voxel)
        {
            return townCenters.FirstOrDefault(t => t.TownVoxels.Contains(voxel));
        }

        public HashSet<House> GetHouses(Town town)
        {
            throw new NotImplementedException();
        }

        public HashSet<WorkerClient> WorkerClients(Town town)
        {
            throw new NotImplementedException();
        }
    }
}