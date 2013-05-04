using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTS.Commands
{
    [ModelObjectChanged]
    public class GoblinFetchCommand : EngineModelObject, IGoblinCommand
    {
        private GoblinFetchUpdater updater = new GoblinFetchUpdater();

        public GoblinFetchCommand()
        {
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
