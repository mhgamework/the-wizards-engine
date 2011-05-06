using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scene;
using MHGameWork.TheWizards.Scripting;
using MHGameWork.TheWizards.Scripting.API;
using MHGameWork.TheWizards.Tests.Gameplay;
using MHGameWork.TheWizards.Tests.OBJParser;
using MHGameWork.TheWizards.Tests.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Scripting
{
    [TestFixture]
    public class ScriptingTest
    {
        [Test]
        public void TestRunScript()
        {
            var game = new XNAGame();

            var runner = new ScriptRunner(game);

            game.InitializeEvent += delegate
                                        {
                                            runner.RunScript(new SimpleStateScript());
                                        };


            game.Run();
        }

        [Test]
        public void TestSceneScriptLoader()
        {
            var twGame = new TestTWGame();

            var scene = new TheWizards.Scene.Scene(twGame.Renderer, twGame.PhysicsFactory);
            twGame.Game.AddXNAObject(scene);

            var loader = new SceneScriptLoader(scene);
            twGame.Game.AddXNAObject(loader);
            var fi = new FileInfo(TWDir.Scripts + "/TestScript.cs");


            var ent = new TheWizards.Scene.Entity(scene);
            ent.Mesh = twGame.BarrelMesh;

            loader.LoadScript(ent, fi);


            twGame.Game.Run();

        }

        [Test]
        public void TestScriptDataBinder()
        {
            var s = new SimpleScriptDatabinding();
            var d = new EntityData();
            d.GetDataElement<string>("Name").Set("TestScript");
            var b = new ScriptDataBinder();
            b.LoadData(s, d);

            s.Data = 8;

            b.SaveData(s, d);

        }
        [Test]
        public void TestPlayerUseListener()
        {

            var twGame = new TestTWGame();

            twGame.SetScriptLayerScope();

            var player = new SimplePlayer();
            player.Data.Name = "MHGameWork";
            var controller = new HelperPlayerController(twGame.Game, player.Data);
            twGame.Game.AddXNAObject(controller);
            twGame.Game.SetCamera(controller.ThirdPersonCamera);

            var scene = new TheWizards.Scene.Scene(twGame.Renderer, twGame.PhysicsFactory);
            var ent = scene.CreateEntity();
            ent.Mesh = twGame.BarrelMesh;
            ent.Transformation = new Transformation(Vector3.Up * 0.5f + Vector3.Forward * 3);

            var loader = new SceneScriptLoader(scene);
            loader.LoadScript(ent, new FileInfo(TWDir.Scripts + "\\TestUseScript.cs"));
            twGame.Game.AddXNAObject(loader);
            twGame.PhysicsTreeRoot.AddDynamicObjectToIntersectingNodes(new ClientPhysicsTestSphere(Vector3.Zero, 100));

            twGame.Game.UpdateEvent += delegate
                                           {
                                               var pos = controller.Controller.GlobalPosition;
                                               var dir = Vector3.Transform(Vector3.Forward,
                                                                           controller.ThirdPersonCamera.ViewInverse)-
                                                                           Vector3.Transform(Vector3.Zero,
                                                                           controller.ThirdPersonCamera.ViewInverse);
                                               dir.Normalize();
                                               var ray = new Ray(pos, dir);
                                               //NOTE: this ray shouldnt be visible :-)
                                               twGame.Game.LineManager3D.AddRay(ray, Color.Green);

                                               GameplayTest.ProcessPlayerInputDirect(controller.Controller, twGame.Game);
                                               if (twGame.Game.Keyboard.IsKeyPressed(Keys.E))
                                               {

                                                   var result = scene.RaycastEntityPhysX(ray, o => true);
                                                   if (result != null)
                                                   {
                                                       result.Entity.RaisePlayerUse(player);
                                                   }
                                               }
                                           };



            twGame.Game.Run();
        }

        [Test]
        public void TestOpenDoor()
        {
            var twGame = new TestTWGame();

            twGame.SetScriptLayerScope();

            var player = new SimplePlayer();
            player.Data.Name = "MHGameWork";
            var controller = new HelperPlayerController(twGame.Game, player.Data);
            twGame.Game.AddXNAObject(controller);
            twGame.Game.SetCamera(controller.ThirdPersonCamera);

            var scene = new TheWizards.Scene.Scene(twGame.Renderer, twGame.PhysicsFactory);
            var ent = scene.CreateEntity();
            ent.Mesh = RenderingTest.CreateMeshFromObj(new OBJToRAMMeshConverter(new RAMTextureFactory()),
                                                       TestFiles.StorageHouseDoorLeftObj,
                                                       TestFiles.StorageHouseDoorLeftMtl);

            ent.Transformation = new Transformation(Vector3.Up * 0.5f + Vector3.Forward * 3);
            ent.Static = false;
            ent.Kinematic = true;

            var loader = new SceneScriptLoader(scene);
            loader.LoadScript(ent, new FileInfo(TWDir.Scripts + "\\TestOpenDoor.cs"));
            twGame.Game.AddXNAObject(loader);
            twGame.PhysicsTreeRoot.AddDynamicObjectToIntersectingNodes(new ClientPhysicsTestSphere(Vector3.Zero, 100));

            twGame.Game.UpdateEvent += delegate
            {
                var pos = controller.Controller.GlobalPosition;
                var dir = Vector3.TransformNormal(controller.Controller.GetForwardVector(),twGame.Game.Camera.ViewInverse);
                dir.Normalize();
                var ray = new Ray(pos, dir);
                //NOTE: this ray shouldnt be visible :-)
                twGame.Game.LineManager3D.AddRay(ray, Color.Green);

                GameplayTest.ProcessPlayerInputDirect(controller.Controller, twGame.Game);
                if (twGame.Game.Keyboard.IsKeyPressed(Keys.E))
                {

                    var result = scene.RaycastEntityPhysX(ray, o => true);
                    if (result != null)
                    {
                        result.Entity.RaisePlayerUse(player);
                    }
                }
            };



            twGame.Game.Run();
        }
    }
}
