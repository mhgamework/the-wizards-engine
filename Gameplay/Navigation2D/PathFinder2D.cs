using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Profiling;
using MHGameWork.TheWizards.VSIntegration;
using QuickGraph;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Collections;
using SlimDX;
using QuickGraph.Algorithms;
using Debugger = System.Diagnostics.Debugger;

namespace MHGameWork.TheWizards.Navigation2D
{

    public class PathFinder2D
    {
        public NavigableGrid2D Grid { get; set; }
        public int Size { get; set; }
        public PathFinder2D()
        {
            Size = 3;
            MaxGScore = 100;
        }

        public List<Vertex2D> FindPath(Vector2 fStart, Vector2 fEnd)
        {
            var start = new Vertex2D(fStart);
            var goal = new Vertex2D(fEnd);

            vertices.Add(fStart, start);
            vertices.Add(fEnd, goal);

            var g_score = new Dictionary<Vertex2D, float>();
            var f_score = new Dictionary<Vertex2D, float>();


            //var openset = new HashSet<Vertex2D>();
            var openset = new FibonacciQueue<Vertex2D, float>(d => f_score[d]);
            var closedset = new HashSet<Vertex2D>();
            var came_from = new Dictionary<Vertex2D, Vertex2D>();

            //came_from := the empty map    // The map of navigated nodes.

            g_score[start] = 0;    // Cost from start along best known path.
            // Estimated total cost from start to goal through y.
            f_score[start] = g_score[start] + heuristic_cost_estimate(start, goal);


            openset.Enqueue(start);

            while (openset.Count > 0)
            {
                var current = openset.Dequeue();

                if (g_score[current] > MaxGScore)
                    return reconstruct_reverse_path(came_from, current).Reverse().ToList();

                if (current.Equals(goal))
                    return reconstruct_reverse_path(came_from, goal).Reverse().ToList();

                closedset.Add(current); //add current to closedset

                foreach (var neighbor in connected_nodes(current))
                {
                    if (closedset.Contains(neighbor)) continue;

                    var tentative_g_score = g_score[current] + dist_between(current, neighbor);
                    if (openset.Contains(neighbor) && !(tentative_g_score <= g_score[neighbor])) continue;

                    came_from[neighbor] = current;
                    g_score[neighbor] = tentative_g_score;
                    f_score[neighbor] = g_score[neighbor] + heuristic_cost_estimate(neighbor, goal);

                    if (openset.Contains(neighbor)) continue;

                    openset.Enqueue(neighbor);
                }

            }
            return null;

        }

        protected float MaxGScore { get; set; }

        private float dist_between(Vertex2D current, Vertex2D neighbor)
        {
            return Vector2.DistanceSquared(current.Position, neighbor.Position);
        }

        private Vector2[] neighbours = new Vector2[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };
        private Dictionary<Vector2, Vertex2D> vertices = new Dictionary<Vector2, Vertex2D>();
        private IEnumerable<Vertex2D> connected_nodes(Vertex2D current)
        {
            for (int i = 0; i < neighbours.Length; i++)
            {
                var pos = current.Position + neighbours[i];
                var x = (int)pos.X;
                var y = (int)pos.Y;


                var ret = getVertex(x, y);
                if (ret == null) continue;



                if (getMinDist(ret) < Size) continue;
                yield return ret;
            }
        }

        private Vertex2D getVertex(int x, int y)
        {
            if (!Grid.InGrid(x, y)) return null;
            if (!vertices.ContainsKey(new Vector2(x, y)))
                vertices.Add(new Vector2(x, y), new Vertex2D(new Vector2(x, y)));
            var ret = vertices[new Vector2(x, y)];
            return ret;
        }

        private int getMinDist(Vertex2D current)
        {
            count2++;
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
                    count++;
                    if (!Grid.IsFree(ix, iy)) return 0;

                }

            count++;
            return int.MaxValue - 10;
        }
        private Vector2[] neighbours2 = new Vector2[] { 
            new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) ,
            new Vector2(1,1),new Vector2(-1,1),new Vector2(1,-1), new Vector2(-1,-1)   };


        private int count = 0;

        private int getMinDist(Vertex2D current, int numSteps)
        {

            if (current == null) return int.MaxValue - 10;
            if (current.MinDistance >= 0) return current.MinDistance;
            if (!Grid.IsFree((int)current.Position.X, (int)current.Position.Y)) return 0;
            if (numSteps <= 0) return int.MaxValue - 10; // prevent buffer overflows!

            count++;
            
            var min = int.MaxValue;

            current.MinDistance = int.MaxValue - 10;

            for (int i = 0; i < neighbours2.Length; i++)
            {
                var pos = current.Position + neighbours2[i];
                var x = (int)pos.X;
                var y = (int)pos.Y;
                var n = getVertex(x, y);
                var t = getMinDist(n, numSteps - 1) + 1;

                if (t < min) min = t;
            }

            current.MinDistance = -1;

            if (numSteps >= Size)
                current.MinDistance = min;

            return min;
        }

        public IEnumerable<T> reconstruct_reverse_path<T>(IDictionary<T, T> came_from, T current_node)
        {
            //Console.WriteLine(count / count2);
            count2 = 0;

            var ret = new List<VoxelBlock>();
            for (int i = 0; i < 200; i++)
            {
                yield return current_node;
                if (!came_from.ContainsKey(current_node))
                    break;
                current_node = came_from[current_node];
            }
        }

        private float heuristic_cost_estimate(Vertex2D start, Vertex2D goal)
        {
            return Vector2.Distance(start.Position, goal.Position) * 2f;
        }

        private List<Vertex2D> verts = new List<Vertex2D>();
        private Vertex2D nStart;
        private Vertex2D nEnd;
        private Vertex2D nInBetween = new Vertex2D();
        private int count2;

        private IEnumerable<Vertex2D> enumVerts()
        {
            yield return nStart;
            yield return nEnd;

            //yield break;
        }
        private IEnumerable<Edge2D> enumEdges(Vertex2D arg)
        {
            if (arg == nStart) yield return new Edge2D { Source = nStart, Target = nInBetween };
            if (arg == nInBetween) yield return new Edge2D { Source = nInBetween, Target = nEnd };
        }

        private double calculateLength(Edge2D arg)
        {
            return Vector2.Distance(arg.Target.Position, arg.Source.Position);
        }

        private double estimateCost(Vertex2D arg)
        {
            return arg.Position.LengthSquared();
        }


    }
    
    public class Edge2D : IEdge<Vertex2D>, IEquatable<Edge2D>
    {
        public Vertex2D Source { get; set; }
        public Vertex2D Target { get; set; }
        public bool Equals(Edge2D other)
        {
            return Source == other.Source && Target == other.Target;
        }
    }
}
