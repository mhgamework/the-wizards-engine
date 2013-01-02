using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    public class TerrainAStar
    {
        private VoxelTerrain terrain;

        public TerrainAStar()
        {
            terrain = TW.Data.GetSingleton<VoxelTerrain>();
        }

        public List<VoxelBlock> findPath(VoxelBlock start, VoxelBlock goal)
        {
            var closedset = new HashSet<VoxelBlock>();
            var openset = new HashSet<VoxelBlock>();
            openset.Add(start);
            var came_from = new Dictionary<VoxelBlock, VoxelBlock>();

            var g_score = new Dictionary<VoxelBlock, float>();
            var f_score = new Dictionary<VoxelBlock, float>();
            //came_from := the empty map    // The map of navigated nodes.

            g_score[start] = 0;    // Cost from start along best known path.
            // Estimated total cost from start to goal through y.
            f_score[start] = g_score[start] + heuristic_cost_estimate(start, goal);

            int numIts = 100;

            while (openset.Count > 0)
            {
                numIts--;
                if (numIts < 0)
                    return null;
                //openset.Sort((a, b) => f_score[a] < f_score[b] ? -1 : f_score[a] > f_score[b] ? 1 : 0);
                var current = openset.OrderBy(o => f_score[o]).First(); // the node in openset having the lowest f_score[] value
                if (current.Equals(goal))
                    return reconstruct_path(came_from, goal);

                openset.Remove(current); //remove current from openset
                closedset.Add(current); //add current to closedset

                foreach (var neighbor in connected_nodes(current))
                {
                    //TW.Graphics.LineManager3D.AddCenteredBox(neighbor.Position + MathHelper.One*0.5f,0.5f, new Color4(0,0,1));
                    if (closedset.Contains(neighbor))
                        continue;
                    var tentative_g_score = g_score[current] + dist_between(current, neighbor);
                    if (!openset.Contains(neighbor) || tentative_g_score <= g_score[neighbor])
                    {
                        came_from[neighbor] = current;
                        g_score[neighbor] = tentative_g_score;
                        f_score[neighbor] = g_score[neighbor] + heuristic_cost_estimate(neighbor, goal);
                        if (!openset.Contains(neighbor))
                        {
                            openset.Add(neighbor);
                        }
                    }

                }

            }
            return null;

        }


        private class CustomList<T>
        {
            private HashSet<T> set = new HashSet<T>();
            private List<T> list = new List<T>();

            public int Count
            {
                get { return list.Count; }
            }

            public void Add(T start)
            {
                list.Add(start);
                set.Add(start);
            }

            public void Sort(Comparison<T> compare)
            {
                list.Sort(compare);
            }

            public bool Contains(T neighbor)
            {
                return set.Contains(neighbor);
            }

            public T this[int i]
            {
                get { return list[i]; }
            }

            public void Remove(T current)
            {
                set.Remove(current);
                list.Remove(current);
            }
        }
        private float dist_between(VoxelBlock current, VoxelBlock neighbor)
        {
            return 1;
        }

        private IEnumerable<VoxelBlock> connected_nodes(VoxelBlock current)
        {
            bool inAir = !getBelow(current).Filled;

            // give neighbours with soil under
            foreach (var n in neighbor_nodes(current).Where(n => !n.Filled))
            {
                if (n.RelativePosition.Y != current.RelativePosition.Y)
                    continue; // no vertical movement
                VoxelBlock it = n;
                int fallDepth = 3; //TODO fix staying in air when once in air
                if (inAir) fallDepth = 1;
                for (int i = 0; i < fallDepth; i++)
                {
                    it = getBelow(it);
                    if (it != null && it.Filled)
                    {
                        if (n.Filled) throw new InvalidOperationException();
                        yield return n;
                    }
                        
                }
            }

            if (!inAir)
            {
                var up = getUp(current);
                if (up != null && !up.Filled)
                {
                    if (up.Filled) throw new InvalidOperationException();
                    yield return up;
                }
            }
            else
            {
                if (getBelow(current).Filled) throw new InvalidOperationException();
                yield return getBelow(current);
            }

        }
        private IEnumerable<VoxelBlock> neighbor_nodes(VoxelBlock current)
        {
            for (int i = 0; i < 3; i++)
            {
                var axis = new Point3();
                axis[i] = 1;

                var n1 = terrain.GetVoxelAt(terrain.GetPositionOf(current) + axis);
                var n2 = terrain.GetVoxelAt(terrain.GetPositionOf(current) - axis);

                if (n1 != null)
                    yield return n1;
                if (n2 != null)
                    yield return n2;

            }
        }
        private VoxelBlock getBelow(VoxelBlock block)
        {
            return terrain.GetVoxelAt(terrain.GetPositionOf(block) + MathHelper.Down);
        }
        private VoxelBlock getUp(VoxelBlock block)
        {
            return terrain.GetVoxelAt(terrain.GetPositionOf(block) + MathHelper.Up);
        }

        private float heuristic_cost_estimate(VoxelBlock start, VoxelBlock goal)
        {
            var a = terrain.GetPositionOf(start);
            var b = terrain.GetPositionOf(goal);
            float dist = 0;
            for (int i = 0; i < 3; i++)
                dist += (float)Math.Abs(a[i] - b[i]);
            return dist;
        }

        public List<VoxelBlock> reconstruct_path(Dictionary<VoxelBlock, VoxelBlock> came_from, VoxelBlock current_node)
        {
            var ret = new List<VoxelBlock>();

            for (int i = 0; i < 200; i++)
            {
                ret.Add(current_node);
                if (!came_from.ContainsKey(current_node))
                    break;
                current_node = came_from[current_node];
            }

            return ret;
            //if came_from[current_node] in set
            //    p := reconstruct_path(came_from, came_from[current_node])
            //    return (p + current_node)
            //else

            //    return current_node
        }
    }
}
