using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.World
{
    public class WorldPhysxSyncActor : IWorldSyncActor
    {
        public Actor Actor { get; private set; }

        public WorldPhysxSyncActor(Actor actor)
        {
            Actor = actor;
        }

        public Vector3 GlobalPosition
        {
            get { return Actor.GlobalPosition; }
            set { Actor.GlobalPosition = value; }
        }

        public Matrix GlobalOrientation
        {
            get { return Actor.GlobalOrientation; }
            set { Actor.GlobalOrientation = value; }
        }
    }
}
