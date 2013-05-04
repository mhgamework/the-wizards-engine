using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// Responsible for simulating goblin commands behaviour
    /// </summary>
    public class GoblinCommandsSimulator :ISimulator
    {
        public void Simulate()
        {
            foreach (var g in TW.Data.Objects.Where(o => o is Goblin).Cast<Goblin>().ToArray())
            {
                g.Commands.UpdateShowingCommands();
            }
        }
    }
}
