using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.Magic;
using MHGameWork.TheWizards.RTSTestCase1.MagicSimulation;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    class EnergyFlowTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestBigToSmallSlowFlow()
        {
            var sim = new CrystalAverager();
                                                                                                                    
            var simpleCryst1 = new SimpleCrystal { Position = new Vector3(0, 0, 0), Capacity = 1000, Energy = 500 };
            var simpleCryst2 = new SimpleCrystal { Position = new Vector3(0, 0, 10), Capacity = 1, Energy = 1 };    
            var simpleCryst3 = new SimpleCrystal { Position = new Vector3(0, 0, 5), Capacity = 10, Energy = 10 };   
            var crystals = new List<SimpleCrystal> { simpleCryst1, simpleCryst2, simpleCryst3 };
            for (var i = 0; i < 100; i++)
            {
                sim.processCrystal(crystals, simpleCryst1, 0.1f);
                sim.processCrystal(crystals, simpleCryst2, 0.1f);
                sim.processCrystal(crystals, simpleCryst3, 0.1f);
            }
        }
    }
}
