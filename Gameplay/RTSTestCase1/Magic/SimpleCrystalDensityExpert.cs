using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    /// <summary>
    /// Responsible for knowing the density of crystals
    /// </summary>
    public class SimpleCrystalDensityExpert : IEnergyDensityExpert
    {

        private FieldDensityCalculator calculator;
        private IGrid grid;
        private List<IFieldElement> crystals = new List<IFieldElement>();
        private bool updated = false;
        private int lastNodeSize;
        private int lastGridSize;
        private Vector3 lastOffSet;
        public void Initialize(int nodeSize, int gridSize,Vector3 offSet)
        {
            calculator = new FieldDensityCalculator();
            var initGrid = CreateGrid();
            initGrid.Initialize(nodeSize, gridSize, offSet);
            grid = initGrid;
            lastNodeSize = nodeSize;
            lastGridSize = gridSize;
            lastOffSet = offSet;
        }
        
        private SimpleGrid CreateGrid()
        {
            var newGrid = new SimpleGrid();
            newGrid.Initialize(lastNodeSize,lastGridSize,lastOffSet);
            return newGrid;
        }

        
        public void PutCrystals(IEnumerable<SimpleCrystal> addCrystals)
        {
            foreach (var simpleCrystal in addCrystals)
            {
                crystals.Add(simpleCrystal);
                updated = false;
            }
        }

        public void ResetDensities(IEnumerable<IFieldElement> newCrystals)
        { 
            grid = CreateGrid();
            updated = false;
            crystals = new List<IFieldElement>(newCrystals);
        } 
        
        public float GetDensity(Vector3 position)
        {
            if(!updated)
                calculator.CalculateDensities(crystals,grid);
            updated = true;
            return grid.GetDensity(grid.GetCellIndexForCoordinate(position));
        }
    }
}