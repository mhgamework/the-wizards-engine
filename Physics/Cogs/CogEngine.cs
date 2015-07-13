using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;
using Ray = StillDesign.PhysX.Ray;

namespace MHGameWork.TheWizards.Cogs
{
    public class CogEngine : IXNAObject
    {
        public XNAGame Game { get; set; }
        public PhysicsEngine Engine { get; set; }

        public enum PlaceMode
        {
            None = 0,
            Cog = 1,
            BarCog = 2,
            StaticBox = 9

        }

        PlaceMode placeMode = PlaceMode.Cog;

        Actor actor = null;

        Actor ghostCogHolder = null;

        ICogComponent ghostComponent = null;
        CogRaycastReport raycastReport = null;

        private List<ICogComponent> components = new List<ICogComponent>();



        public CogEngine(XNAGame game, PhysicsEngine engine)
        {
            Game = game;
            Engine = engine;

            raycastReport = new CogRaycastReport(this);
        }


        public void Initialize(IXNAGame _game)
        {
            Engine.Initialize();

            PhysicsDebugRendererXNA debugRenderer = new PhysicsDebugRendererXNA(Game, Engine.Scene);

            Game.AddXNAObject(debugRenderer);


            Actor holderActor;
            actor = CreateCogHolder(Engine, out holderActor);


            var bar = new BarCog(Engine);

            bar.Actor.GlobalPosition = Vector3.Up * 2 + Vector3.UnitZ * -10;

        }

        public void Render(IXNAGame _game)
        {
        }

