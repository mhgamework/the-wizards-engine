using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Rendering
{
    public class RockRenderData : IModelObjectAddon<Rock>
    {
        private readonly Rock self;
        private Entity entity;

        public RockRenderData(Rock self)
        {
            this.self = self;
            entity = new Entity();
        }

        public void Update()
        {
            entity.Mesh = TW.Assets.LoadMesh("RTS\\Rock");
            var size = 5;

            float height = -size;
            height += 1f;
            height += self.Height * 0.1f;

            entity.WorldMatrix = Matrix.Scaling(size, size, size) * Matrix.Translation(self.Position + height * Vector3.UnitY);
        }
        public void Dispose()
        {

        }
    }
}
