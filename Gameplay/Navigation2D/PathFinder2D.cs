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

    /// <summary>
    /// TODO: add Jump Point Search for symmetric areas!!!
    /// </summary>
    public class PathFinder2D<T> where T : class
    {
        private Dictionary<T, float> gScore;
        private Dictionary<T, float> fScore;
        public Func<T, bool> StopCondition { get; set; }

        public PathFinder2D()
        {
            StopCondition = x => true;
        }

        public List<T> FindPath(T start, T goal)
        {
            if (start == null) return null;
            if (goal == null) return null;

            gScore = new Dictionary<T, float>();
            fScore = new Dictionary<T, float>();


            //var openset = new HashSet<T>();
            var openset = new FibonacciQueue<T, float>(d => fScore[d]);
            var closedset = new HashSet<T>();
            var came_from = new Dictionary<T, T>();

            //came_from := the empty map    // The map of navigated nodes.

            gScore[start] = 0;    // Cost from start along best known path.
            // Estimated total cost from start to goal through y.
            fScore[start] = gScore[start] + ConnectionProvider.GetHeuristicCostEstimate(start, goal);


            openset.Enqueue(start);

            while (openset.Count > 0)
            {
                var current = openset.Dequeue();

                if (StopCondition(current))
                    return reconstruct_reverse_path(came_from, current).Reverse().ToList();

                if (current.Equals(goal))
                    return reconstruct_reverse_path(came_from, goal).Reverse().ToList();

                closedset.Add(current); //add current to closedset

                foreach (var neighbor in ConnectionProvider.GetConnectedNodes(current))
                {
                    if (closedset.Contains(neighbor)) continue;

                    var tentative_g_score = gScore[current] + ConnectionProvider.GetCost(current, neighbor);
                    if (openset.Contains(neighbor) && !(tentative_g_score <= gScore[neighbor])) continue;

                    came_from[neighbor] = current;
                    gScore[neighbor] = tentative_g_score;
                    fScore[neighbor] = gScore[neighbor] + ConnectionProvider.GetHeuristicCostEstimate(neighbor, goal);

                    if (openset.Contains(neighbor)) continue;

                    openset.Enqueue(neighbor);
                }

            }
            return null;

        }


        public Func<T, bool> NodeFilter { get; set; }



        public IEnumerable<T> reconstruct_reverse_path<T>(IDictionary<T, T> came_from, T current_node)
        {
            for (int i = 0; i < 200; i++)
            {
                yield return current_node;
                if (!came_from.ContainsKey(current_node))
                    break;
                current_node = came_from[current_node];
            }
        }



        public IConnectionProvider<T> ConnectionProvider { get; set; }

        public float GetCurrentCost(T vertex2D)
        {
            return gScore[vertex2D];
        }
    }
}
