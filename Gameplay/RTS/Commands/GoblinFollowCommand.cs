using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.RTS.Commands
{
    public class GoblinFollowCommand : IGoblinCommand
    {
        private GoblinFollowUpdater updater;

        public GoblinFollowCommand(GoblinFollowUpdater updater)
        {
            this.updater = updater;
        }

        public void Update(Goblin goblin)
        {
            updater.Update(goblin);
        }

        public string Description { get { return "following my master."; } }
    }
}
