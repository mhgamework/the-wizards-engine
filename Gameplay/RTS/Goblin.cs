using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    [ModelObjectChanged]
    public class Goblin : EngineModelObject

    {
        public Vector3 Position { get; set;}
        public Goblin BestFriend { get; set; }

        public void MoveTo(Vector3 position)
        {
            throw new NotImplementedException();
        }
    }
}
