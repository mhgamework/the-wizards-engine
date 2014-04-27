using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class NavigableGrid2D
    {
        private short[,] free;

        private HashSet<Object> objects = new HashSet<object>();

        public NavigableGrid2D()
        {
        }

        public float NodeSize { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public void Create(float nodeSize, int width, int height)
        {
            this.Height = height;
            this.Width = width;
            this.NodeSize = nodeSize;
            free = new short[width, height];
        }

        public void AddObject(object obj, BoundingBox bb)
        {
            if (objects.Contains(obj)) throw new InvalidOperationException();
            objects.Add(obj);
            foreachInBB(bb, (x, z) =>
                {
                    if (!InGrid(x, z)) return;

                    free[x, z]++;
                });
        }

        private void foreachInBB(BoundingBox bb, Action<int, int> action)
        {
            // Normalize bb
            bb.Minimum = bb.Minimum / NodeSize;
            bb.Maximum = bb.Maximum / NodeSize;

            var min = new int[3];
            var max = new int[3];
            for (int i = 0; i < 3; i++)
            {
                min[i] = (int)Math.Floor(bb.Minimum[i] + 0.001f);
                max[i] = (int)Math.Ceiling(bb.Maximum[i] - 0.001f);
            }
            for (int x = min[0]; x < max[0]; x++)
                for (int z = min[2]; z < max[2]; z++)
                {
                    action(x, z);
                }
        }

        public void MoveObject(object obj, BoundingBox bb)
        {
            RemoveObject(obj, bb);
            AddObject(obj, bb);
        }
        public void RemoveObject(object obj, BoundingBox bb)
        {
            if (!objects.Contains(obj)) throw new InvalidOperationException();
            objects.Remove(obj);
            foreachInBB(bb, (x, z) =>
                {
                    if (!InGrid(x, z)) return;
                    free[x, z]--;
                });
        }


        public bool IsFree(int x, int y)
        {
            return free[x, y] == 0;
        }

        public bool InGrid(int x, int y)
        {
            if (x < 0) return false;
            if (y < 0) return false;
            if (x >= Width) return false;
            if (y >= Width) return false;
            return true;
        }
    }
}
