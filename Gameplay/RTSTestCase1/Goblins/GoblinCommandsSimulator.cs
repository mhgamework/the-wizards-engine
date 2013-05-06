using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// Responsible for simulating goblin commands behaviour
    /// Also simulates the command orbs
    /// </summary>
    public class GoblinCommandsSimulator :ISimulator
    {
        public void Simulate()
        {
            foreach (var g in TW.Data.Objects.Where(o => o is Goblin).Cast<Goblin>().ToArray())
            {
                g.Commands.UpdateShowingCommands();
            }

            assignHoldersToOrbs();
            foreach (var g in TW.Data.Objects.Where(o => o is GoblinCommandOrb).Cast<GoblinCommandOrb>().ToArray())
            {
                g.UpdateInHolder();
            }
        }

        private void assignHoldersToOrbs()
        {
            var map = new Dictionary<GoblinCommandOrb, ICommandHolder>();

            foreach (var g in TW.Data.Objects.Where(o => o is ICommandHolder).Cast<ICommandHolder>().ToArray())
            {
                foreach (var o in g.CommandHolder.AssignedCommands)
                    map[o] = g;
            }

            foreach (var o in map.Keys)
            {
                o.CurrentHolder = map[o];
            }
        }
    }
}
