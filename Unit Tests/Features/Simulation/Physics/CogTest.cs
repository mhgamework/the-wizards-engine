using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Cogs;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using StillDesign.PhysX;
using Ray = StillDesign.PhysX.Ray;

namespace MHGameWork.TheWizards.Tests.Physics
{
    [TestFixture]
    public class CogTest
    {
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestTwoCogs()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.CameraPosition = new Vector3(0, 0, -40);
            game.SpectaterCamera.FarClip = 10000;
            PhysicsEngine engine = new PhysicsEngine();
            TriangleMesh triangleMesh = null;
            //game.AddXNAObject(engine);

            Actor actor = null;
            Actor holderActor = null;
            Actor holderActor2 = null;

            game.InitializeEvent += delegate
            {
                engine.Initialize();

                PhysicsDebugRendererXNA debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);

                game.AddXNAObject(debugRenderer);


                actor = CogEngine.CreateCogHolder(engine, out holderActor);
                Joint joint;
                CylindricalJointDescription jointDesc;
                ActorDescription actorDesc;
                BodyDescription bodyDesc;
                CapsuleShapeDescription capsuleShapeDesc;


                Actor mobileCog = CogEngine.CreateCog(engine.Scene);
                mobileCog.GlobalPose = Matrix.CreateRotationX(MathHelper.PiOver2);

                capsuleShapeDesc = new CapsuleShapeDescription(4.7f, 3f);

                actorDesc = new ActorDescription(capsuleShapeDesc);

                bodyDesc = new BodyDescription(10000);
                bodyDesc.BodyFlags |= BodyFlag.Kinematic;

                actorDesc.GlobalPose = Matrix.CreateTranslation(Vector3.Up + new Vector3(0, 0, 0));
                actorDesc.BodyDescription = bodyDesc;

                holderActor2 = engine.Scene.CreateActor(actorDesc);


                jointDesc = new CylindricalJointDescription();
                jointDesc.Actor1 = mobileCog;
                jointDesc.Actor2 = holderActor2;

                jointDesc.SetGlobalAnchor(new Vector3(0, 0, 0));
                jointDesc.SetGlobalAxis(Vector3.Up);


                joint = engine.Scene.CreateJoint(jointDesc);
                joint.AddLimitPlane(new LimitPlane(new Plane(Vector3.Up, 1), 0));



            };

            game.UpdateEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var bol = PhysicsHelper.CreateDynamicSphereActor(engine.Scene, 1, 1);
                    bol.GlobalPosition = game.SpectaterCamera.CameraPosition +
                                           game.SpectaterCamera.CameraDirection * 5;
                    bol.LinearVelocity = game.SpectaterCamera.CameraDirection * 5;
                }

                Plane p = new Plane(Vector3.Up, 0);
                var ray = game.GetWereldViewRay(new Vector2(game.Window.ClientBounds.Width * 0.5f,
                                                  game.Window.ClientBounds.Height * 0.5f));

                float? dist = ray.Intersects(p);
                if (dist.HasValue && game.Keyboard.IsKeyDown(Keys.C))
                {
                    //mobileCog.GlobalPosition = ray.Position + ray.Direction * dist.Value + Vector3.Up;
                    holderActor2.MoveGlobalPositionTo(ray.Position + ray.Direction * dist.Value + Vector3.Up);

                }

                if (game.Keyboard.IsKeyDown(Keys.R))
                {
                    CogEngine.temp(game, actor);
                    /*mobileCog.Dispose();
                    mobileCog = CreateCog(triangleMesh, engine.Scene);*/
                }




                engine.Update(game.Elapsed);
            };

            game.Run();

            engine.Dispose();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestCogsFun()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.CameraPosition = new Vector3(0, 0, -40);
            game.SpectaterCamera.FarClip = 10000;
            PhysicsEngine engine = new PhysicsEngine();

            Actor actor = null;

            Actor ghostCogHolder = null;

            game.InitializeEvent += delegate
            {
                engine.Initialize();

                PhysicsDebugRendererXNA debugRenderer = new PhysicsDebugRendererXNA(game, engine.Scene);

                game.AddXNAObject(debugRenderer);


                Actor holderActor;
                actor = CogEngine.CreateCogHolder(engine, out holderActor);





            };

            game.UpdateEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    var bol = PhysicsHelper.CreateDynamicSphereActor(engine.Scene, 1, 1);
                    bol.GlobalPosition = game.SpectaterCamera.CameraPosition +
                                           game.SpectaterCamera.CameraDirection * 5;
                    bol.LinearVelocity = game.SpectaterCamera.CameraDirection * 5;
                }


                Plane p = new Plane(Vector3.Up, 0);
                var ray = game.GetWereldViewRay(new Vector2(game.Window.ClientBounds.Width * 0.5f,
                                                  game.Window.ClientBounds.Height * 0.5f));

                float? dist = ray.Intersects(p);
                if (dist.HasValue && game.Keyboard.IsKeyDown(Keys.C))
                {
                    if (ghostCogHolder != null)
                        ghostCogHolder.MoveGlobalPositionTo(ray.Position + ray.Direction * dist.Value + Vector3.Up * 7f);

                }

                if (game.Keyboard.IsKeyDown(Keys.R))
                {
                    CogEngine.temp(game, actor);
                    /*mobileCog.Dispose();
                    mobileCog = CreateCog(triangleMesh, engine.Scene);*/
                }

                if (game.Keyboard.IsKeyPressed(Keys.Enter))
                {
                    CogEngine.CreateCogHolder(engine, out ghostCogHolder);

                    //ghostCogHolder.MoveGlobalPositionTo(Vector3.UnitX * 30 );
                    ghostCogHolder.MoveGlobalOrientationTo(Matrix.CreateRotationX(MathHelper.PiOver2) *
                                                           Matrix.CreateTranslation(Vector3.UnitX * 30 + Vector3.UnitY * 20));
                }



                engine.Update(game.Elapsed);
            };

            game.Run();

            engine.Dispose();
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestAdvancedCogEngine()
        {
            XNAGame game = new XNAGame();
            game.SpectaterCamera.CameraPosition = new Vector3(0, 0, -40);
            game.SpectaterCamera.FarClip = 10000;
            PhysicsEngine engine = new PhysicsEngine();

            CogEngine cogEngine = new CogEngine(game, engine);

            game.AddXNAObject(cogEngine);

            game.Run();

            engine.Dispose();
        }



    }
}