        public void Update(IXNAGame _game)
        {
            if (Game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
            {
                var bol = PhysicsHelper.CreateDynamicSphereActor(Engine.Scene, 1, 1);
                bol.GlobalPosition = Game.SpectaterCamera.CameraPosition +
                                       Game.SpectaterCamera.CameraDirection * 5;
                bol.LinearVelocity = Game.SpectaterCamera.CameraDirection * 5;
            }


            Plane p = new Plane(Vector3.Up, 0);
            var ray = Game.GetWereldViewRay(new Vector2(Game.Window.ClientBounds.Width * 0.5f,
                                              Game.Window.ClientBounds.Height * 0.5f));

            ray.Direction.Normalize();
            Ray worldRay = new Ray(ray.Position, ray.Direction);
            //worldRay.dir.normalize(); //Important!!

            //var hit = Engine.Scene.RaycastClosestShape(worldRay, ShapesType.All, uint.MaxValue, 1000, uint.MaxValue, null);
            var numHits = Engine.Scene.RaycastAllShapes(worldRay, raycastReport, ShapesType.All);

            if (numHits != raycastReport.Hits.Count)
                throw new InvalidOperationException("I dont seem to understand how this works");

            RaycastHit closestHit;
            closestHit = new RaycastHit();
            closestHit.Distance = float.PositiveInfinity;
            closestHit.Shape = null;

            for (int i = 0; i < raycastReport.Hits.Count; i++)
            {
                var hit = raycastReport.Hits[i];
                if (ghostComponent != null)
                    if (ghostComponent.ContainsShape(hit.Shape)) continue;

                if (hit.Distance < closestHit.Distance && hit.Distance < 1000) closestHit = hit;

            }

            raycastReport.Hits.Clear();

            if (Game.Keyboard.IsKeyDown(Keys.C))
            {
                if (closestHit.Shape != null)
                    if (ghostComponent != null)
                        ghostComponent.PlaceAtHit(closestHit);
            }


            if (Game.Keyboard.IsKeyPressed(Keys.X))
            {

                ICogComponent comp = findComponentWithShape(closestHit.Shape);
                if (comp != null)
                {
                    comp.DisposeActors();
                    components.Remove(comp);
                }


            }



            if (Game.Keyboard.IsKeyDown(Keys.R))
            {
                temp(Game, actor);
                /*mobileCog.Dispose();
                mobileCog = CreateCog(triangleMesh, engine.Scene);*/
            }

            if (Game.Keyboard.IsKeyPressed(Keys.Enter) || Game.Keyboard.IsKeyPressed(Keys.A))
            {


                switch (placeMode)
                {
                    case PlaceMode.StaticBox:
                        ghostComponent = new StaticBox(Engine);
                        components.Add(ghostComponent);
                        break;
                    case PlaceMode.Cog:
                        ghostComponent = new Cog(this, Engine);
                        components.Add(ghostComponent);
                        break;

                }

            }


            if (Game.Keyboard.IsKeyPressed(Keys.NumPad9))
            {
                placeMode = PlaceMode.StaticBox;
            }
            if (Game.Keyboard.IsKeyPressed(Keys.NumPad1))
            {
                placeMode = PlaceMode.Cog;
            }



            Engine.Update(Game.Elapsed);
        }

        private ICogComponent findComponentWithShape(Shape shape)
        {
            for (int i = 0; i < components.Count; i++)
            {
                var ic = components[i];
                if (ic.ContainsShape(shape))
                    return ic;

            }
            return null;
        }





        public static Actor CreateCogHolder(PhysicsEngine engine, out Actor holderActor)
        {
            Actor actor;
            actor = CreateCog(engine.Scene);
            actor.GlobalPose = Matrix.CreateRotationX(MathHelper.PiOver2);

            CapsuleShapeDescription capsuleShapeDesc = new CapsuleShapeDescription(1f, 3f);

            var actorDesc = new ActorDescription(capsuleShapeDesc);

            var bodyDesc = new BodyDescription(10000);
            bodyDesc.BodyFlags |= BodyFlag.Kinematic;

            actorDesc.GlobalPose = Matrix.CreateTranslation(Vector3.Up + new Vector3(0, 0, 0));
            actorDesc.BodyDescription = bodyDesc;

            holderActor = engine.Scene.CreateActor(actorDesc);


            CylindricalJointDescription jointDesc = new CylindricalJointDescription();
            jointDesc.Actor1 = actor;
            jointDesc.Actor2 = holderActor;

            jointDesc.SetGlobalAnchor(new Vector3(0, 0, 0));
            jointDesc.SetGlobalAxis(Vector3.Up);


            var joint = engine.Scene.CreateJoint(jointDesc);
            joint.AddLimitPlane(new LimitPlane(new Plane(Vector3.Up, 0.0f), 0));
            joint.AddLimitPlane(new LimitPlane(new Plane(Vector3.Down, 0f), 0));
            return actor;
        }

        public static void temp(IXNAGame Game, Actor actor)
        {
            var mat = actor.CenterOfMassGlobalOrientation;

            Game.LineManager3D.AddLine(actor.CenterOfMassGlobalPosition, actor.CenterOfMassGlobalPosition + mat.Forward, Color.LightGreen);
            actor.AddTorque(mat.Forward * 10000, ForceMode.Force);

        }

        public static Actor CreateCog(StillDesign.PhysX.Scene scene)
        {
            var actorDesc = new ActorDescription();


            float radius = 6f;
            float radius2 = 6.8f;

            for (int i = 0; i < 16; i++)
            {
                SphereShapeDescription desc = new SphereShapeDescription(0.6f);
                float angle = (i + 0.5f) * MathHelper.TwoPi * (1 / 16f);
                desc.LocalPosition = new Vector3(radius * (float)Math.Sin(angle), radius * (float)Math.Cos(angle), 0);
                actorDesc.Shapes.Add(desc);

                desc = new SphereShapeDescription(0.2f);
                desc.LocalPosition = new Vector3(radius2 * (float)Math.Sin(angle), radius2 * (float)Math.Cos(angle), 0);
                actorDesc.Shapes.Add(desc);

            }



            actorDesc.BodyDescription = new BodyDescription();
            actorDesc.BodyDescription.Mass = 10;

            actorDesc.GlobalPose = Matrix.CreateRotationX(MathHelper.PiOver2);

            return scene.CreateActor(actorDesc);
        }

    }
}
