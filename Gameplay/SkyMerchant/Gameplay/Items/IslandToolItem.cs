using System;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay.Items
{
    /// <summary>
    /// Allows placing and randomizing islands
    /// Note that a randomized island randomizes island structure (voxels), as well as island decorations (trees, rocks)
    /// </summary>
    public class IslandToolItem : IHotbarItem
    {
        private readonly WorldPlacerHelper placer;
        private readonly IWorld world;
        private readonly Random random;

        public IslandToolItem(IWorld world, Random random)
        {
            this.world = world;
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

        private IPositionComponent createNewIsland()
        {
            var island = world.CreateIsland(random.Next());
            return island;
        }

    }
}