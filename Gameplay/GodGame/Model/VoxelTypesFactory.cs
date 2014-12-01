using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Model
{
    /// <summary>
    /// Responsible for providing access to the repository of voxel rawTypes
    /// </summary>
    public class VoxelTypesFactory
    {
        private readonly IEnumerable<IGameVoxelType> rawTypes;
        private readonly ItemTypesFactory itemTypes;
        private Dictionary<IGameVoxelType, BuildingSiteType> buildingSites = new Dictionary<IGameVoxelType, BuildingSiteType>();

        public VoxelTypesFactory(IEnumerable<IGameVoxelType> rawTypes, ItemTypesFactory itemTypes)
        {
            this.rawTypes = rawTypes;
            this.itemTypes = itemTypes;
            createBuildingSite<HouseType>(new[] { new BuildingSiteType.ItemAmount { Type = itemTypes.WoodType, Amount = 5 } });
            createBuildingSite<WoodworkerType>(new[] { new BuildingSiteType.ItemAmount { Type = itemTypes.WoodType, Amount = 20 } });

            createBuildingSite<MinerType>(new[] { new BuildingSiteType.ItemAmount { Type = itemTypes.WoodType, Amount = 20 } });

            //TODO: worshipsite


            createBuildingSite<TownCenterType>(new[] { new BuildingSiteType.ItemAmount { Type = itemTypes.WoodType, Amount = 20 } });

            createBuildingSite<FisheryType>(new[] { new BuildingSiteType.ItemAmount { Type = itemTypes.WoodType, Amount = 10 } });
            createBuildingSite<MarketType>(new[] { new BuildingSiteType.ItemAmount { Type = itemTypes.FishType, Amount = 20 } });
        }

        private BuildingSiteType createBuildingSite<T>(IEnumerable<BuildingSiteType.ItemAmount> amounts)
            where T : GameVoxelType
        {
            return createBuildingSite(amounts, Get<T>());
        }
        private BuildingSiteType createBuildingSite(IEnumerable<BuildingSiteType.ItemAmount> amounts, GameVoxelType typeToBuild)
        {
            if (buildingSites.ContainsKey(typeToBuild)) throw new InvalidOperationException("Already added a building site for type " + typeToBuild.Name);
            var ret = new BuildingSiteType(typeToBuild, amounts.ToList(), typeToBuild.Name, itemTypes);
            buildingSites[typeToBuild] = ret;
            ret.InjectVoxelTypesFactory(this);
            return ret;
        }



        public T Get<T>() where T : GameVoxelType
        {
            return (T)rawTypes.First(t => t.GetType() == typeof(T));
        }
        public BuildingSiteType GetBuildingSite<T>() where T : GameVoxelType
        {
            return GetBuildingSite(Get<T>());
        }
        public BuildingSiteType GetBuildingSite(GameVoxelType typeToBuild)
        {
            return buildingSites[typeToBuild];
        }

        public IEnumerable<IGameVoxelType> AllTypes { get { return rawTypes.Union(buildingSites.Values); } }
    }
}