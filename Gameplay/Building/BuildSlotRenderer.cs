using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Rendering.Deferred;
using MHGameWork.TheWizards.ServerClient.Entity.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    public class BuildSlotRenderer
    {
        private readonly ModelContainer.ModelContainer world;
        private readonly DeferredRenderer renderer;
        private Dictionary<BuildSlot, DeferredMeshRenderElement> BuildSlotRenderDataMap = new Dictionary<BuildSlot, DeferredMeshRenderElement>();
        public BuildSlotRenderer(ModelContainer.ModelContainer world, DeferredRenderer renderer)
        {
            this.world = world;
            this.renderer = renderer;
        }

        public void ProcessWorldChanges()
        {
            int length;
            ModelContainer.ModelContainer.ObjectChange[] objectChanges;
            world.GetEntityChanges(out objectChanges, out length);


            for (int i = 0; i < length; i++)
            {
                var change = objectChanges[i];


                if (!(change.ModelObject is BuildSlot))
                    continue;

                var ent = (BuildSlot)change.ModelObject;

                switch (change.Change)
                {
                    case ModelChange.None:
                        throw new InvalidOperationException();
                    case ModelChange.Added:
                        setMeshRenderElement(ent);
                        break;
                    case ModelChange.Modified:
                        setMeshRenderElement(ent);
                        break;
                    case ModelChange.Removed:
                        DeferredMeshRenderElement meshEl = BuildSlotRenderDataMap[ent];
                        if (meshEl != null)
                        {
                            BuildSlotRenderDataMap.Remove(ent);
                            meshEl.Delete();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

        private void setMeshRenderElement(BuildSlot slot)
        {
            DeferredMeshRenderElement meshEl = null;

            if (BuildSlotRenderDataMap.ContainsKey(slot))
            {
                meshEl = BuildSlotRenderDataMap[slot];

            }
            if (meshEl != null)
            {
                BuildSlotRenderDataMap.Remove(slot);
                meshEl.Delete();
            }
            if (slot.BuildUnit != null)
            {
                meshEl = renderer.CreateMeshElement(slot.BuildUnit.Mesh);
                meshEl.WorldMatrix = Matrix.Scaling(slot.Scaling) * Matrix.RotationY(slot.RelativeRotationY) * Matrix.Translation(slot.RelativeTranslation) *
                                     Matrix.Translation(slot.Block.Position);
                BuildSlotRenderDataMap.Add(slot, meshEl);
            }

        }
    }
}



