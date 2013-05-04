using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;

namespace MHGameWork.TheWizards.RTS.Commands
{
    public class GoblinFollowUpdater
    {
        public void Update(Goblin goblin)
        {
            var playerPos = TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx();
            var toPlayer = goblin.Position - playerPos;
            var minDistance = 5;
            var goal = playerPos;
            if (toPlayer.Length() < minDistance)
                goal = goblin.Position;
            goal.Y = 0.85f / 2;
            goblin.MoveTo(goal);
        }
    }
}