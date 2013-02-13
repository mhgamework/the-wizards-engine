﻿using SlimDX;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.RTSTestCase1.Pickupping
{
    public class PickupPhysXUpdater
    {
        public void Update(IPickupObject obj)
        {
            if (obj.GetHoldingActor() == null) return;
            Actor actor = obj.GetHoldingActor();

            if (isToFar(obj, actor))
            {
                obj.DropHolding();
                return;
            }

            applyHoldingForce(obj, actor);


        }

        private void applyHoldingForce(IPickupObject pickupObject, Actor actor)
        {
            var diff = pickupObject.GetHoldingPosition() - actor.GlobalPosition.dx();

            //if (actor.LinearVelocity.Length() < 0.5)
            actor.AddForce(diff.xna() * TW.Graphics.Elapsed * 500, ForceMode.Impulse);
            actor.AddForce(-actor.LinearVelocity*10, ForceMode.Force);

        }

        private bool isToFar(IPickupObject pickupObject, Actor actor)
        {
            var diff = pickupObject.GetHoldingPosition() - actor.GlobalPosition.dx();
            return diff.Length() > 1;
        }
    }
}