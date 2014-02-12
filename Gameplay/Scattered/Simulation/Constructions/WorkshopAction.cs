using System;
using MHGameWork.TheWizards.Scattered.Model;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Simulation.Constructions
{
    public class WorkshopAction : IConstructionAction
    {
        private readonly Island island;
        private float nextResourceGen;
        private Level level;
        private Traveller roamingCart;
        private Traveller goodsCart;
        private DistributionHelper distributionHelper;
        private const float Interval = 3;

        private Random random = new Random(0);

        public WorkshopAction(Island island, DistributionHelper distributionHelper)
        {
            this.island = island;
            this.distributionHelper = distributionHelper;
            this.level = island.Level;
            nextResourceGen = TW.Graphics.TotalRunTime + Interval;
        }

        public void Update()
        {
            if (roamingCart != null && roamingCart.Destination == null) roamingCart = null; // Cleanup removed cart
            if (roamingCart == null) createRoamingCart();

            if (goodsCart != null && goodsCart.Destination == null) goodsCart = null; // Cleanup removed cart
            if (goodsCart == null) createGoodsCart();
        }

        private void createGoodsCart()
        {
            var departed = false;
            goodsCart = level.CreateNewTraveller(island, () =>
            {
                if (departed && goodsCart.Island == island)
                {
                    // Back home
                    island.Inventory.TakeAll(goodsCart.Inventory);
                    return null;
                }

                if (isWarehouse(goodsCart.Island))
                {
                    // Take goods
                    goodsCart.Island.Inventory.TransferItemsTo(goodsCart.Inventory, level.ScrapType, Math.Min(4, goodsCart.Island.Inventory[level.ScrapType]));
                }

                if (!departed)
                {
                    departed = true;
                    var warehouse = distributionHelper.FindNearestIsland(island, isWarehouse);
                    return warehouse ?? island;
                }

                return island;


            });
        }


        private void createRoamingCart()
        {

            // TODO: this first part is a generic wanderer algorithm
            var startHops = 3;
            var numHops = startHops;

            roamingCart = level.CreateNewTraveller(island, () =>
                    {
                        if (roamingCart.Island == island && numHops != startHops) return null; // back home, remove cart
                        if (numHops == 0) return island;
                        if (!roamingCart.Island.ConnectedIslands.Any()) return null;

                        numHops--;

                        return roamingCart.Island.ConnectedIslands.ElementAt(
                                random.Next(0, roamingCart.Island.ConnectedIslands.Count()));

                    });


            // This is the specific algorithm

            roamingCart.OnReachIsland = i =>
                {
                    if (!isCamp(i)) return;

                    var missingUnits = 4 - i.Inventory[level.UnitTier1Type];
                    var newUnits = Math.Min(missingUnits, island.Inventory[level.ScrapType] / 2); // 2 scrap per unit

                    island.Inventory.DestroyItems(level.ScrapType, newUnits * 2);

                    i.Inventory.AddNewItems(level.UnitTier1Type, newUnits);

                };

            roamingCart.Type = level.WorkshopCartType;
        }


        private bool isCamp(Island island1)
        {
            return island1.Construction.Name == "Camp"; //TODO
        }

        private bool isWarehouse(Island island1)
        {
            return island1.Construction.Name == "Warehouse"; //TODO
        }
    }
}