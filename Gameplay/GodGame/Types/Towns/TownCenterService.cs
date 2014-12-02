using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Towns.Data;
using MHGameWork.TheWizards.GodGame.Types.Towns.Workers;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    public class TownCenterService
    {
        private IList<Town> townCenters;
        private Lazy<HouseType> houseType;
        private readonly GenericDatastoreRecord datastoreRecord;

        public TownCenterService(Lazy<HouseType> houseType, GenericDatastoreRecord store)
        {
            this.houseType = houseType;
            this.datastoreRecord = store;
            townCenters = datastoreRecord.GetList<Town>("TownCenterService-Towns", r => new Town(this, r));

        }

        public Town CreateTown(IVoxel center)
        {
            var ret = new Town(this, datastoreRecord.CreateRecord());
            townCenters.Add(ret);
            ret.AddVoxel(center);
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