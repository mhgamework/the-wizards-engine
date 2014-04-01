using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Model;
using Castle.Core.Internal;

namespace MHGameWork.TheWizards.Scattered.Simulation.Playmode
{
    public class ClusterPhysicsSimulator : ISimulator
    {
        private readonly Level level;

        public ClusterPhysicsSimulator(Level level)
        {
            this.level = level;
        }

        public void Simulate()
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