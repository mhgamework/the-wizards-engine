using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.RTS.Commands
{
    public class TalkingCommand : IGoblinCommand
    {

        public void Update(Goblin goblin)
        {
            // Talking
            goblin.Goal = goblin.Position;
        }

        public string Description { get { return "WTF YOU CANT KNOW IM TALKING!"; } }
    }
}
