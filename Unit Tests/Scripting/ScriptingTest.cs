using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.Client;
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
using Microsoft.Xna.Framework;
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
            var fi = new FileInfo(TWDir.Binaries + "/../../Scripts/TestScript.cs");


            var ent = new TheWizards.Scene.Entity(scene);
            ent.Mesh = twGame.BarrelMesh;

            loader.LoadScript(ent, fi);


            twGame.Game.Run();

        }
        [Test]
        public void TestPlayerUseListener()
        {
            var twGame = new TestTWGame();

            twGame.SetScriptLayerScope();

            var controller = new HelperPlayerController(twGame.Game, new PlayerData());
            twGame.Game.AddXNAObject(controller);
            var scene = new TheWizards.Scene.Scene(twGame.Renderer, twGame.PhysicsFactory);


            twGame.Game.UpdateEvent += delegate
                                           {
                                               if (twGame.Game.Keyboard.IsKeyPressed(Keys.E))
                                               {
                                                   var ray = new Ray();
                                                   var ent = scene.RaycastEntityPhysX(ray, o => true);
                                               }
                                           };



            twGame.Game.Run();
        }


    }
}
