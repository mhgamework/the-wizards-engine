using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Pickupping;
using SlimDX;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    [ModelObjectChanged]
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

        /// <summary>
        /// Updates correct placement of the orb when it is in a holder
        /// </summary>
        public void UpdateInHolder()
        {
            if (CurrentHolder == null) return;

            var index = CurrentHolder.CommandHolder.AssignedCommands.IndexOf(this);

            var pos = CurrentHolder.CommandHolder.HoldingArea.RelativeStart
                      + CurrentHolder.CommandHolder.HoldingArea.Direction*index*0.6f;

            pos += CurrentHolder.Physical.WorldMatrix.xna().Translation.dx();

            Physical.WorldMatrix = Matrix.Translation(pos);
        }
        public ICommandHolder CurrentHolder { get; set; }
    }
}
