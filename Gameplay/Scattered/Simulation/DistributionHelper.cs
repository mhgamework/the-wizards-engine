using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    public class DistributionHelper
    {
        private readonly Level level;
        private readonly IPathFinder2D<Island> pathFinder;

        public DistributionHelper(Level level, IPathFinder2D<Island> pathFinder)
        {
            this.level = level;
            this.pathFinder = pathFinder;
        }

        /// <summary>
        /// Returns null if no reachable target can be found!
        /// creates a cart that goes to the closest island with targetpredicate and drops off all its items.
        /// When empty returns to the start island
        /// </summary>
        /// <param name="start"></param>
        /// <param name="targetPredicate"></param>
        /// <returns></returns>
        public Traveller CreateDeliveryCart(Island start, Func<Island, bool> targetPredicate)
        {
            //TODO: check unreachable too
            if (!FindReachableIslands(start, targetPredicate).Any()) return null;

            Traveller cart = null;
            cart = level.CreateNewTraveller(start, delegate()
                {
                    if (targetPredicate(cart.Island))
                    {
                        // When at warehouse, drop off goods
                        cart.Island.Inventory.TakeAll(cart.Inventory);
                    }

                    if (cart.Inventory.ItemCount > 0)
                    {
                        // When has goods, go to warehouse
                        var nearestWarehouse = FindNearestIsland(start, targetPredicate);
                        if (nearestWarehouse == null) return cart.Island; // Wait in place
                        return nearestWarehouse;
                    }

                    if (cart.IsAtIsland(start))
                    {
                        // When home and empty, no more destination!
                        cart = null;
                        return null;
                    }

                    return start; // go home!
                });

            cart.Type = level.DeliveryCartType;
            return cart;
        }

        public Island FindNearestIsland(Island start, Func<Island, bool> targetPredicate)
        {
            // All reachable warehouses
            var warehouses = FindReachableIslands(start, targetPredicate);

            Func<List<Island>, float> getPathLength = path => path.Take(path.Count() - 1)
                                                                  .Zip(path.Skip(1), (a, b) =>
                                                                                     Vector3.Distance(a.Position,
                                                                                                      b.Position))
                                                                  .Sum();

            Func<Island, float> getDistance = w => getPathLength(pathFinder.FindPath(start, w));

            return warehouses.OrderBy(getDistance).FirstOrDefault();
        }

        public IEnumerable<Island> FindReachableIslands(Island start, Func<Island, bool> targetPredicate)
        {
            return level.Islands.Where(targetPredicate).Where(w => pathFinder.FindPath(start, w) != null);
        }

    }
}