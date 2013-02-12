using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;

namespace MHGameWork.TheWizards.RTSTestCase1.Rendering
{
    public class RTSRendererSimulator :ISimulator
    {
        public void Simulate()
        {
            TW.Data.EnsureAttachment<Tree,TreeRenderData>(o => new TreeRenderData(o));
            foreach (Tree t in TW.Data.GetChangedObjects<Tree>()) t.get<TreeRenderData>().Update();

            TW.Data.EnsureAttachment<Rock, RockRenderData>(o => new RockRenderData(o));
            foreach (Rock t in TW.Data.GetChangedObjects<Rock>()) t.get<RockRenderData>().Update();

            TW.Data.EnsureAttachment<DroppedThing, DroppedThingRenderData>(o => new DroppedThingRenderData(o));
            foreach (DroppedThing t in TW.Data.GetChangedObjects<DroppedThing>()) t.get<DroppedThingRenderData>().Update();
        }

    }
}
