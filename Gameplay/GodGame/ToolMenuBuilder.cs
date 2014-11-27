using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.Indexed;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.ToolSelection;
using MHGameWork.TheWizards.GodGame.Types;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Types.Towns;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Responsible for constructing the tools menu!
    /// </summary>
    public class ToolMenuBuilder
    {
        private VoxelTypesFactory typesFactory;
        private Internal.Model.World world;
        private readonly Func<string, ToolSelectionCategory> createCategory;
        private readonly ToolSelectionTool.Factory createToolItem;
        private readonly IIndex<Type, PlayerTool> getPlayerTool;

        public ToolMenuBuilder(VoxelTypesFactory typesFactory, Internal.Model.World world, Func<string, ToolSelectionCategory> createCategory, ToolSelectionTool.Factory createToolItem,
            IIndex<Type, PlayerTool> getPlayerTool)
        {
            this.typesFactory = typesFactory;
            this.world = world;
            this.createCategory = createCategory;
            this.createToolItem = createToolItem;
            this.getPlayerTool = getPlayerTool;
        }

        public List<IToolSelectionItem> BuildMenu()
        {
            var ret = new List<IToolSelectionItem>();

            ret.Add(createCategory("Terrain").Alter(c => c.SelectionItems.AddRange(createTerrainInputs().Select(toToolItem))));
            ret.Add(createCategory("Buildings").Alter(c =>
                {
                    c.SelectionItems.Add(createTool(typesFactory.Get<RoadType>()));
                    c.SelectionItems.Add(createCat("Industry", createBuildingIndustryInputs()));
                    c.SelectionItems.Add(createCat("Village", createBuildingVillageInputs()));
                }));
            ret.Add(createCategory("BuildingSites").Alter(c => c.SelectionItems.AddRange(createBuildingSiteInputs().Select(toToolItem))));
            ret.Add(createCategory("Godpowers").Alter(c => c.SelectionItems.AddRange(createGodpowerInputs().Select(toToolItem))));

            return ret;
        }

        private ToolSelectionTool toToolItem(PlayerTool arg)
        {
            return createToolItem(arg, arg.Name);
        }

        private ToolSelectionCategory createCat(string name, IEnumerable<PlayerTool> tools)
        {
            return createCategory(name).Alter(k => k.SelectionItems.AddRange(tools.Select(toToolItem)));

        }


        private IEnumerable<PlayerTool> createTerrainInputs()
        {
            yield return new CreateLandTool(world, typesFactory.Get<AirType>(), typesFactory.Get<LandType>());
            yield return new ChangeHeightTool(world);
            yield return createTypeInput(typesFactory.Get<ForestType>());
            yield return createTypeInput(typesFactory.Get<InfestationVoxelType>());
            yield return createTypeInput(typesFactory.Get<WaterType>());
            yield return createTypeInput(typesFactory.Get<HoleType>());
            yield return createOreInput();
            yield return createTypeInput(typesFactory.Get<MonumentType>());
        }

        private IEnumerable<PlayerTool> createBuildingIndustryInputs()
        {
            yield return createTypeInput(typesFactory.Get<WarehouseType>());
            yield return createTypeInput(typesFactory.Get<MinerType>());
            yield return createTypeInput(typesFactory.Get<QuarryType>());
            yield return createTypeInput(typesFactory.Get<GrinderType>());
            yield return createTypeInput(typesFactory.Get<CropType>());
            yield return createTypeInput(typesFactory.Get<FarmType>());
            yield return createTypeInput(typesFactory.Get<FisheryType>());
            yield return createTypeInput(typesFactory.Get<WoodworkerType>());

        }
        private IEnumerable<PlayerTool> createBuildingVillageInputs()
        {
            yield return createTypeInput(typesFactory.Get<TownCenterType>());
            yield return createTypeInput(typesFactory.Get<HouseType>());
            yield return createTypeInput(typesFactory.Get<MarketType>());
            yield return getPlayerTool[typeof(TownBorderTool)];
        }

        private IEnumerable<PlayerTool> createBuildingSiteInputs()
        {
            yield return createTypeInput(typesFactory.GetBuildingSite<MarketType>(), "MarketBuildSite");
            yield return createTypeInput(typesFactory.GetBuildingSite<FisheryType>(), "FisheryBuildSite");

        }

        private IEnumerable<PlayerTool> createGodpowerInputs()
        {
            yield return new LightGodPowerTool(typesFactory.Get<InfestationVoxelType>());
        }


        private PlayerTool createTypeInput(GameVoxelType type, string name)
        {
            var ret = new DelegatePlayerTool(name,
                                          v => v.ChangeType(typesFactory.Get<LandType>()),
                                          v =>
                                          {
                                              if (v.Type == typesFactory.Get<LandType>())
                                                  v.ChangeType(type);
                                          });
            ret.onTargetChanged = (player, v, key, mouse) =>
                {
                    if (mouse.LeftMousePressed)
                        ret.onLeftClick(v);
                    else if (mouse.RightMousePressed)
                        ret.onRightClick(v);
                };
            return ret;
        }
        private ToolSelectionTool createTool(GameVoxelType type)
        {
            return toToolItem(createTypeInput(type));
        }
        private PlayerTool createTypeInput(GameVoxelType type)
        {
            return createTypeInput(type, type.Name);
        }
        private PlayerTool createOreInput()
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