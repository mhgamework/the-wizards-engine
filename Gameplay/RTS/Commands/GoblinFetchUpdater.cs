using System;
using SlimDX;
using System.Linq;
namespace MHGameWork.TheWizards.RTS.Commands
{
    public class GoblinFetchUpdater
    {
        private Vector3 targetPosition;
        private ResourceType resourceType;
        private Goblin goblin;
        private float dropAreaRange = 0.1f;

        public void Update(Goblin goblin, Vector3 targetPosition, ResourceType resourceType)
        {
            this.goblin = goblin;
            this.resourceType = resourceType;
            this.targetPosition = targetPosition;
            if (this.goblin.IsHoldingResource(this.resourceType))
            {
                goblin.MoveTo(targetPosition);
                if (reachedTarget(targetPosition))
                {
                    this.goblin.DropHolding();
                }
            }
            else
            {
                var res = findClosestResource(dropAreaRange);
                if (res == null) return;

                var closest = res.Position;

                goblin.MoveTo(closest);
                if (reachedTarget(closest))
                {
                    pickupResource(this.goblin);
                }
            }
        }

        private void pickupResource(Goblin goblin)
        {
            var res = findClosestResource(0);
            if (res == null) return;
            TW.Data.Objects.Remove(res);
            goblin.Holding = res.Thing;
        }

        private DroppedThing findClosestResource(float minDist)
        {
            var closest = TW.Data.Objects.Where(o => o is DroppedThing)
              .Cast<DroppedThing>().Where(o => o.Thing.Type == resourceType)
              .Where(o => Vector3.DistanceSquared(o.Position , targetPosition) >minDist*minDist)
              .OrderBy(o => (o.Position - goblin.Position).LengthSquared())
              .FirstOrDefault();

            return closest;

        }

        private bool reachedTarget(Vector3 target)
        {
            return (goblin.Position - target).Length() < dropAreaRange;
        }
    }
}
