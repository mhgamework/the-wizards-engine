using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;

namespace MHGameWork.TheWizards.RTSTestCase1.Rendering
{
    public class RTSRendererSimulator :ISimulator
    {
        public void Simulate()
        {
            TW.Data.EnsureAttachment<Tree,TreeRenderData>(o => new TreeRenderData(o));
            TW.Data.EnsureAttachment<Rock,RockRenderData>(o => new RockRenderData(o));
            foreach (Tree t in TW.Data.GetChangedObjects<Tree>()) t.get<TreeRenderData>().Update();
            foreach (Rock t in TW.Data.GetChangedObjects<Rock>()) t.get<RockRenderData>().Update();
        }
    }
}
