using System.Collections.Generic;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    /// <summary>
    /// Interface for allowing
    /// </summary>
    public class IslandConnectionProvider:IConnectionProvider<Island>
    {
        public IEnumerable<Island> GetConnectedNodes(PathFinder2D<Island> finder, Island current)
        {
            return current.ConnectedIslands;
        }

        public float GetCost(Island current, Island neighbor)
        {
            return Vector3.Distance(current.Position, neighbor.Position);
        }

        public float GetHeuristicCostEstimate(Island start, Island goal)
        {
            return GetCost(start, goal);
        }
    }
}