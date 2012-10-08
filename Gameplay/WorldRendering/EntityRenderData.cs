using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.WorldRendering
{
    public class EntityRenderData
    {
        private readonly WorldRendering.Entity entity;
        private readonly DeferredRenderer renderer;

        private DeferredMeshRenderElement element;

        public EntityRenderData(WorldRendering.Entity entity, DeferredRenderer renderer)
        {
            this.entity = entity;
            this.renderer = renderer;
        }

        public void UpdateRenderData()
        {
            createElement();
            updateElement();
        }

        private void createElement()
        {
            if (element != null && element.Mesh == entity.Mesh) return; // already ok
            RemoveRenderData();
            if (entity.Mesh == null) return;
            element = renderer.CreateMeshElement(entity.Mesh);
        }

        private void updateElement()
        {
            if (element == null) return;
            element.WorldMatrix = entity.WorldMatrix;
            element.Visible = entity.Visible && !entity.Batched;
        }

        public void RemoveRenderData()
        {
            if (element != null)
                element.Delete();
            element = null;
        }
    }
}
