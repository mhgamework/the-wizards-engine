using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    /// <summary>
    /// Responsible for ensuring Wireframebox is rendererd
    /// </summary>
    public class WireframeBoxSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var change in TW.Data.GetChangesOfType<WireframeBox>())
                updateWireframeBox(change);
        }


        private void updateWireframeBox(Data.ModelContainer.ObjectChange change)
        {
            var ent = (WireframeBox)change.ModelObject;

            if (change.Change == ModelChange.Added)
            {
                var el = TW.Graphics.AcquireRenderer().CreateLinesElement();

                ent.set(el);

                change.Change = ModelChange.Modified; // also modified :P
            }
            if (change.Change == ModelChange.Modified)
            {
                var el = ent.get<DeferredLinesElement>();
                el.Lines.ClearAllLines();

                if (ent.Visible)
                    el.Lines.AddAABB(new BoundingBox(MathHelper.One * -0.5f, MathHelper.One * 0.5f), ent.WorldMatrix, ent.Color);
            }
            if (change.Change == ModelChange.Removed)
            {
                ent.get<DeferredLinesElement>().Delete();
                ent.set<DeferredLinesElement>(null);
            }
        }
    }
}
