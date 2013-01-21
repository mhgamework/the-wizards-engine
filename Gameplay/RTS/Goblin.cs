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
        public Vector3 Position { get; set;}
        public Vector3 Goal { get; set; }
        public Goblin BestFriend { get; set; }
        public Thing Holding { get; set; }


        public void MoveTo(Vector3 position)
        {
            Goal = position;
        }

        public void DropHolding()
        {
            new DroppedThing() { Thing = Holding, Position = Position  };

            Holding = null;
        }

        public bool IsHoldingResource(ResourceType type)
        {
            if (Holding == null) return false;
            return Holding.Type == type;
        }

    }
}
