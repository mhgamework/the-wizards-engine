using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class WaypointEngine
    {
        private GridConnectionProvider gridConnectionProvider = new GridConnectionProvider();
        private WaypointConnectionProvider waypointConnectionProvider = new WaypointConnectionProvider();

        public float WaypointRange { get; set; }
        public WaypointEngine()
        {
            WaypointRange = 10;
        }

        public List<Vertex2D> findPath(Vector2 vStart, Vector2 vEnd)
        {
            var start = gridConnectionProvider.GetVertex(vStart);
            var end = gridConnectionProvider.GetVertex(vEnd);
            var wA = findClosestWaypoint(start);
            var wB = findClosestWaypoint(end);
            var wPath = findPathWaypoints(wA, wB);

            return findPathAlongWaypoints(start, end, wPath);

        }

        private List<Vertex2D> findPathAlongWaypoints(Vertex2D start, Vertex2D end, List<Waypoint> wPath)
        {
            var finder = createPathFinder<Vertex2D>();
            finder.ConnectionProvider = gridConnectionProvider;
            finder.NodeFilter = (n => isNodeWithinPathRange(n, wPath, WaypointRange));
            return finder.FindPath(start, end);
        }

        private bool isNodeWithinPathRange(Vertex2D vertex2D, List<Waypoint> wPath, float range)
        {
            // gwn afstand berekenen?
            throw new NotImplementedException();
        }

        private Waypoint findClosestWaypoint(Vertex2D end)
        {
            // Dees kunde doen das aster gebruiken zoals hierbove
            throw new NotImplementedException();
        }

        public List<Waypoint> findPathWaypoints(Waypoint start, Waypoint end)
        {
            var finder = createPathFinder<Waypoint>();
            finder.ConnectionProvider = waypointConnectionProvider;
            return finder.FindPath(start, end);
        }

        private PathFinder2D<T> createPathFinder<T>() where T : class
            {
            return new PathFinder2D<T>();
        }
    }

    internal class WaypointConnectionProvider : IConnectionProvider<Waypoint>
    {
        public IEnumerable<Waypoint> GetConnectedNodes(Waypoint current)
        {
            throw new NotImplementedException();
        }

        public float GetCost(Waypoint current, Waypoint neighbor)
        {
            throw new NotImplementedException();
        }

        public float GetHeuristicCostEstimate(Waypoint start, Waypoint goal)
        {
            throw new NotImplementedException();
        }
    }

    public class Waypoint
    {
    }
    public class Vertex2D
    {
        public Vertex2D()
        {
            MinDistance = -1;
        }

        public Vertex2D(Vector2 position)
            : this()
        {
            Position = position;

        }

        public Vector2 Position { get; set; }
        public int MinDistance { get; set; }
    }
}
