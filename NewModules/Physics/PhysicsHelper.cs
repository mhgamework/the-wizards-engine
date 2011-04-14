using System;
using System.Collections.Generic;
using System.Text;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Physics
{
    public static class PhysicsHelper
    {
        public static Actor CreateDynamicSphereActor(Scene scene, float radius, float mass)
        {
            ActorDescription actorDesc = new ActorDescription(new SphereShapeDescription(radius));
            actorDesc.BodyDescription = new BodyDescription(mass);

            return scene.CreateActor(actorDesc);
        }
    }
}
