using System;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    public class GoblinMoveSourceBehaviour : IGoblinBehaviour
    {
        private readonly IItemStorage storage;

        public GoblinMoveSourceBehaviour(IItemStorage storage)
        {
            this.storage = storage;
        }

        public void Update(Goblin goblin)
        {
            var playerPos = getTargetPos(goblin);

            goblin.Goal = playerPos;



            if (Vector3.Distance(playerPos, goblin.Physical.GetPosition()) > 0.5f) return; // Keep going

            // arrived!

            goblin.Goal = goblin.Physical.GetPosition();

            if (storage.ItemStorage.Items.Count == 0) return; // No items!

            var item = storage.ItemStorage.Items[storage.ItemStorage.Items.Count - 1];

            goblin.ItemStorage.Items.Add(item);
            storage.ItemStorage.Items.Remove(item);

        }

        private static Vector3 getTargetPos(Goblin goblin)
        {
            var orb = goblin.Commands.Orbs.FirstOrDefault(o => o.Type == TW.Data.Get<CommandFactory>().MoveSource);
            if (orb == null) throw new InvalidOperationException("Cannot perform this behaviour!");
            return orb.Physical.WorldMatrix.xna().Translation.dx();
        }
    }
}