using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic.Simulators
{
    public class CrystalInfoDrawSimulator :ISimulator
    {
        public void Simulate()
        {drawBarsOverCrystals();
        }
         private void drawBarsOverCrystals()
        {
            var crystalRenderData = TW.Data.Objects.OfType<SimpleCrystal>().Select(o => o.get<CrystalRenderData>());
            foreach (var crystal in crystalRenderData.Where(crystal => crystal != null))
            {
                crystal.RenderBar();
            }
        }
    }
}