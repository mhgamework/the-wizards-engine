using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.Indexed;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.ToolSelection;
using MHGameWork.TheWizards.GodGame.Types;
using System.Linq;
using MHGameWork.TheWizards.GodGame.Types.Towns;
using MHGameWork.TheWizards.GodGame.Types.Transportation;
using MHGameWork.TheWizards.GodGame.Types.Transportation.Generic;
using MHGameWork.TheWizards.Scattered.Model;

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
        private readonly ItemTypesFactory itemTypesFactory;

        public ToolMenuBuilder(VoxelTypesFactory typesFactory,
            Internal.Model.World world,
            Func<string, ToolSelectionCategory> createCategory,
            ToolSelectionTool.Factory createToolItem,
            IIndex<Type, PlayerTool> getPlayerTool,
            ItemTypesFactory itemTypesFactory)
        {
            this.typesFactory = typesFactory;
            this.world = world;
            this.createCategory = createCategory;
            this.createToolItem = createToolItem;
            this.getPlayerTool = getPlayerTool;
            this.itemTypesFactory = itemTypesFactory;
        }

        public bool CheatmodeEnabled { get; set; }

        public List<IToolSelectionItem> BuildMenu()
        {

            var ret = new List<IToolSelectionItem>();

            ret.Add(createTransportation());

            ret.Add(getVoxelType<RoadType>());
            ret.Add(createCat("Towns",
                getBuildingSite<HouseType>(),
                getBuildingSite<MarketType>(),
                getBuildingSite<TownCenterType>(),
                getTool<TownBorderTool>()
                //getBuildingSite<WorshipSiteType>(),

                ));
            ret.Add(createCat("Industry",
              getBuildingSite<WoodworkerType>(),
              getBuildingSite<MinerType>()
                //getBuildingSite<WorshipSiteType>(),

              ));

            ret.Add(createCategory("Terrain").Alter(c => c.SelectionItems.AddRange(new[]
                {
                    createTypeInput(typesFactory.Get<AirType>()),
                    new ChangeHeightTool(world),
                    createTypeInput(typesFactory.Get<ForestType>()),
                    createTypeInput(typesFactory.Get<InfestationVoxelType>()),
                    createTypeInput(typesFactory.Get<WaterType>()),
                    createTypeInput(typesFactory.Get<HoleType>()),
                    createOreInput(),
                    createTypeInput(typesFactory.Get<MonumentType>())
                }.Select(toToolItem))));
            ret.Add(createCategory("Buildings").Alter(c =>
                {
                    c.SelectionItems.Add(createCat("Industry", new[]
                        {
                            createTypeInput(typesFactory.Get<WarehouseType>()),
                            createTypeInput(typesFactory.Get<MinerType>()),
                            createTypeInput(typesFactory.Get<QuarryType>()),
                            createTypeInput(typesFactory.Get<GrinderType>()),
                            createTypeInput(typesFactory.Get<CropType>()),
                            createTypeInput(typesFactory.Get<FarmType>()),
                            createTypeInput(typesFactory.Get<FisheryType>()),
                            createTypeInput(typesFactory.Get<WoodworkerType>())
                        }));
                    c.SelectionItems.Add(createCat("Village", new[]
                        {
                            createTypeInput(typesFactory.Get<TownCenterType>()),
                            createTypeInput(typesFactory.Get<HouseType>()),
                            createTypeInput(typesFactory.Get<MarketType>()),
                            getPlayerTool[typeof (TownBorderTool)]
                        }));
                }));
            ret.Add(createCategory("BuildingSites").Alter(c => c.SelectionItems.AddRange(new[]
                {
                    createTypeInput(typesFactory.GetBuildingSite<MarketType>(), "MarketBuildSite"),
                    createTypeInput(typesFactory.GetBuildingSite<FisheryType>(), "FisheryBuildSite")
                }.Select(toToolItem))));
            ret.Add(createCategory("Godpowers").Alter(c => c.SelectionItems.AddRange(new PlayerTool[] { new LightGodPowerTool(typesFactory.Get<InfestationVoxelType>()) }.Select(toToolItem))));

            return ret;
        }

        private ToolSelectionCategory createTransportation()
        {
            GenericVoxelType<BasicFactory> basicFactory = new GenericVoxelType<BasicFactory>(v => new BasicFactory(v));
            GenericVoxelType<ConstantFactory> constantFactory = new GenericVoxelType<ConstantFactory>(v => new ConstantFactory(v));
            GenericVoxelType<Puller> puller = new GenericVoxelType<Puller>(v => new Puller(v));
            GenericVoxelType<Pusher> pusher = new GenericVoxelType<Pusher>(v => new Pusher(v));


            return createCat("Transportation",
                             getVoxelType<RoadType>(),
                             getVoxelType<WarehouseType>(),
                             toToolItem(new DelegatePlayerTool("ConstantFood", v => v.ChangeType(typesFactory.Get<LandType>()), v
                                                                                                                                =>
                                 {
                                     v.ChangeType(constantFactory);
                                     var fact = ((IVoxel)v).GetPart<ConstantFactory>();
                                     fact.Rate = 1;
                                     fact.ItemsToGenerate = new[] { itemTypesFactory.CropType };

                                 })),
                             toToolItem(new DelegatePlayerTool("BasicFoodToPizza",
                                                    v => v.ChangeType(typesFactory.Get<LandType>()), v =>
                                                        {
                                                            v.ChangeType(basicFactory);
                                                            var fact = ((IVoxel)v).GetPart<BasicFactory>();
                                                            fact.EfficiencySpeedMultiplier = 1;
                                                            fact.Input = new[]
                                                                {
                                                                    itemTypesFactory.CropType, itemTypesFactory.CropType,
                                                                    itemTypesFactory.CropType
                                                                };
                                                            fact.Output = new[] {itemTypesFactory.PigmentType};
                                                        })),
                             toToolItem(createTypeInput(puller)),
                             toToolItem(createTypeInput(pusher))
                            

);


        }

        private ToolSelectionTool getTool<T>() where T : PlayerTool
        {
            return toToolItem(getPlayerTool[typeof(T)]);
        }
        private ToolSelectionTool getVoxelType<T>() where T : GameVoxelType
        {
            return toToolItem(createTypeInput(typesFactory.Get<T>()));
        }
        private ToolSelectionTool getBuildingSite<T>() where T : GameVoxelType
        {
            var type = typesFactory.Get<T>();
            return toToolItem(createBuildingSiteInput(type, "Build " + type.Name));
        }


        private ToolSelectionTool toToolItem(PlayerTool arg)
        {
            return createToolItem(arg, arg.Name);
        }

        private ToolSelectionCategory createCat(string name, params IToolSelectionItem[] items)
        {
            return createCategory(name).Alter(k => k.SelectionItems.AddRange(items));

        }
        private ToolSelectionCategory createCat(string name, params PlayerTool[] tools)
        {
            return createCat(name, (IEnumerable<PlayerTool>)tools);
        }
        private ToolSelectionCategory createCat(string name, IEnumerable<PlayerTool> tools)
        {
            return createCategory(name).Alter(k => k.SelectionItems.AddRange(tools.Select(toToolItem)));

        }

        private PlayerTool createBuildingSiteInput(GameVoxelType type, string name)
        {
            var buildingSiteType = typesFactory.GetBuildingSite(type);
            var ret = new DelegatePlayerTool(name,
                                          v => v.ChangeType(typesFactory.Get<LandType>()),
                                          v =>
                                          {
                                              if (v.Type != typesFactory.Get<LandType>()) return;
                                              if (CheatmodeEnabled)
                                                  v.ChangeType(type);
                                              else
                                                  v.ChangeType(buildingSiteType);
                                          });
            return ret;
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