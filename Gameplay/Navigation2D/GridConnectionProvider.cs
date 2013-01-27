using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class GridConnectionProvider : IConnectionProvider<Vertex2D>
    {
        public NavigableGrid2D Grid { get; set; }
        public int Size { get; set; }



        private Vector2[] neighbours = new Vector2[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };
        private Dictionary<Vector2, Vertex2D> vertices = new Dictionary<Vector2, Vertex2D>();

        public GridConnectionProvider()
        {
            Size = 3;

        }

        public IEnumerable<Vertex2D> GetConnectedNodes(Vertex2D current)
        {
            for (int i = 0; i < neighbours.Length; i++)
            {
                var pos = current.Position + neighbours[i];
                var x = (int)pos.X;
                var y = (int)pos.Y;


                var ret = GetVertex(x, y);
                if (ret == null) continue;



                if (getMinDist(ret) < Size) continue;
                yield return ret;
            }
        }

        public float GetCost(Vertex2D current, Vertex2D neighbor)
        {
            return Vector2.DistanceSquared(current.Position, neighbor.Position);
        }

        public Vertex2D GetVertex(Vector2 v)
        {
            return GetVertex((int) v.X, (int) v.Y);
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
            var ret = getMinDist2(current);
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
            return Vector2.Distance(start.Position, goal.Position) * 2f;
        }
       
    }
}