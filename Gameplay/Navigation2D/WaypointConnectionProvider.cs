using System;
using System.Collections.Generic;
using System.Linq;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class WaypointConnectionProvider : IConnectionProvider<Waypoint>
    {
        public IEnumerable<Waypoint> GetConnectedNodes(Waypoint current)
        {
            return current.Edges.Select(e=>e.Target);
        }

        public float GetCost(Waypoint current, Waypoint neighbor)
        {
            return current.Edges.First(e => e.Target == neighbor).Distance;
        }

        public float GetHeuristicCostEstimate(Waypoint start, Waypoint goal)
        {
            return Vector2.Distance(start.Position, goal.Position);
        }
    }
}