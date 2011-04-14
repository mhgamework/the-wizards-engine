using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Cogs
{
    public class BarCog : ICogComponent
    {
        public Actor Actor { get; set; }
        public void PlaceAtHit(RaycastHit hit)
        {
        }

        public void IncreaseSize()
        {
        }

        public void DecreaseSize()
        {
        }

        public bool ContainsShape(Shape shape)
        {
            throw new NotImplementedException();
        }

        public void DisposeActors()
        {
            Actor.Dispose();
        }

        public BarCog(PhysicsEngine engine)
        {
            var actorDesc = new ActorDescription();

            actorDesc.BodyDescription = new BodyDescription(100);

            var boxShapeDesc = new BoxShapeDescription(100, 1f, 1f);
            actorDesc.Shapes.Add(boxShapeDesc);


            for (float i = -50; i < 51; i += 2)
            {
                Cog.AddCogToothShapes(actorDesc, new Vector3(i, 0, 0.8f), Vector3.UnitZ, Vector3.UnitY);
            }


            Actor = engine.Scene.CreateActor(actorDesc);


        }
    }
}
