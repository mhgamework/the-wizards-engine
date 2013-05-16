using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic.Simulators
{
    class MagicSimulator : ISimulator
    {
        CrystalExploder crystalExploder = new CrystalExploder();
        CrystalSpawner crystalSpawner = new CrystalSpawner();
        AmbientEnergyCharger ambientCharger;
        CrystalAverager averager = new CrystalAverager();
        SimpleCrystalDensityExpert densityExpert;
        private GridDrawSimulator drawSim;
        private int nodeSize = 10;
        private int gridSize = 20;
        public MagicSimulator()
        {
            densityExpert = new SimpleCrystalDensityExpert();
            densityExpert.Initialize(nodeSize, gridSize, new Vector3(-100, 0, -100));
            ambientCharger = new AmbientEnergyCharger(densityExpert);
            crystalSpawner.MinPosition = new Vector3(-100, 0, -100);
            crystalSpawner.MaxPosition = -crystalSpawner.MinPosition;
            drawSim = new GridDrawSimulator(densityExpert);
        }
        
        public void Simulate()
        {
            var elapsedTime = TW.Graphics.Elapsed;
            var crystals = TW.Data.Objects.Where(o => o is ICrystal && ((ICrystal)o).IsActive()).Cast<ICrystal>();
            SimulateExplode(crystals, elapsedTime);
            crystals = TW.Data.Objects.Where(o => o is ICrystal && ((ICrystal)o).IsActive()).Cast<ICrystal>();
            SimulateAverager(crystals, elapsedTime);
            SimulateAmbientCharging(crystals, elapsedTime);
            drawBarsOverCrystals();
            densityExpert.ResetDensities(crystals.Cast<SimpleCrystal>());
            crystalSpawner.DoSpawn(densityExpert, elapsedTime);
            //refineDensities(crystals.Cast<SimpleCrystal>());
            drawSim.Simulate();
        }

        private void refineDensities(IEnumerable<IFieldElement> crystals )
        {
            refineCounter --;
            if (refineCounter > 0) return;
            refineCounter += 20;
            densityExpert.ResetDensities(crystals);
        }

        private int refineCounter = 20;
        private void drawBarsOverCrystals()
        {
            var crystalRenderData = TW.Data.Objects.OfType<SimpleCrystal>().Select(o => o.get<CrystalRenderData>());
            foreach (var crystal in crystalRenderData.Where(crystal => crystal != null))
            {
                crystal.RenderBar();
            }
        }

        
        private IEnumerator<object> charger;

        private void SimulateAmbientCharging(IEnumerable<ICrystal> crystals, float elapsedTime)
        {
            ambientCharger.ChargeAllCrystals(crystals, elapsedTime);
            return;
            if (charger == null)
                return;// charger = ambientCharger.ChargeAllCrystals(crystals, elapsedTime).GetEnumerator();

            if (!charger.MoveNext()) charger = null;


        }
        private void SimulateAverager(IEnumerable<ICrystal> crystals, float elapsedTime)
        {
            foreach (var crystal in crystals)
            {
                averager.processCrystal(crystals, crystal, elapsedTime);
            }
        }

        private void SimulateExplode(IEnumerable<ICrystal> crystals, float elapsedTime)
        {
            var kaboomed = crystalExploder.CheckExplode(densityExpert, crystals, elapsedTime);
                foreach(var kaboom in kaboomed)
                {
                    ((SimpleCrystal) kaboom).Active = false;
                }
        }

        private IEnumerable<ICrystal> SimulateSpawn(float elapsedTime)
        {

            //throw new Exception("Not Yet Working");
            var newies = crystalSpawner.DoSpawn(densityExpert, elapsedTime);
            foreach (var crystal in newies)
            {
                Console.WriteLine("added " + crystal.GetPosition().X + "  " + crystal.GetPosition().Y);
            }
            return newies;
        }
    }
}
