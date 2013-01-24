using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTS.Commands
{
    [ModelObjectChanged]
    public class TalkingCommand : EngineModelObject, IGoblinCommand
    {

        public void Update(Goblin goblin)
        {
            // Talking
            goblin.Goal = goblin.Position;
        }

        public string Description { get { return "WTF YOU CANT KNOW IM TALKING!"; } }
    }
}
