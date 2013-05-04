using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Items;

namespace MHGameWork.TheWizards.RTSTestCase1
{
    [ModelObjectChanged]
    public class CommandFactory : EngineModelObject
    {
        public CommandFactory()
        {
            Follow = new GoblinCommandType { Name = "Follow", MeshPath = @"RTS\Commands\export\follow" };
            Cancel = new GoblinCommandType { Name = "Cancel", MeshPath = @"RTS\Commands\export\cancel" };
            Defend = new GoblinCommandType { Name = "Defend", MeshPath = @"RTS\Commands\export\defend" };
        }
        public GoblinCommandType Follow { get; set; }
        public GoblinCommandType Cancel { get; set; }
        public GoblinCommandType Defend { get; set; }

        public IEnumerable<GoblinCommandType> AllCommands()
        {
            yield return Follow;
            yield return Cancel;
            yield return Defend;
        }


    }
}