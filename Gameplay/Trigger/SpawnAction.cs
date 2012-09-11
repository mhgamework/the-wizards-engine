using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Trigger
{
    public class SpawnAction : IAction
    {
        private readonly Matrix matrix;
        private readonly IMesh mesh;

        public SpawnAction(Matrix matrix, IMesh mesh)
        {
            this.matrix = matrix;
            this.mesh = mesh;
        }

        public void Activate()
        {
            WorldRendering.Entity ent = new WorldRendering.Entity();
            ent.Mesh = mesh;
            ent.WorldMatrix = matrix;
            ent.Visible = true;
        }
    }
}
