using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// Responsible for enabling a goblin to provide commands to the user
    /// </summary>
    public class GoblinCommandsPart : EngineModelObject
    {
        public GoblinCommandsPart()
        {
            ShowingCommands = false;
            Orbs = new List<GoblinCommandOrb>();
        }

        public Goblin Goblin { get; set; }
        public bool ShowingCommands { get; set; }
        public List<GoblinCommandOrb> Orbs { get; set; }

        /// <summary>
        /// Ensure correct display of commands a goblin provides to the player
        /// </summary>
        public void UpdateShowingCommands()
        {
            if (ShowingCommands)
                showOrbs();
            else
                hideOrbs();
        }

        private void showOrbs()
        {
            if (Orbs.Count == 0)
            {
                Orbs.Add(new GoblinCommandOrb { Type = TW.Data.Get<CommandFactory>().Follow });
                Orbs.Add(new GoblinCommandOrb { Type = TW.Data.Get<CommandFactory>().Defend });
            }

            var pos = Goblin.Physical.WorldMatrix.xna().Translation.dx() + new Vector3(0, 1, 0);
            foreach (var orb in Orbs)
            {
                orb.Physical.WorldMatrix = Matrix.Translation(pos);
                pos += new Vector3(0.75f, 0, 0);
            }
        }

        private void hideOrbs()
        {
            // remove all orbs for now
            foreach (var orb in Orbs)
            {
                orb.Physical.Visible = false;
                //TW.Data.RemoveObject(orb);
            }
            Orbs.Clear();
        }
    }
}