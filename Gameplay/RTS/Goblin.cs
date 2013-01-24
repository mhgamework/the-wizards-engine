using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS.Commands;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    [ModelObjectChanged]
    public class Goblin : EngineModelObject
    {
        public Vector3 Position { get; set; }
        public Vector3 Goal { get; set; }
        public Goblin BestFriend { get; set; }
        public Thing Holding { get; set; }


        public void MoveTo(Vector3 position)
        {
            Goal = position;
        }

        public void DropHolding()
        {
            var pos = Position;
            pos.Y = 0.5f;
            new DroppedThing() { Thing = Holding, InitialPosition = pos };

            Holding = null;
        }

        public bool IsHoldingResource(ResourceType type)
        {
            if (Holding == null) return false;
            return Holding.Type == type;
        }



        public Engine.WorldRendering.Entity GoblinEntity { get; set; }
        public Engine.WorldRendering.Entity HoldingEntity { get; set; }

    }
}
