using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTS.Commands
{
    [ModelObjectChanged]
    public class GoblinFollowCommand : EngineModelObject, IGoblinCommand
    {
        private GoblinFollowUpdater updater = new GoblinFollowUpdater();

        public void Update(Goblin goblin)
        {
            updater.Update(goblin);
        }

        public string Description { get { return "following my master."; } }
    }
}
