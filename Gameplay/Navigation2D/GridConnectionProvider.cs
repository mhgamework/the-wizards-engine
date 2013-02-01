using System;
using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class GridConnectionProvider : IConnectionProvider<Vertex2D>
    {
        public NavigableGrid2D Grid { get; set; }
        public int Size { get; set; }
        public Func<Vertex2D, Vertex2D, float> Heuristic { get; set; }


        private Vector2[] neighbours = new Vector2[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };
        private Dictionary<Vector2, Vertex2D> vertices = new Dictionary<Vector2, Vertex2D>();

        public GridConnectionProvider()
        {
            Size = 1;

            Heuristic = (start, goal) => Vector2.Distance(start.Position, goal.Position) * 1.001f;
            Heuristic =
                (start, goal) =>
                Math.Abs(start.Position.X - goal.Position.X) + Math.Abs(start.Position.Y - goal.Position.Y);

        }

        private int count = 0;
        public IEnumerable<Vertex2D> GetConnectedNodes(PathFinder2D<Vertex2D> finder, Vertex2D current)
        {
            count++;
            //var cameFrom = finder.GetCameFrom(current);
            //if (cameFrom == null) return getConnectionsAll(current);
            return getConnectionsLeaped(finder, null, current);
            //return getConnectionsAll(current);
        }

        private IEnumerable<Vertex2D> getConnectionsLeaped(PathFinder2D<Vertex2D> finder, Vertex2D getCameFrom, Vertex2D current)
        {
            //if (count > 20) yield break;
            this.activeFinder = finder;

            var dir = new Vector2(1, 0);
            var right = new Vector2(0, 1);

            Vertex2D nRight;
            Vertex2D nLeft;

            nRight = doJump(current, right, dir);
            nLeft = doJump(current, -right, dir);

            if (nRight != null) yield return nRight;
            if (nLeft != null) yield return nLeft;

            var top = findHorizontalJump(current, dir, right);
            var bottom = findHorizontalJump(current, dir, right);

            if (top != null) yield return top;
            if (bottom != null) yield return bottom;
        }

        private Vertex2D findHorizontalJump(Vertex2D current, Vector2 dir, Vector2 right)
        {
            Vertex2D nRight;
            Vertex2D nLeft;
             
            // Get right and left neighbours
            do
            {
                current = GetVertex(current.Position + dir);
                if (!canWalkOn(current)) return null;
                if (current == activeFinder.Goal) return current;
                if (current == null) return null;
                nRight = doJump(current, right, dir);
                nLeft = doJump(current, -right, dir);

            } while (nRight == null && nLeft == null);

            return current;

        }

        private Vertex2D doJump(Vertex2D current, Vector2 dir, Vector2 right)
        {
            var n = current;
            if (n == null) return null;
            do
            {
                if (n == activeFinder.Goal) return n;
                n = GetVertex(n.Position + dir);
                if (n == null) return null;
                if (!canWalkOn(n)) return null;
            } while (!isJumpPoint(n, ref dir, ref right));
            return n;
        }

        private bool isJumpPoint(Vertex2D pos, ref Vector2 dir, ref Vector2 right)
        {
            return (!canWalkOn(GetVertex(pos.Position + right - dir)) && canWalkOn(GetVertex(pos.Position + right)))
                   || (!canWalkOn(GetVertex(pos.Position - right - dir)) && canWalkOn(GetVertex(pos.Position - right)));
        }

        private IEnumerable<Vertex2D> getConnectionsAll(Vertex2D current)
        {
            for (int i = 0; i < neighbours.Length; i++)
            {
                var pos = current.Position + neighbours[i];
                var x = (int)pos.X;
                var y = (int)pos.Y;


                var ret = GetVertex(x, y);
                if (ret == null) continue;


                if (!canWalkOn(ret)) continue;
                yield return ret;
                break;
            }
        }

        private bool canWalkOn(Vertex2D ret)
        {
            if (ret == null) return false;
            return getMinDist(ret) >= Size;
        }

        public float GetCost(Vertex2D current, Vertex2D neighbor)
        {
            return Vector2.DistanceSquared(current.Position, neighbor.Position);
        }

        public Vertex2D GetVertex(Vector2 v)
        {
            return GetVertex((int)v.X, (int)v.Y);
        }
        public Vertex2D GetVertex(int x, int y)
        {
            if (!Grid.InGrid(x, y)) return null;
            if (!vertices.ContainsKey(new Vector2(x, y)))
                vertices.Add(new Vector2(x, y), new Vertex2D(new Vector2(x, y)));
            var ret = vertices[new Vector2(x, y)];
            return ret;
        }

        private int getMinDist(Vertex2D current)
        {
            if (current.MinDistance > 0)
                return current.MinDistance;
            var ret = getMinDist2(current);
            current.MinDistance = ret;
            //var ret = getMinDist(current, Size );
            return ret;
        }

        private int getMinDist2(Vertex2D current)
        {
            var x = (int)current.Position.X;
            var y = (int)current.Position.Y;
            if (!Grid.IsFree(x, y)) return 0;

            for (int ix = x - Size; ix < x + Size; ix++)
                for (int iy = y - Size; iy < y + Size; iy++)
                {
                    if (!Grid.InGrid(ix, iy)) continue;
                    if (!Grid.IsFree(ix, iy)) return 0;

                }

            return int.MaxValue - 10;
        }

        private Vector2[] neighbours2 = new Vector2[] { 
            new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) ,
            new Vector2(1,1),new Vector2(-1,1),new Vector2(1,-1), new Vector2(-1,-1)   };

        private PathFinder2D<Vertex2D> activeFinder;


        private int getMinDist(Vertex2D current, int numSteps)
        {

            if (current == null) return int.MaxValue - 10;
            if (current.MinDistance >= 0) return current.MinDistance;
            if (!Grid.IsFree((int)current.Position.X, (int)current.Position.Y)) return 0;
            if (numSteps <= 0) return int.MaxValue - 10; // prevent buffer overflows!

            var min = int.MaxValue;

            current.MinDistance = int.MaxValue - 10;

            for (int i = 0; i < neighbours2.Length; i++)
            {
                var pos = current.Position + neighbours2[i];
                var x = (int)pos.X;
                var y = (int)pos.Y;
                var n = GetVertex(x, y);
                var t = getMinDist(n, numSteps - 1) + 1;

                if (t < min) min = t;
            }

            current.MinDistance = -1;

            if (numSteps >= Size)
                current.MinDistance = min;

            return min;
        }


        public float GetHeuristicCostEstimate(Vertex2D start, Vertex2D goal)
        {
            return Heuristic(start, goal);
        }

    }
}