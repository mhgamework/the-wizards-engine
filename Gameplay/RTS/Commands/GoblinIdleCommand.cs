using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTS.Commands
{
    [ModelObjectChanged]
    public class GoblinIdleCommand : EngineModelObject,IGoblinCommand
    {
        public void Update(Goblin goblin)
        {
            // Contemplating the universe
            goblin.Goal = goblin.Position;
        }

        public string Description { get { return "contemplating the universe."; } }
    }
}
