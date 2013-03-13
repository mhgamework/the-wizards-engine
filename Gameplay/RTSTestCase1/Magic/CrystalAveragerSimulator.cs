using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Magic;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.MagicSimulation
{
    class CrystalAveragerSimulator: ISimulator
    {
        private CrystalAverager averager = new CrystalAverager();
        
        public void Simulate()
        {
            
            var crystals = (IEnumerable<ICrystal>)TW.Data.Objects.Where(o => o is ICrystal);
            var elapsedTime = TW.Graphics.Elapsed;
            foreach (var crystal in crystals)
            {
                averager.processCrystal(crystals, crystal,elapsedTime);
            }
        }

    }
}
