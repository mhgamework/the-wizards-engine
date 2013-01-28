using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class WaypointEngine
    {
        private GridConnectionProvider gridConnectionProvider;
        private WaypointConnectionProvider waypointConnectionProvider;
        private IEnumerable<Waypoint> waypoints;
        public float WaypointRange { get; set; }


        public WaypointEngine(GridConnectionProvider gridConnectionProvider, WaypointConnectionProvider waypointConnectionProvider, IEnumerable<Waypoint> waypoints, float waypointRange)
        {
            this.gridConnectionProvider = gridConnectionProvider;
            this.waypointConnectionProvider = waypointConnectionProvider;
            this.waypoints = waypoints;
            WaypointRange = waypointRange;
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
            foreach (var p in wPath)
            {
                if (Vector2.Distance(p.Position, vertex2D.Position) < range)
                    return true;
            }
            return false;
        }

        private Waypoint findClosestWaypoint(Vertex2D end)
        {
            return getWaypoints().OrderBy(w => Vector2.Distance(w.Position, end.Position)).FirstOrDefault();
        }

        private IEnumerable<Waypoint> getWaypoints()
        {
            return waypoints;
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
}
