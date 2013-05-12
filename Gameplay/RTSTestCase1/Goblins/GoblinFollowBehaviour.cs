using System;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    public class GoblinFollowBehaviour : IGoblinBehaviour
    {
        public void Update(Goblin goblin)
        {
            var playerPos = getTargetPos(goblin);
            var toPlayer = goblin.Position - playerPos;
            var minDistance = 1;
            var goal = playerPos;
            if (toPlayer.Length() < minDistance)
                goal = goblin.Position;
            goal.Y = 0;
            goblin.MoveTo(goal);
        }

        private static Vector3 getTargetPos(Goblin goblin)
        {
            var orb = goblin.Commands.Orbs.FirstOrDefault(o => o.Type == TW.Data.Get<CommandFactory>().Follow);
            if (orb == null) throw new InvalidOperationException("Cannot perform this behaviour!");

            return orb.Physical.WorldMatrix.xna().Translation.dx();
        }
    }
}