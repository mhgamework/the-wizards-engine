using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class WaypointEngine
    {
        public List<Vertex2D> findPath(Vector2 start, Vector2 end)
        {
            var wA = findClosestWaypoint(start);
            var wB = findClosestWaypoint(end);
            var wPath = findPathWaypoints(wA, wB);

            return findPathAlongWaypoints(start, end, wPath);

        }

        private List<Vertex2D> findPathAlongWaypoints(Vector2 start, Vector2 end, List<Waypoint> wPath)
        {
            var finder = createPathFinder<Vertex2D>();
            finder.NodeGenerator = getGridNodeGenerator();
            finder.NodeFilter = n => isNodeWithinPathRange(n, wPath, range);
            return finder.FindPath(start, end);
        }

        private void isNodeWithinPathRange(Vertex2D vertex2D, List<Waypoint> wPath, float range)
        {
            // gwn afstand berekenen?
        }

        private Func<Vertex2D, IEnumerable<Vertex2D>> getGridNodeGenerator()
        {
            // Data structure todo
        }

        private Waypoint findClosestWaypoint(Vector2 end)
        {
            // Dees kunde doen das aster gebruiken zoals hierbove
        }

        public List<Waypoint> findPathWaypoints(Waypoint start, Waypoint end)
        {
            var finder = createPathFinder<Waypoint>();
            finder.NodeGenerator = getWaypointNodeGenerator();
            return finder.FindPath(start, end);
        }

        private Func<Waypoint, IEnumerable<Waypoint>> getWaypointNodeGenerator()
        {
            // Data structure todo
        }

        private PathFinder2D<T> createPathFinder<T>()
        {
            // factory method todo
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
