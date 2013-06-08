using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Magic;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;

namespace MHGameWork.TheWizards.RTSTestCase1.Rendering
{
    /// <summary>
    /// Binds entities to visualize, simulate physx for the objects in the RTSTestCase1
    /// </summary>
    public class RTSEntitySimulator :ISimulator
    {
        public void Simulate()
        {
            TW.Data.EnsureAttachment<Rock, RockRenderData>(o => new RockRenderData(o));
            foreach (Rock t in TW.Data.GetChangedObjects<Rock>()) t.get<RockRenderData>().Update();

            TW.Data.EnsureAttachment<SimpleCrystal, CrystalRenderData>(o => new CrystalRenderData(o));
            foreach (SimpleCrystal t in TW.Data.GetChangedObjects<SimpleCrystal>()) t.get<CrystalRenderData>().Update();

        }

    }

}
