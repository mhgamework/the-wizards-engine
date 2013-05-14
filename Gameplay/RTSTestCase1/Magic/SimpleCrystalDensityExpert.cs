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
    
        public void Initialize(int nodeSize, int gridSize)
        {
            calculator = new FieldDensityCalculator();
            var initGrid = CreateGrid();
            initGrid.Initialize(nodeSize, gridSize);
            grid = initGrid;
        }
        
        private static SimpleGrid CreateGrid()
        {
            return new SimpleGrid();
        }


        [TWProfile]
        public void PutCrystals(IEnumerable<SimpleCrystal> addCrystals)
        {
            foreach (var simpleCrystal in addCrystals)
            {
                crystals.Add(simpleCrystal);
                updated = false;
            }
        }
        [TWProfile]
        public void ResetDensities()
        {
            SimpleGrid grid = CreateGrid();
        }
        [TWProfile]
        public float GetDensity(Vector3 position)
        {
            if(!updated)
                calculator.CalculateDensities(crystals,grid);
            updated = true;
            return grid.GetDensity(grid.GetCellIndexForCoordinate(position));
        }
    }
}