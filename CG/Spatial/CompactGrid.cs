using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;

namespace MHGameWork.TheWizards.CG.Spatial
{
    /// <summary>
    /// Responsible for implementing the CompactGrid structure as described in :
    /// </summary>
    /// <remarks>
    /// Responsible for implementing the CompactGrid structure as described in :
    /// 
    /// Compact, Fast and Robust Grids for Ray Tracing
    /// Ares Lagae & Philip Dutré
    /// </remarks>
    public class CompactGrid
    {
        private int[] C;
        private Point3 M;
        private List<ISceneObject> objects;
        private BoundingBoxCalculator bbCalc;
        public float NodeSize { get; private set; }
        private int[] L;

        public Vector3 GridOffset { get; private set; }
        private Vector3 inverseNodeSize;


        public CompactGrid()
        {
            bbCalc = new BoundingBoxCalculator();
        }

        public Point3 NodeCount
        {
            get { return M; }
        }

        public int getCellIndex(Point3 pos)
        {
            return (((NodeCount.Y * pos.Z) + pos.Y) * NodeCount.X) + pos.X;
        }

        /*public void Intersects(Point3 pos)
        {
            // intersect all objects in cell (x,y,z) 
            int i = getCIndex(pos);
            BoundingBox bb;
            for (int j = C[i]; j < C[i + 1]; ++j)
            {
                // intersect object L[j] 
            }
        }*/



        public void GetNodeBoundingBox(int index, out BoundingBox bb)
        {
            int z = (int)(index / (M.Y * M.X));
            index -= z * (M.Y * M.X);
            int y = (int)(index / (M.X));
            index -= y * M.X;
            int x = index;

            bb.Minimum.X = GridOffset.X + x * NodeSize;
            bb.Minimum.Y = GridOffset.Y + y * NodeSize;
            bb.Minimum.Z = GridOffset.Z + z * NodeSize;

            bb.Maximum = bb.Minimum + new Vector3(NodeSize, NodeSize, NodeSize);

        }

        /// <summary>
        /// Uses given set of objects to build this grid. This overrides the old data
        /// </summary>
        /// <param name="objects"></param>
        public void buildGrid(List<ISceneObject> objects)
        {
            this.objects = objects;

            calculateNumberOfCells(4, objects.Count, caculateTotalBoundingBox());
            C = createCellArray();

            calculateCellObjectCounts();

            convertToSpecialIndex();

            createLArray();

            fillLArray();

        }

        private void fillLArray()
        {
            int N = objects.Count;

            for (int i = N - 1; i >= 0; --i)
            {
                var o = objects[i];
                var bb = o.BoundingBox;


                int i1 = i;
                doForeachCellInBoundingBox(bb, delegate(int j)
                {
                    /* for each cell j overlapped by object i */
                    L[--C[j]] = i1;
                });


            }
        }

        private void createLArray()
        {
            L = new int[C[GetTotalCellCount()]];
        }

        private void convertToSpecialIndex()
        {
            for (int i = 1; i <= GetTotalCellCount(); ++i)
            {
                C[i] += C[i - 1];
            }
        }

        public int GetTotalCellCount()
        {
            return NodeCount.X * NodeCount.Y * NodeCount.Z;
        }

        private void calculateCellObjectCounts()
        {
            var inverseNodeSize = new Vector3(1f / NodeSize);

            foreach (var o in objects)
            {
                var bb = o.BoundingBox;
                doForeachCellInBoundingBox(bb, i => C[i]++);

            }
        }

        private void doForeachCellInBoundingBox(BoundingBox bb, Action<int> callback)
        {
            bb.Minimum = Vector3.Modulate(bb.Minimum - GridOffset, inverseNodeSize);
            bb.Maximum = Vector3.Modulate(bb.Maximum - GridOffset, inverseNodeSize);
            //TODO: use epsilons here?

            for (int x = (int)bb.Minimum.X; x <= (int)bb.Maximum.X; x++)
                for (int y = (int)bb.Minimum.Y; y <= (int)bb.Maximum.Y; y++)
                    for (int z = (int)bb.Minimum.Z; z <= (int)bb.Maximum.Z; z++)
                        callback(getCellIndex(new Point3(x, y, z)));

        }



        private int[] createCellArray()
        {
            // One extra as described in the paper
            return new int[NodeCount.X * NodeCount.Y * NodeCount.Z + 1];
        }

        private void calculateNumberOfCells(int gridDenstity, int numberOfObjects, BoundingBox totalBb)
        {
            var size = totalBb.Maximum - totalBb.Minimum;

            float volume = size.X * size.Y * size.Z;


            var temp = (float)System.Math.Pow(gridDenstity * numberOfObjects / volume, 1 / 3f);
            M.X = (int)System.Math.Ceiling(size.X * temp);
            M.Y = (int)System.Math.Ceiling(size.Y * temp);
            M.Z = (int)System.Math.Ceiling(size.Z * temp);

            NodeSize = 1 / temp;
            inverseNodeSize = new Vector3(1f / NodeSize);

            GridOffset = totalBb.Minimum;


        }

        private BoundingBox caculateTotalBoundingBox()
        {
            var ret = new BoundingBox();
            foreach (var o in objects)
            {
                var add = o.BoundingBox;
                //TODO: check if this works!!! (double ret)
                BoundingBox.MergeWith(ref ret, ref add, ref ret);
            }
            return ret;
        }

        public int GetNumberObjects(int cell)
        {
            return C[cell + 1] - C[cell];
        }

        public ISceneObject getCellObject(int cell, int objectIndex)
        {
            return objects[L[C[cell] + objectIndex]];
        }

        public bool InGrid(Point3 point3)
        {
            if (point3.X < 0) return false;
            if (point3.Y < 0) return false;
            if (point3.Z < 0) return false;
            if (point3.X >= M.X) return false;
            if (point3.Y >= M.Y) return false;
            if (point3.Z >= M.Z) return false;
            return true;
        }
    }
}
