using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Cogs
{
    public class Cog : ICogComponent
    {
        public CogEngine CogEngine { get; set; }
        public PhysicsEngine Engine { get; set; }
        protected Actor Holder { get; set; }

        private Joint joint;

        public static void AddCogToothShapes(ActorDescription actorDesc, Vector3 pos, Vector3 normal, Vector3 tangent)
        {
            SphereShapeDescription desc = new SphereShapeDescription(0.6f);

            desc.LocalPosition = pos;
            actorDesc.Shapes.Add(desc);

            desc = new SphereShapeDescription(0.2f);
            desc.LocalPosition = pos + 0.6f * normal;
            actorDesc.Shapes.Add(desc);
        }

        public Cog(CogEngine cogEngine, PhysicsEngine engine)
        {
            CogEngine = cogEngine;
            Engine = engine;

            Actor holder;
            Actor = CogEngine.CreateCogHolder(Engine, out holder);
            Holder = holder;
            holder.MoveGlobalOrientationTo(Matrix.CreateRotationX(MathHelper.PiOver2) *
                                                   Matrix.CreateTranslation(Vector3.UnitX * 30 + Vector3.UnitY * 20));
            holder.RaiseActorFlag(ActorFlag.DisableCollision);
        }


        public Actor Actor { get; set; }
        public void PlaceAtHit(RaycastHit hit)
        {
            Matrix mat;



            mat = Matrix.Identity;
            CreateRotationFromVectors(hit.WorldNormal, Vector3.UnitY, out mat);
            mat *= Matrix.CreateTranslation(hit.WorldImpact + hit.WorldNormal * 1);
            Holder.MoveGlobalPoseTo(mat);


            mat = Matrix.Identity;
            CreateRotationFromVectors(hit.WorldNormal, -Vector3.UnitZ, out mat);
            mat *= Matrix.CreateTranslation(hit.WorldImpact + hit.WorldNormal * 1);

            //Actor.MoveGlobalPoseTo(mat);
            //Actor.GlobalPose = mat;
        }

        public void IncreaseSize()
        {
        }

        public void DecreaseSize()
        {
        }


        public static void CreateRotationFromVectors(Vector3 v1, Vector3 v2, out Matrix matrix)
        {
            float cosTheta;

            Vector3 v1n = v1;
            v1n.Normalize();
            Vector3 v2n = v2;
            v2n.Normalize();

            Vector3.Dot(ref v1n, ref v2n, out cosTheta);
            Vector3 axis;
            Vector3.Cross(ref v1n, ref v2n, out axis);
            Matrix.CreateFromAxisAngle(ref axis, (float)Math.Acos(cosTheta), out matrix);
        }

        public bool ContainsShape(Shape shape)
        {
            return shape.Actor == Holder || shape.Actor == Actor;
        }

        public void DisposeActors()
        {
            Actor.Dispose();
            Holder.Dispose();
        }
    }
}
