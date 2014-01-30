using System.Drawing;
using Castle.Core.Internal;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    public class InterIslandMovementSimulator : ISimulator
    {
        private readonly Level level;

        public InterIslandMovementSimulator(Level level)
        {
            this.level = level;
        }

        public void Simulate()
        {
            level.Travellers.ForEach(stepMovement);
        }

        private void stepMovement(Traveller obj)
        {
            if (obj.Destination == null) return;
            if (obj.IsAtIsland(obj.Destination))
            {
                onReachedDestination(obj);
                return;
            }
            var v = obj.BridgePosition;
            v.Percentage = MathHelper.Clamp(v.Percentage + TW.Graphics.Elapsed * 0.2f, 0, 1);
            v.End = obj.Destination;
            obj.BridgePosition = v;

        }

        private void onReachedDestination(Traveller traveller)
        {
            traveller.Destination = traveller.DetermineDestinationAction();
        }

    }
}