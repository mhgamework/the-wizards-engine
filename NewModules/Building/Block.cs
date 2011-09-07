using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    public class Block
    {
        private readonly DeferredRenderer renderer;
        private readonly IMesh mesh;
        public Point3 Position { get; private set; }

        public Block(DeferredRenderer renderer, IMesh mesh, Point3 position)
        {
            this.renderer = renderer;
            this.mesh = mesh;
            Position = position;
            var meshEl = renderer.CreateMeshElement(mesh);
            meshEl.WorldMatrix = Matrix.Translation(position+ new Vector3(0.5f,0,0.5f));
            
        }
    }
}
