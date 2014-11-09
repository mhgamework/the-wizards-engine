using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    public class TownCenterService
    {
        private List<Town> townCenters = new List<Town>();


        public Town CreateTown(IVoxel center)
        {
            var ret = new Town(this);
            townCenters.Add(ret);
            ret.TownVoxels.Add(center);
            return ret;
        }


        public HashSet<House> GetHouses(Town town)
        {
            throw new NotImplementedException();
        }

        public Town GetTownForVoxel(IVoxel voxel)
        {
            return townCenters.FirstOrDefault(t => t.TownVoxels.Contains(voxel));
        }
    }
}