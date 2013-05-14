using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic.Simulators
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
