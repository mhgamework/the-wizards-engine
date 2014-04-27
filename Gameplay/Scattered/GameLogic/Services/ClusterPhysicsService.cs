using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.Scattered.GameLogic.Services
{
    public class ClusterPhysicsService 
    {
        private readonly Level level;

        public ClusterPhysicsService(Level level)
        {
            this.level = level;
        }

        public void UpdateClusterMovement()
        {
            level.Islands.ForEach(stepMovement);
        }

        private void stepMovement(Island obj)
        {
            obj.Position += obj.Velocity * TW.Graphics.Elapsed;
            obj.Velocity -= obj.Velocity * (0.3f * TW.Graphics.Elapsed);
        }
    }
}