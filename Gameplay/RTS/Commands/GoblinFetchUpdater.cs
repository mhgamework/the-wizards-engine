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
        private float dropAreaRange = 1f;

        public void Update(Goblin goblin, Vector3 targetPosition,ResourceType resourceType)
        {
            Update(goblin,targetPosition,10000000f,targetPosition,resourceType);
        }

        public void Update(Goblin goblin, Vector3 sourcePosition, float sourceRangeSquared, Vector3 targetPosition, ResourceType resourceType)
        {
            this.goblin = goblin;
            this.resourceType = resourceType;
            this.targetPosition = targetPosition;
            if (this.goblin.IsHoldingResource(this.resourceType))
            {
                goblin.MoveTo(calculateCorrectDropPosition());
                if (Math.Abs( Vector3.Dot( goblin.HoldingEntity.WorldMatrix.xna().Translation.dx()-targetPosition ,new Vector3(1,0,1))) < 0.1f)
                {
                    this.goblin.DropHolding();
                }
            }
            else
            {
                var res = findClosestResource(allowedToPickup,sourceRangeSquared,sourcePosition);
                if (res == null) return;

                var closest = res.get<Engine.WorldRendering.Entity>().WorldMatrix.xna().Translation.dx();

                goblin.MoveTo(closest);
                if (reachedTarget(closest))
                {
                    pickupResource(this.goblin);
                }
            }
        }

        public Vector3 calculateCorrectDropPosition()
        {
            var dir = targetPosition - goblin.Position;
            var armLength = Vector3.Dot(goblin.CalculateHoldingResourcePosition() - goblin.Position,
                                        new Vector3(1, 0, 1));
            return targetPosition - dir*armLength;
        }

        public void pickupResource(Goblin goblin)
        {
            var res = findClosestResource(o => true,float.MaxValue,goblin.Position);
            if (res == null) return;
            TW.Data.Objects.Remove(res);
            goblin.Holding = res.Thing;
        }

        public DroppedThing findClosestResource(Func<DroppedThing, bool> toPickup,float rangeSquared,Vector3 sourcePosition)
        {
            var closest = TW.Data.Objects.Where(o => o is DroppedThing)
              .Cast<DroppedThing>().Where(o => o.Thing.Type == resourceType)
              .Where(toPickup).Where(t => getDistanceSqToDrop(sourcePosition,t) < rangeSquared)
              .OrderBy(t => getDistanceSqToDrop(goblin.Position, t))
              .FirstOrDefault();

            return closest;

        }

        public bool allowedToPickup(DroppedThing drop)
        {
            return getDistanceSqToDrop(targetPosition, drop) > dropAreaRange * dropAreaRange;
        }

        public float getDistanceSqToDrop(Vector3 pos, DroppedThing drop)
        {
            var droppos = drop.get<Engine.WorldRendering.Entity>().WorldMatrix.xna().Translation.dx();
            var target = pos;
            droppos.Y = 0;
            target.Y = 0;
            return Vector3.DistanceSquared(droppos, target);
        }

        public bool reachedTarget(Vector3 target)
        {
            return (goblin.Position - target).Length() < 0.01f;
        }
    }
}
