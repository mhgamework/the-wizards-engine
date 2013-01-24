﻿using System;
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
                var res = findClosestResource(allowedToPickup);
                if (res == null) return;

                var closest = res.get<Engine.WorldRendering.Entity>().WorldMatrix.xna().Translation.dx();

                goblin.MoveTo(closest);
                if (reachedTarget(closest))
                {
                    pickupResource(this.goblin);
                }
            }
        }

        private void pickupResource(Goblin goblin)
        {
            var res = findClosestResource(o => true);
            if (res == null) return;
            TW.Data.Objects.Remove(res);
            goblin.Holding = res.Thing;
        }

        private DroppedThing findClosestResource(Func<DroppedThing, bool> toPickup)
        {
            var closest = TW.Data.Objects.Where(o => o is DroppedThing)
              .Cast<DroppedThing>().Where(o => o.Thing.Type == resourceType)
              .Where(toPickup)
              .OrderBy(t => getDistanceSqToDrop(goblin.Position, t))
              .FirstOrDefault();

            return closest;

        }

        private bool allowedToPickup(DroppedThing drop)
        {
            return getDistanceSqToDrop(targetPosition, drop) > dropAreaRange * dropAreaRange;
        }

        private float getDistanceSqToDrop(Vector3 pos, DroppedThing drop)
        {
            var droppos = drop.get<Engine.WorldRendering.Entity>().WorldMatrix.xna().Translation.dx();
            var target = pos;
            droppos.Y = 0;
            target.Y = 0;
            return Vector3.DistanceSquared(droppos, target);
        }

        private bool reachedTarget(Vector3 target)
        {
            return (goblin.Position - target).Length() < 0.01f;
        }
    }
}