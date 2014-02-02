using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.Scattered.Simulation.Constructions
{
    public class ScrapStationAction : IConstructionAction
    {
        private readonly Island island;
        private float nextResourceGen;
        private Level level;
        private Traveller cart;
        private DistributionHelper distributionHelper;
        private const float Interval = 3;

        public ScrapStationAction(Island island, DistributionHelper distributionHelper)
        {
            this.island = island;
            this.distributionHelper = distributionHelper;
            this.level = island.Level;
            nextResourceGen = TW.Graphics.TotalRunTime + Interval;
        }

        public void Update()
        {
            if (cart != null && cart.Destination == null) cart = null; // Cleanup removed cart

            if (cart == null && island.Inventory.GetAmountOfType(level.ScrapType) > 0)
            {
                cart = distributionHelper.CreateSingleTarget(island, isWarehouse);

                if (cart != null) cart.Inventory.TakeAll(island.Inventory);
            }

            if (island.Inventory.GetAmountOfType(level.ScrapType) >= 4) return;
            if (nextResourceGen > TW.Graphics.TotalRunTime) return;

            nextResourceGen = TW.Graphics.TotalRunTime + Interval;
            island.Inventory.AddNewItems(level.ScrapType, 1);
        }


        private bool isWarehouse(Island island1)
        {
            return island1.Construction.Name == "Warehouse"; //TODO
        }
    }
}