using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1.Magic;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    public class MagicTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestFieldDensityCalculator()
        {
            var calc = new FieldDensityCalculator();
            var gridResolution = 10;
            var grid = createGrid(gridResolution);

            var list = new List<IFieldElement>();

            list.Add(createElement(new Vector3(5, 0, 5), 10));
            list.Add(createElement(new Vector3(5, 0, 5), 10));

            list.Add(createElement(new Vector3(-5, 0, 5), 10));
            list.Add(createElement(new Vector3(-5, 0, -5), 10));

            calc.CalculateDensities(list,grid);

            // TODO: test 1 cell with 20, 2 cells with 10 and all the rest 0

        }

        private IFieldElement createElement(Vector3 p0, int p1)
        {
            throw new System.NotImplementedException();
        }

        private IGrid createGrid(int gridResolution)
        {
            throw new System.NotImplementedException();
        }
    }
}