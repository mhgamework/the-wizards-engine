using System;
using System.IO;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    /// <summary>
    /// Responsible for setting up the default inventory, as it should be used in the actual game.
    /// </summary>
    public class DefaultInventoryBuilder
    {
        private Random random;
        private ObjectsFactory objectsFactory;

        public DefaultInventoryBuilder(Random random, ObjectsFactory objectsFactory)
        {
            this.random = random;
            this.objectsFactory = objectsFactory;
        }

        public IInventoryNode CreateTree()
         {
             var root = new GroupInventoryNode();
             root.AddChild(createTools());
             root.AddChild(createMeshSpawners());
             root.AddChild(createScripts());

             return root;
         }

        private IInventoryNode createScripts()
        {
            //TODO:
            return new GroupInventoryNode();
        }

        private IInventoryNode createMeshSpawners()
        {
            return new MeshSpawnersInventoryNode(new DirectoryInfo(TWDir.GameData + "\\SkyMerchant"));
        }

        private IInventoryNode createTools()
        {
            var ret = new GroupInventoryNode();

            ret.AddChild(new HotBarItemInventoryNode(new IslandToolItem(objectsFactory,random)));
            ret.AddChild(new HotBarItemInventoryNode(new BridgeToolItem()));

            return ret;
        }
    }
}