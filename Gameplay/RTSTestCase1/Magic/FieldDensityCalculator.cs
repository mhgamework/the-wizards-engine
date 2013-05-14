using System;
using System.Collections.Generic;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    /// <summary>
    /// Responsible for calculating the energy densities, given a grid to insert the approximate densities into
    /// </summary>
    public class FieldDensityCalculator
    {
        public void CalculateDensities(List<IFieldElement> elements, IGrid grid)
        {


            grid.Reset();
            foreach (var el in elements)
            {
                var cellIndex = grid.GetCellIndexForCoordinate(el.Position);
                grid.AddDensity(cellIndex, el.Density);
                int range = 4;
                for (int i = -range; i < range + 1; i++)
                {
                    for (int j = -range; j < range + 1; j++)
                    {
                        grid.AddDensity(cellIndex + new Point2(i, j), el.Density/ (Math.Abs(j) + Math.Abs(i) + 1));
                    }
                }

            }


        }

    }

    public interface IGrid
    {
        Point2 GetCellIndexForCoordinate(Vector3 position);
        void AddDensity(Point2 cellIndex, float density);
        void Reset();
        float GetDensity(Point2 cell);
    }

    public interface IFieldElement
    {
        Vector3 Position { get; }
        float Density { get; }
    }

    /// <summary>
    /// World space size = NodeSize * GridSize
    /// 
    /// </summary>
    public class SimpleGrid : IGrid
    {
        public float NodeSize { get; private set; }
        public int GridSize { get; private set; }
        public Vector3 Offset { get; set; }

        private float[,] values;

        public SimpleGrid()
        {
            NodeSize = -1;
            GridSize = -1;
            Offset = new Vector3();
        }

        public void Initialize(float nodeSize, int gridSize)
        {
            NodeSize = nodeSize;
            GridSize = gridSize;

            values = new float[gridSize, gridSize];
        }

        public bool InGrid(Point2 v)
        {
            if (v.X < 0 || v.Y < 0 || v.X >= GridSize || v.Y >= GridSize) return false;

            return true;
        }

        public Point2 GetCellIndexForCoordinate(Vector3 position)
        {
            position = (position - Offset) / NodeSize;
            return Point2.Floor(position.TakeXZ());
        }
        public void AddDensity(Point2 cellIndex, float density)
        {AddDensity(cellIndex,density,5);}

        private void AddDensity(Point2 cellIndex, float density, int range)
        {
            if (!InGrid(cellIndex)) return; // Not necessary but, hey we like duplicate code!
            
                    SetDensity(cellIndex,GetDensity(cellIndex) + density);
        }

        public float GetDensityAt(Vector3 position)
        {
            var coord = GetCellIndexForCoordinate(position);
            return GetDensity(coord);

        }


        public float GetDensity(Point2 p)
        {
            if (!InGrid(p)) return 0;
            return values[p.X, p.Y];
        }
        public void SetDensity(Point2 p, float value)
        {
            if (!InGrid(p)) return;
            values[p.X, p.Y] = value;
        }


        public void Reset()
        {
            forall(p => values[p.X, p.Y] = 0);
        }

        private void forall(Action<Point2> func)
        {
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                {
                    func(new Point2(x, y));
                }
        }
    }
}