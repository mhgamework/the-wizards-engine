using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    [ModelObjectChanged]
    public class Tree : EngineModelObject, IWorldResource, IPhysical
    {
        public Tree()
        {
            Physical = new Physical();
        }

        public float Size { get; set; }
        public Vector3 Position { get; set; }


        public Vector3 GenerationPoint { get { return Position + Vector3.UnitY * 0.3f + Vector3.UnitX * 1f; } }
        public Vector3 OutputDirection { get { return new Vector3(0, 0, 1); } }
        public Thing TakeResource()
        {
            if (!ResourceAvailable) throw new InvalidOperationException();
            Size -= 1;
            return new Thing() { Type = TW.Data.Get<ResourceFactory>().Wood };
        }

        public bool ResourceAvailable { get { return Size > 1; } }
        public void Grow(float elapsed)
        {
            Size += elapsed / 10; // is the growth rate
            if (Size > 3) Size = 3;
        }

        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            var entity = Physical;
            var tree = this;

            entity.Solid = true;
            entity.Static = true;
            entity.Mesh = TW.Assets.LoadMesh("RTS\\Tree");
            entity.WorldMatrix = Matrix.Scaling(1 + tree.Size * 0.1f, 1 + tree.Size * 0.1f, 1 + tree.Size * 0.1f) * Matrix.Scaling(1f, 3, 1f) * Matrix.Translation(tree.Position);
        }



    }
}
