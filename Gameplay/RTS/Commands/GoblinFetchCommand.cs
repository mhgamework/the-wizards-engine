using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.RTS.Commands
{
    public class GoblinFetchCommand : IGoblinCommand
    {
        private GoblinFetchUpdater updater;

        public GoblinFetchCommand(GoblinFetchUpdater updater)
        {
            this.updater = updater;
        }

        public void Update(Goblin goblin)
        {
            updater.Update(goblin, TargetPosition, ResourceType);
        }

        public string Description { get { return "collecting, because thats what barrels do."; } }

        public Vector3 TargetPosition { get; set; }

        public ResourceType ResourceType { get; set; }
    }
}
