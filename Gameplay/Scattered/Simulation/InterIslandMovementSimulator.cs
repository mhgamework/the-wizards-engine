using System.Drawing;
using Castle.Core.Internal;
using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.Scattered.Model;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Simulation
{
    public class InterIslandMovementSimulator : ISimulator
    {
        private readonly Level level;
        private readonly IPathFinder2D<Island> pathFinder;

        public InterIslandMovementSimulator(Level level, IPathFinder2D<Island> pathFinder)
        {
            this.level = level;
            this.pathFinder = pathFinder;
        }

        public void Simulate()
        {
            level.Travellers.ForEach(stepMovement);

            // Cleanup dead travellers
            level.Travellers.Where(t => t.Destination == null).ToArray().ForEach(t => level.RemoveTraveller(t));
        }

        private void stepMovement(Traveller obj)
        {
            if (obj.Destination == null) return;

            // If at destination try to create a new path
            if (obj.IsAtIsland(obj.Destination))
            {
                if (!tryPlanNewPath(obj)) return;

                // Set initial movement position
                obj.BridgePosition = new BridgePosition(obj.PlannedPath[0], obj.PlannedPath[1], 0);
            }
                


            if (obj.IsAtIsland(obj.PlannedPath[1]))
            {
                // Arrived at next node, shorten path
                obj.PlannedPath = obj.PlannedPath.Skip(1).ToArray();
                obj.BridgePosition = new BridgePosition(obj.PlannedPath[0], obj.PlannedPath[1], 0);
            }


            var v = obj.BridgePosition;

            var length = Vector3.Distance(v.Start.Position, v.End.Position);
            var movementSpeed = 10; // in m/s
            var speed = movementSpeed / length;

            v.Percentage = MathHelper.Clamp(v.Percentage + TW.Graphics.Elapsed * speed, 0, 1);
      
            obj.BridgePosition = v;


        }

        private bool tryPlanNewPath(Traveller obj)
        {
            var pos = obj.Destination;
            // Look for new destination
            var dest = obj.DetermineDestinationAction();

            if (dest == null)
            {
                // No more paths required
                obj.PlannedPath = null;
                return false;
            }
            // If still at destination, we wait
            if (obj.IsAtIsland(dest))
            {
                obj.PlannedPath = new Island[] { pos };
                return false;
            }

            // Find a path to the new destination
            obj.PlannedPath = pathFinder.FindPath(pos, dest).ToArray();

            return true;
        }
    }
}