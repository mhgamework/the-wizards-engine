﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Gameplay.Fortress;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class FortressTest
    {
        [Test]
        public void TestPlayer()
        {

            var game = new TestTWGame();

            var scene = new TheWizards.Scene.Scene(game.Renderer, game.PhysicsFactory);
            game.Game.AddXNAObject(scene);
            var psc = new PlayerSceneComponent(scene);
            game.Game.AddXNAObject(psc);
     
            game.Game.UpdateEvent += delegate
                                     {
                                         if (game.Game.FrameNumber == 1)
                                         {
                                             var ent = scene.CreateEntity();

                                             var playerEntityScript = new PlayerEntity();

                                             var handle = ent.CreateEntityHandle(playerEntityScript);

                                             scene.ExecuteInScriptScope(handle, () => playerEntityScript.Init(handle));

                                         }
                                     };

            game.Game.Run();
        }

        [Test]
        public void TestPickupSpawnCrystal()
        {

            var game = new TestTWGame();

            var scene = new TheWizards.Scene.Scene(game.Renderer, game.PhysicsFactory);
            game.Game.AddXNAObject(scene);
            var psc = new PlayerSceneComponent(scene);
            game.Game.AddXNAObject(psc);

            game.Game.UpdateEvent += delegate
            {
                if (game.Game.FrameNumber == 1)
                {
                    var ent = scene.CreateEntity();
                    var playerEntityScript = new PlayerEntity();
                    var handle = ent.CreateEntityHandle(playerEntityScript);
                    scene.ExecuteInScriptScope(handle, () => playerEntityScript.Init(handle));


                    ent = scene.CreateEntity();
                    ent.Mesh = game.BarrelMesh;
                    ent.Solid = true;
                    ent.Static = false;
                    ent.Transformation = new TheWizards.Graphics.Transformation(Vector3.One*2); 
                    var crystal = new SpawnCrystal();
                    var handle2 = ent.CreateEntityHandle(crystal);
                    scene.ExecuteInScriptScope(handle, () => crystal.Init(handle2));

                }
            };

            game.Game.Run();
        }
    }
}
