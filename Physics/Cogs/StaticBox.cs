using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Cogs
{
    public class StaticBox : ICogComponent
    {
        public PhysicsEngine Engine { get; set; }
        public Actor Actor { get; set; }

        private float size;

        public StaticBox(PhysicsEngine engine)
        {
            Engine = engine;
            size = 2;
            Actor = engine.Scene.CreateActor(new ActorDescription(new BoxShapeDescription(size, size, size)));
        }

        public void IncreaseSize()
        {

        }
        public void DecreaseSize()
        {

        }

        public bool ContainsShape(Shape shape)
        {
            return shape.Actor == Actor;
        }

        public void DisposeActors()
        {
            Actor.Dispose();
        }

        public void PlaceAtHit(RaycastHit hit)
        {
            Vector3 pos = hit.WorldImpact + hit.WorldNormal * size * 0.5f;
            pos.X -= pos.X % size;
            pos.Y -= pos.Y % size;
            pos.Z -= pos.Z % size;
            Actor.GlobalPosition = pos;
        }
    }
}
