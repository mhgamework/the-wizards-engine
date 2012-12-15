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
        private Engine.WorldRendering.Entity ent;

        public SpawnAction(Matrix matrix, IMesh mesh)
        {
            ent = new Engine.WorldRendering.Entity();
            ent.Mesh = mesh;
            ent.WorldMatrix = matrix;
            ent.Visible = false;
        }

        public void Activate()
        {
            ent.Visible = true;
        }

        public void Reset()
        {
            ent.Visible = false;
        }
    }
}
