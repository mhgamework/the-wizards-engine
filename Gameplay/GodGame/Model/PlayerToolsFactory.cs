using System.Collections.Generic;
using MHGameWork.TheWizards.GodGame.Types;

namespace MHGameWork.TheWizards.GodGame.Model
{
    /// <summary>
    /// Responsible for creating all player tools
    /// </summary>
    public class PlayerToolsFactory
    {
        private readonly Internal.Model.World world;
        private readonly VoxelTypesFactory typesFactory;
        private List<IPlayerTool> tools = new List<IPlayerTool>();
        public IEnumerable<IPlayerTool> Tools { get { return tools; } }

        public PlayerToolsFactory(Internal.Model.World world, VoxelTypesFactory typesFactory)
        {
            this.world = world;
            this.typesFactory = typesFactory;
            tools.AddRange(createPlayerInputs());
        }


        private IEnumerable<IPlayerTool> createPlayerInputs()
        {
            yield return new CreateLandTool(world, typesFactory.Get<AirType>(), typesFactory.Get<LandType>());
            yield return new ChangeHeightTool(world);
            yield return createTypeInput(typesFactory.Get<ForestType>());
            yield return createTypeInput(typesFactory.Get<VillageType>());
            yield return createTypeInput(typesFactory.Get<WarehouseType>());
            yield return createTypeInput(typesFactory.Get<InfestationVoxelType>());
            yield return createTypeInput(typesFactory.Get<MonumentType>());
            yield return createTypeInput(typesFactory.Get<WaterType>());
            yield return createTypeInput(typesFactory.Get<HoleType>());
            yield return createOreInput();
            yield return createTypeInput(typesFactory.Get<MinerType>());
            yield return createTypeInput(typesFactory.Get<RoadType>());
            yield return createTypeInput(typesFactory.Get<CropType>());
            yield return createTypeInput(typesFactory.Get<FarmType>());
            yield return createTypeInput(typesFactory.Get<MarketType>());
            yield return createTypeInput(typesFactory.GetBuildingSite<MarketType>(), "MarketBuildSite");
            yield return createTypeInput(typesFactory.Get<FisheryType>());
            yield return createTypeInput(typesFactory.GetBuildingSite<FisheryType>(), "FisheryBuildSite");
            yield return createTypeInput(typesFactory.Get<WoodworkerType>());
            yield return createTypeInput(typesFactory.Get<QuarryType>());
            yield return createTypeInput(typesFactory.Get<GrinderType>());
            yield return new LightGodPowerTool(typesFactory.Get<InfestationVoxelType>());
        }

        private IPlayerTool createTypeInput(GameVoxelType type, string name)
        {
            return new DelegatePlayerTool(name,
                v => v.ChangeType(typesFactory.Get<LandType>()),
                v =>
                {
                    if (v.Type == typesFactory.Get<LandType>())
                        v.ChangeType(type);
                });
        }
        private IPlayerTool createTypeInput(GameVoxelType type)
        {
            return createTypeInput(type, type.Name);
        }
        private IPlayerTool createOreInput()
        {
            return new DelegatePlayerTool(typesFactory.Get<OreType>().Name,
                v => v.ChangeType(typesFactory.Get<LandType>()),
                v =>
                {
                    if (v.Type == typesFactory.Get<LandType>())
                    {
                        v.ChangeType(typesFactory.Get<OreType>());
                        v.Data.DataValue = 20;
                    }

                });
        }


    }


}