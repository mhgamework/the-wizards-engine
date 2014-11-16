﻿using System;
using System.Collections.Generic;
using Autofac;
using MHGameWork.TheWizards.GodGame.Model;
using MHGameWork.TheWizards.GodGame.ToolSelection;
using MHGameWork.TheWizards.GodGame.Types;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Responsible for constructing the tools menu!
    /// </summary>
    public class ToolMenuBuilder
    {
        private VoxelTypesFactory typesFactory;
        private Internal.Model.World world;
        private readonly Func<string,ToolSelectionCategory> createCategory;
        private readonly ToolSelectionTool.Factory createToolItem;

        public ToolMenuBuilder(VoxelTypesFactory typesFactory, Internal.Model.World world, Func<string,ToolSelectionCategory> createCategory, ToolSelectionTool.Factory createToolItem )
        {
            this.typesFactory = typesFactory;
            this.world = world;
            this.createCategory = createCategory;
            this.createToolItem = createToolItem;
        }

        public List<IToolSelectionItem> BuildMenu()
        {
            var ret = new List<IToolSelectionItem>();

            ret.Add(createLandCategory());

            return ret;
        }

        private IToolSelectionItem createLandCategory()
        {
            var ret = createCategory("Land");
            ret.SelectionItems.AddRange(createLandInputs().Select(toToolItem));
            return ret;
        }

        private ToolSelectionTool toToolItem(IPlayerTool arg)
        {
            return createToolItem(arg, arg.Name);
        }


        private IEnumerable<IPlayerTool> createLandInputs()
        {
            yield return new CreateLandTool(world, typesFactory.Get<AirType>(), typesFactory.Get<LandType>());
            yield return new ChangeHeightTool(world);
            yield return createTypeInput(typesFactory.Get<ForestType>());
            yield return createTypeInput(typesFactory.Get<InfestationVoxelType>());
            yield return createTypeInput(typesFactory.Get<WaterType>());
            yield return createTypeInput(typesFactory.Get<HoleType>());
            yield return createOreInput();
        }

        private IEnumerable<IPlayerTool> createOtherInputs()
        {
            yield return createTypeInput(typesFactory.Get<VillageType>());
            yield return createTypeInput(typesFactory.Get<WarehouseType>());
            yield return createTypeInput(typesFactory.Get<MonumentType>());
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