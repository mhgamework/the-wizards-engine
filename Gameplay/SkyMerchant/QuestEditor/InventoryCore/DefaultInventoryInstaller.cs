﻿using System;
using System.IO;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MHGameWork.TheWizards.SkyMerchant.Gameplay;
using MHGameWork.TheWizards.SkyMerchant.Gameplay.Inventory;
using MHGameWork.TheWizards.SkyMerchant.Gameplay.Items;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.Scripting;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore
{
    /// <summary>
    /// Responsible for setting up the default inventory, as it should be used in the actual game.
    /// </summary>
    public class DefaultInventoryInstaller : IWindsorInstaller
    {
        private IInventoryNode createScripts(IKernel input)
        {
            return new ScriptsInventoryNode("MHGameWork.TheWizards.SkyMerchant",
                                            delegate(Type t)
                                            {
                                                var repo = input.Resolve<ScriptsRepository>();
                                                return input.Resolve<ScriptToolItem>(new Arguments().InsertTyped(repo.GetScriptType(t)));
                                            });
        }

        private IInventoryNode createMeshSpawners(IKernel input)
        {
            return new MeshSpawnersInventoryNode(new DirectoryInfo(TWDir.GameData + "\\SkyMerchant"), input.Resolve<IMeshSpawnerItemFactory>());
        }

        private IInventoryNode createTools(IKernel input)
        {
            var ret = new GroupInventoryNode();

            ret.AddChild(new HotBarItemInventoryNode(input.Resolve<IslandToolItem>()));
            ret.AddChild(new HotBarItemInventoryNode(input.Resolve<BridgeToolItem>()));

            return ret;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IInventoryNode>().UsingFactoryMethod(createInventory));
            container.Register(Component.For<IInventoryNodeRenderer>().UsingFactoryMethod(createInventoryRenderer));
        }

        private MeshSpawnerInventoryRenderer createInventoryRenderer(IKernel input)
        {
            return new MeshSpawnerInventoryRenderer(new HotBarItemTextInventoryRenderer(new WireframeInventoryNodeRenderer()));
        }

        private IInventoryNode createInventory(IKernel input)
        {
            var root = input.Resolve<GroupInventoryNode>();
            root.AddChild(createTools(input));
            root.AddChild(createMeshSpawners(input));
            root.AddChild(createScripts(input));

            return root;
        }
    }
}