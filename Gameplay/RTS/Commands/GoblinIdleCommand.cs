using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.RTS.Commands
{
    public class GoblinIdleCommand : IGoblinCommand
    {
        public void Update(Goblin goblin)
        {
            // Contemplating the universe
            goblin.Goal = goblin.Position;
        }

        public string Description { get { return "contemplating the universe."; } }
    }
}
