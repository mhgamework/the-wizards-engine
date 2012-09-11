using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Trigger
{
    public class PlayerPositionCondition : ICondition
    {
        private readonly PlayerData player;
        private readonly BoundingBox bb;

        public PlayerPositionCondition(PlayerData player, BoundingBox bb)
        {
            this.player = player;
            this.bb = bb;

            var box = new WireframeBox();
            box.Color = new Color4(1, 1, 1);
            box.FromBoundingBox(bb);
        }

        public bool IsSatisfied()
        {
            var pPos = player.Position;

            if (pPos.X >= bb.Minimum.X && pPos.X <= bb.Maximum.X && pPos.Y >= bb.Minimum.Y && pPos.Y <= bb.Maximum.Y && pPos.Z >= bb.Minimum.Z && pPos.Z <= bb.Maximum.Z)
                return true;
            return false;
        }
    }
}
