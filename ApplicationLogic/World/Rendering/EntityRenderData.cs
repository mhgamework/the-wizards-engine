using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.World.Rendering
{
    public class EntityRenderData
    {
        private readonly Entity entity;
        private readonly DeferredRenderer renderer;

        private DeferredMeshRenderElement element;

        public EntityRenderData(Entity entity, DeferredRenderer renderer)
        {
            this.entity = entity;
            this.renderer = renderer;
        }

        public void UpdateRenderData()
        {
            bool create = false;

            if (element == null || element.Mesh == entity.Mesh) create = true;

            if (create)
            {
                RemoveRenderData();
                element = renderer.CreateMeshElement(entity.Mesh);
            }

            element.WorldMatrix = entity.WorldMatrix;


        }

        public void RemoveRenderData()
        {
            if (element != null)
                element.Delete();
            element = null;
        }
    }
}
