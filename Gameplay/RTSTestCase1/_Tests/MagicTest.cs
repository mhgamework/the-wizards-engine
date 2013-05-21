using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.Magic;
using MHGameWork.TheWizards.RTSTestCase1.Magic.Simulators;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    public class MagicTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestFieldDensityCalculator()
        {
            TestUtilities.CreateGroundPlane();
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

       
        [Test] 

        public void TestMagicSimulation()
        {
            var crystal = new SimpleCrystal() { Position = new Vector3(0, 0, 0), Capacity = 1000 };
            var crystal2 = new SimpleCrystal() { Position = new Vector3(10, 0, 0), Capacity = 1000 };
            
            var crystal3 = new SimpleCrystal() { Position = new Vector3(20, 0, 0), Capacity = 1000, Energy = 1000 };
            var crystal4 = new SimpleCrystal() { Position = new Vector3(30, 0, 0), Capacity = 1000, Energy = 1000 };
            engine.AddSimulator(new CrystalBrownSimulator());
            engine.AddSimulator(new MagicSimulator());
            engine.AddSimulator(new RTSEntitySimulator());
            engine.AddSimulator(new CrystalInfoDrawSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
               
        }

        public void TestMagicConsumption()
        {
            var crystal = new SimpleCrystal() {Position = new Vector3(0, 0, 0), Capacity = 1000};
            var consumer1 = new SimpleCrystalEnergyConsumer() {Position = new Vector3(1, 0, 0)};
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