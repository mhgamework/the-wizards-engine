using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Pickupping;
using MHGameWork.TheWizards.RTSTestCase1._Engine;
using SlimDX;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    public class GoblinCommandOrb : EngineModelObject, IPhysical
    {
        public Physical Physical { get; set; }

        public GoblinCommandOrb()
        {
            Physical = new Physical();
        }

        public GoblinCommandType Type { get; set; }

        public void UpdatePhysical()
        {
            var entity = Physical;
            entity.Mesh = TW.Assets.LoadMesh(Type.MeshPath);
            entity.Solid = false;
            entity.Static = false;

            entity.ObjectMatrix = Matrix.Scaling(0.5f, 0.5f, 0.5f);
        }
    }
}
