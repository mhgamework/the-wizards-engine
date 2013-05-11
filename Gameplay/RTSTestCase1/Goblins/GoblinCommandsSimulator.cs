using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// Responsible for simulating goblin commands behaviour
    /// Also simulates the command orbs
    /// </summary>
    public class GoblinCommandsSimulator : ISimulator
    {
        public void Simulate()
        {
            updateGoblinsProvidingCommands();

            assignHoldersToOrbs();
            updateOrbsInHolders();
            }

        private static void updateOrbsInHolders()
        {
            foreach (var g in TW.Data.Objects.Where(o => o is GoblinCommandOrb).Cast<GoblinCommandOrb>().ToArray())
            {
                g.UpdateInHolder();
            }



            doBehaviour();

        }

        private static void updateGoblinsProvidingCommands()
        {
            foreach (var g in TW.Data.Objects.Where(o => o is Goblin).Cast<Goblin>().ToArray())
            {
                g.Commands.UpdateShowingCommands();
            }
        }
        private void doBehaviour()
        {
            foreach (var g in TW.Data.Objects.Where(o => o is Goblin).Cast<Goblin>().ToArray())
            {
                if (g.Commands.ShowingCommands) continue;
                if (g.Commands.Orbs.Count == 0) continue;

                var f = new GoblinFollowBehaviour();

                f.Update(g);

                var toGoal = -(g.Physical.WorldMatrix.xna().Translation.dx() - g.Goal);
                toGoal.Normalize();
                toGoal = toGoal*2;

                g.Physical.WorldMatrix = g.Physical.WorldMatrix*Matrix.Translation(toGoal*TW.Graphics.Elapsed);
                Console.WriteLine(g.Goal);

            }
        }

        private void assignHoldersToOrbs()
        {
            var map = new Dictionary<GoblinCommandOrb, ICommandHolder>();

            foreach (var g in TW.Data.Objects.Where(o => o is CommandHolderPart).Cast<CommandHolderPart>().ToArray())
            {
                foreach (var o in g.AssignedCommands)
                    map[o] = g.Holder;
            }

            foreach (var o in map.Keys)
            {
                o.CurrentHolder = map[o];
            }
        }
    }
}
