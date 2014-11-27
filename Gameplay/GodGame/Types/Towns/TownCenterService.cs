using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Towns.Workers;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    public class TownCenterService
    {
        private List<Town> townCenters = new List<Town>();
        private Lazy<HouseType> houseType;

        public TownCenterService(Lazy<HouseType> houseType)
        {
            this.houseType = houseType;
        }

        public Town CreateTown(IVoxel center)
        {
            var ret = new Town(this);
            townCenters.Add(ret);
            ret.TownVoxels.Add(center);
            return ret;
        }
        public void DestroyTown(Town town)
        {
            townCenters.Remove(town);
        }


        public Town GetTownForVoxel(IVoxel voxel)
        {
            return townCenters.FirstOrDefault(t => t.TownVoxels.Contains(voxel));
        }

        public IEnumerable<IWorkerProducer> GetProducers(Town town)
        {
            return town.TownVoxels.Where(v => v.Data.Type == houseType.Value).Select(v => houseType.Value.GetWorkerProducer((IVoxelHandle)v));
        }

        public IEnumerable<IWorkerConsumer> GetConsumers(Town town)
        {
            return town.TownVoxels.Where(v => v.Data.Type is IIndustryBuildingType).Select(v => ((IIndustryBuildingType)v.Data.Type).GetWorkerConsumer((IVoxelHandle)v));
        }
    }
}