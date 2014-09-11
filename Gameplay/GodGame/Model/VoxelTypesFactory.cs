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
        private readonly IEnumerable<GameVoxelType> rawTypes;
        private readonly ItemTypesFactory itemTypes;
        private Dictionary<GameVoxelType, BuildingSiteType> buildingSites = new Dictionary<GameVoxelType, BuildingSiteType>();

        public VoxelTypesFactory(IEnumerable<GameVoxelType> rawTypes, ItemTypesFactory itemTypes)
        {
            this.rawTypes = rawTypes;
            this.itemTypes = itemTypes;
            CreateBuildingSite<FisheryType>(new[] { new BuildingSiteType.ItemAmount { Type = itemTypes.CropType, Amount = 10 } });
            CreateBuildingSite<MarketType>(new[] { new BuildingSiteType.ItemAmount { Type = itemTypes.CropType, Amount = 10 } });
        }

        private BuildingSiteType CreateBuildingSite<T>(IEnumerable<BuildingSiteType.ItemAmount> amounts) where T : GameVoxelType
        {
            if (buildingSites.ContainsKey(Get<T>())) throw new InvalidOperationException("Already added a building site for type " + typeof(T).Name);
            var ret = new BuildingSiteType(Get<T>(), amounts.ToList(), itemTypes);
            buildingSites[Get<T>()] = ret;
            return ret;
        }



        public T Get<T>() where T : GameVoxelType
        {
            return (T)rawTypes.First(t => t.GetType() == typeof(T));
        }
        public BuildingSiteType GetBuildingSite<T>() where T : GameVoxelType
        {
            return buildingSites[Get<T>()];
        }

        public IEnumerable<GameVoxelType> AllTypes { get { return rawTypes.Union(buildingSites.Values); } }
    }
}