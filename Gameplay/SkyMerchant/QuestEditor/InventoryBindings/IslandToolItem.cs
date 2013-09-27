using System;
using DirectX11;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    /// <summary>
    /// Allows placing and randomizing islands
    /// Note that a randomized island randomizes island structure (voxels), as well as island decorations (trees, rocks)
    /// </summary>
    public class IslandToolItem : IHotbarItem
    {
        private readonly WorldPlacerHelper placer;
        private readonly ObjectsFactory objectsFactory;
        private readonly Random random;

        public IslandToolItem(ObjectsFactory objectsFactory, Random random)
        {
            this.objectsFactory = objectsFactory;
            this.random = random;
            placer = new WorldPlacerHelper(createNewIsland);
        }

        public string Name { get { return "IslandTool"; } }
        public void OnSelected()
        {

        }

        public void OnDeselected()
        {
        }

        public void Update()
        {
            placer.Update();
        }

        private Physical createNewIsland()
        {
            var island = objectsFactory.CreateIsland();
            island.Seed = random.Next();
            return island.Physical;
        }

    }
}