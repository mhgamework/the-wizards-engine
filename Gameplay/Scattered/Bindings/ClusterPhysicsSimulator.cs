﻿using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Model;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Scattered._Engine;

namespace MHGameWork.TheWizards.Scattered.Bindings
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