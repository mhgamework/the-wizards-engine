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
            densityExpert.ResetDensities(crystals.Cast<SimpleCrystal>());
            crystalSpawner.DoSpawn(densityExpert, elapsedTime);
            drawSim.Simulate();
        }

        private void SimulateAmbientCharging(IEnumerable<ICrystal> crystals, float elapsedTime)
        {
            ambientCharger.ChargeAllCrystals(crystals, elapsedTime);
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
    }
}
