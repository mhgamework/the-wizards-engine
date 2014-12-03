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
        private IList<Town> towns;
        private Lazy<HouseType> houseType;
        private readonly GenericDatastoreRecord datastoreRecord;

        public TownCenterService(Lazy<HouseType> houseType, GenericDatastoreRecord store)
        {
            this.houseType = houseType;
            this.datastoreRecord = store;
            towns = datastoreRecord.GetList<Town>("TownCenterService-Towns", r => new Town(this, r));

        }

        public IEnumerable<Town> Towns { get { return towns; } }

        /// <summary>
        /// Creates a new town for given towncenter
        /// Returns existing town if already found
        /// </summary>
        public Town CreateTown(IVoxel center)
        {
            var existing = towns.FirstOrDefault(t => t.TownCenter == center);
            if (existing != null) return existing;
            var ret = new Town(this, datastoreRecord.CreateRecord());
            towns.Add(ret);
            ret.SetTownCenter(center);
            return ret;

        }
        public void DestroyTown(Town town)
        {
            towns.Remove(town);
        }


        public Town GetTownForVoxel(IVoxel voxel)
        {
            return towns.FirstOrDefault(t => t.TownVoxels.Contains(voxel));
        }

        public IEnumerable<IWorkerProducer> GetProducers(Town town)
        {
            return town.TownVoxels.Where(v => v.Data.Type == houseType.Value).Select(v => houseType.Value.GetWorkerProducer((IVoxelHandle)v));
        }

        public IEnumerable<IWorkerConsumer> GetConsumers(Town town)
        {
            return town.TownVoxels.Cast<IVoxelHandle>().Where(v => v.Data.Type.HasAddon<IndustryBuildingAddon>(v))
                .Select(v => v.Type.GetAddon<IndustryBuildingAddon>(v));
        }
    }
}